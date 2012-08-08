/*
 *  Copyright 2012 Tamme Schichler <tammeschichler@googlemail.com>
 * 
 *  This file is part of ~ATH.
 *
 *  ~ATH is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  ~ATH is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with ~ATH.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using _ATH.Expressions;

namespace _ATH
{
    public class _ATHProgram
    {
        private readonly _ATHExpression[] _expressions;

        private MethodInfo _entryPoint;
        private AssemblyBuilder _assemblyBuilder;

        public _ATHExpression[] Expressions
        {
            get { return _expressions; }
        }


        public _ATHProgram(string source)
        {
            var expressions = new List<_ATHExpression>();

            int position = 0;
            var lexer = new _ATHLexer(new[]{
                typeof(BifurcateExpression),
                typeof(CommentExpression),
                typeof(DIEExpression),
                typeof(ExpressionBlockExpression),
                typeof(ImportExpression),
                typeof(NULLExpression),
                typeof(THISListDIEExpression),
                typeof(TildeATHExpression)});

            _ATHLexer.SkipWhitespace(source, ref position);

            do
            {
                while (_ATHLexer.MaybeSkipColour(source, ref position, new Colour()))
                {
                    _ATHLexer.SkipWhitespace(source, ref position);
                }
                expressions.Add(lexer.ReadExpression(source, ref position, new Colour()));
                _ATHLexer.SkipWhitespace(source, ref position);
            } while (position < source.Length || !(IsTHISDIE(expressions[expressions.Count - 1]) || IsBifurcateTHIS(expressions[expressions.Count - 1])));

            _expressions = expressions.ToArray();
        }

        private bool IsTHISDIE(_ATHExpression expression)
        {
            var dieExpression = expression as DIEExpression;
            if (dieExpression == null) return false;

            return dieExpression.Target == "THIS";
        }

        private bool IsBifurcateTHIS(_ATHExpression expression)
        {
            var bifurcateExpression = expression as BifurcateExpression;
            if (bifurcateExpression == null) return false;
            return true;
        }

        #region THIS
        TypeBuilder _typeBuilder;
        Dictionary<Colour, FieldBuilder> _thisFields;

        private FieldBuilder GetOrCreateTHISField(Colour colour)
        {
            FieldBuilder field;
            while (!_thisFields.TryGetValue(colour, out field))
            {
                RegisterTHIS(colour);
            }
            return field;
        }

        private void RegisterTHIS(Colour colour)
        {
            var field = _typeBuilder.DefineField("THIS" + colour.HexString, typeof(uint), FieldAttributes.Static);
            field.SetConstant(0u);

            _thisFields.Add(colour, field);
        }

        public FieldBuilder AddStaticField(string name, Type type, FieldAttributes attributes)
        {
            if (name.StartsWith("THIS"))
            {
                throw new Exception("Custom static fields can't start with THIS.");
            }

            return _typeBuilder.DefineField(name, type, attributes | FieldAttributes.Static);
        }

        internal void EmitIsTHISAlive(ILGenerator ilGenerator, Colour colour)
        {
            ilGenerator.Emit(OpCodes.Volatile);
            ilGenerator.Emit(OpCodes.Ldsfld, GetOrCreateTHISField(colour));
        }

        internal void EmitKillTHIS(ILGenerator ilGenerator, Colour colour)
        {
            ilGenerator.Emit(OpCodes.Ldc_I4_0);
            ilGenerator.Emit(OpCodes.Volatile);
            ilGenerator.Emit(OpCodes.Stsfld, GetOrCreateTHISField(colour));
        }

        internal void EmitDieIfKilled(ILGenerator ilGenerator, Colour colour)
        {
            var skipDieLabel = ilGenerator.DefineLabel();

            ilGenerator.Emit(OpCodes.Volatile);
            ilGenerator.Emit(OpCodes.Ldsfld, GetOrCreateTHISField(colour));
            ilGenerator.Emit(OpCodes.Brtrue, skipDieLabel);
            ilGenerator.Emit(OpCodes.Ret);
            ilGenerator.MarkLabel(skipDieLabel);
        }

        internal void EmitTHISFork(ILGenerator ilGenerator, Colour[] Colours, MethodBuilder[] methodBuilders)
        {
            foreach (var colour in Colours)
            {
                EmitTHISStart(ilGenerator, colour);
            }

            var threadStartConstructor = typeof(ThreadStart).GetConstructor(new[] { typeof(object), typeof(IntPtr) });
            var threadConstructor = typeof(Thread).GetConstructor(new[] { typeof(ThreadStart) });
            var threadStart = typeof(Thread).GetMethod("Start", Type.EmptyTypes);

            foreach (var methodBuilder in methodBuilders)
            {
                ilGenerator.Emit(OpCodes.Ldnull);
                ilGenerator.Emit(OpCodes.Ldftn, methodBuilder);
                ilGenerator.Emit(OpCodes.Newobj, threadStartConstructor);
                ilGenerator.Emit(OpCodes.Newobj, threadConstructor);
                ilGenerator.EmitCall(OpCodes.Call, threadStart, null);
            }
        }

        internal void EmitTHISJoin(ILGenerator ilGenerator, Colour expressionColour, Colour[] Colours)
        {
            var joinStartLabel = ilGenerator.DefineLabel();
            ilGenerator.MarkLabel(joinStartLabel);
            {
                var dontKillSelfLabel = ilGenerator.DefineLabel();

                bool first = true;

                foreach (var colour in Colours)
                {
                    EmitIsTHISAlive(ilGenerator, colour);
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        ilGenerator.Emit(OpCodes.Add_Ovf_Un);
                    }
                }

                ilGenerator.Emit(OpCodes.Brtrue, dontKillSelfLabel);
                EmitKillTHIS(ilGenerator, expressionColour);
                ilGenerator.MarkLabel(dontKillSelfLabel);
            }
            {
                var dontKillBranchesLabel = ilGenerator.DefineLabel();

                EmitIsTHISAlive(ilGenerator, expressionColour);
                ilGenerator.Emit(OpCodes.Brtrue, dontKillBranchesLabel);

                foreach (var colour in Colours)
                {
                    EmitKillTHIS(ilGenerator, colour);
                }

                ilGenerator.Emit(OpCodes.Ret);

                ilGenerator.MarkLabel(dontKillBranchesLabel);
            }

#if !NODELAY
            var threadSleep = ((Action<int>)Thread.Sleep).Method;

            ilGenerator.Emit(OpCodes.Ldc_I4, 100);
            ilGenerator.EmitCall(OpCodes.Call, threadSleep, null);
#endif

            ilGenerator.Emit(OpCodes.Br, joinStartLabel);
        }

        private void EmitBlackTHISStart(ILGenerator ilGenerator)
        {
            var black = new Colour();

            black = EmitTHISStart(ilGenerator, black);
        }

        internal MethodBuilder CreateTHISMethod(Colour colour)
        {
            return _typeBuilder.DefineMethod("THIS" + colour.HexString, MethodAttributes.Public | MethodAttributes.Static);
        }

        private Colour EmitTHISStart(ILGenerator ilGenerator, Colour colour)
        {
            ilGenerator.Emit(OpCodes.Ldc_I4_1);
            ilGenerator.Emit(OpCodes.Volatile);
            ilGenerator.Emit(OpCodes.Stsfld, GetOrCreateTHISField(colour));
            return colour;
        }
        #endregion

        public void Save(string path)
        {
            _assemblyBuilder.Save(Path.GetFileName(path));

            File.Move(Path.GetFileName(path), path);
        }

        public void Run()
        {
            _entryPoint.Invoke(null, null);
        }

        public void Compile(string name, Type[] importTypes)
        {
            var assemblyName = new AssemblyName();
            assemblyName.Name = name;
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);

            var moduleBuilder = assemblyBuilder.DefineDynamicModule(name, name + ".exe");

            var typeBuilder = moduleBuilder.DefineType("Program", TypeAttributes.Public);

            var methodBuilder = typeBuilder.DefineMethod("THIS000000", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, null, null);

            { // Method

                var importHandles = new Dictionary<string, ImportHandle>();
                for (int i = 0; i < importTypes.Length; i++)
                {
                    var importType = importTypes[i];
                    if ((typeof(_ATHImport).IsAssignableFrom(importType) == false) || (importType.IsAbstract == true))
                    {
                        throw new ArgumentException("Invalid import: " + importType.ToString(), "imports");
                    }

                    var keyword = (string)importType.GetProperty("Keyword", typeof(string)).GetGetMethod().Invoke(null, null);
                    var emitImport = (Action<_ATHProgram, ILGenerator, Colour, Tuple<string, Colour>>)Delegate.CreateDelegate(typeof(Action<_ATHProgram, ILGenerator, Colour, Tuple<string, Colour>>), importType.GetMethod("EmitImport"));
                    var emitIsAlive = (Action<_ATHProgram, ILGenerator, Tuple<string, Colour>>)Delegate.CreateDelegate(typeof(Action<_ATHProgram, ILGenerator, Tuple<string, Colour>>), importType.GetMethod("EmitIsAlive"));
                    var emitDie = (Action<_ATHProgram, ILGenerator, Tuple<string, Colour>>)Delegate.CreateDelegate(typeof(Action<_ATHProgram, ILGenerator, Tuple<string, Colour>>), importType.GetMethod("EmitDie"));

                    importHandles.Add(keyword, new ImportHandle()
                    {
                        Keyword = keyword,
                        Import = importType,
                        EmitImport = emitImport,
                        EmitIsAlive = emitIsAlive,
                        EmitDie = emitDie
                    });
                }

                var objects = new Dictionary<Tuple<string, Colour>, ImportHandle>();

                var ilGenerator = methodBuilder.GetILGenerator();

                _typeBuilder = typeBuilder;
                _thisFields = new Dictionary<Colour, FieldBuilder>();

                EmitBlackTHISStart(ilGenerator);
                foreach (var expression in _expressions)
                {
                    EmitDieIfKilled(ilGenerator, new Colour());

                    expression.EmitIL(this, new Colour(), ilGenerator, importHandles, objects);
                }

                EmitDieIfKilled(ilGenerator, new Colour());

                //Necessary for JIT-Compiler
                ilGenerator.ThrowException(typeof(InvalidProgramException));

                _typeBuilder = null;
                _thisFields = null;
            }

            assemblyBuilder.SetEntryPoint(methodBuilder, PEFileKinds.ConsoleApplication);

            _entryPoint = typeBuilder.CreateType().GetMethod("THIS000000", BindingFlags.Public | BindingFlags.Static);

            _assemblyBuilder = assemblyBuilder;
        }
    }
}
