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
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Diagnostics;
using System.IO;
using _ATH.Tokens;
using System.Threading;

namespace _ATH.Expressions
{
    public class TildeATHExpression : _ATHExpression
    {
        public static string Keyword
        {
            get { return "~ATH"; }
        }

        public bool Not { get; private set; }
        public string Target { get; private set; }
        public Colour? TargetColour { get; private set; }

        private _ATHExpression[] _loopExpressions;

        public _ATHExpression[] LoopExpressions
        {
            get
            {
                var result = new _ATHExpression[_loopExpressions.Length];
                Array.Copy(_loopExpressions, result, _loopExpressions.Length);
                return result;
            }
        }


        public _ATHExpression ExecuteExpression { get; private set; }
        public string ExecuteCommand { get; private set; }

        public TildeATHExpression(
            SkipWhitespaceToken a,
            RoundOpenToken b,
            SkipWhitespaceToken c,
            MaybeNotToken not,
            MaybeColouredNameToken target,
            SkipWhitespaceToken d,
            RoundCloseToken e,
            SkipWhitespaceToken f,
            CurlyOpenToken g,
            SkipWhitespaceToken h,
            CurlyCloseDelimitedExpressionListToken loopExpressions,
            SkipWhitespaceToken i,
            EXECUTEToken j,
            SkipWhitespaceToken k,
            RoundOpenToken l,
            SkipWhitespaceToken m,
            ExpressionOrCommandToken executeExpressionOrCommand,
            SkipWhitespaceToken n,
            RoundCloseToken o,
            SkipWhitespaceToken p,
            SemicolonToken q)
        {
            Not = not.Not;
            Target = target.Name;
            TargetColour = target.Colour;
            _loopExpressions = loopExpressions.Expressions;
            ExecuteExpression = executeExpressionOrCommand.Expression;
            ExecuteCommand = executeExpressionOrCommand.Command;
        }

        public override void EmitIL(_ATHProgram program, Colour expressionColour, ILGenerator ilGenerator, Dictionary<string, ImportHandle> importHandles, Dictionary<Tuple<string, Colour>, ImportHandle> objects)
        {
            var startLabel = ilGenerator.DefineLabel();
            var endLabel = ilGenerator.DefineLabel();

            ilGenerator.MarkLabel(startLabel);

            if (Target == "THIS")
            {
                program.EmitIsTHISAlive(ilGenerator, TargetColour ?? expressionColour);

                if (Not)
                {
                    ilGenerator.Emit(OpCodes.Brtrue, endLabel);
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Brfalse, endLabel);
                }
            }
            else
            {
                var target = Tuple.Create(Target, TargetColour ?? expressionColour);
                objects[target].EmitIsAlive(ilGenerator, target);
                if (Not)
                {
                    ilGenerator.Emit(OpCodes.Brtrue, endLabel);
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Brfalse, endLabel);
                }
            }

            foreach (var expression in _loopExpressions)
            {
                program.EmitDieIfKilled(ilGenerator, expressionColour);

                expression.EmitIL(program, expressionColour, ilGenerator, importHandles, objects);
            }

#if !NODELAY
            var threadSleep = ((Action<int>)Thread.Sleep).Method;

            ilGenerator.Emit(OpCodes.Ldc_I4, 1000);
            ilGenerator.EmitCall(OpCodes.Call, threadSleep, null);
#endif

            ilGenerator.Emit(OpCodes.Br, startLabel);

            ilGenerator.MarkLabel(endLabel);

            if (ExecuteExpression != null)
            {
                if (ExecuteExpression is NULLExpression)
                {
                    // Empty.
                }
                else
                {
                    program.EmitDieIfKilled(ilGenerator, expressionColour);

                    ExecuteExpression.EmitIL(program, expressionColour, ilGenerator, importHandles, objects);
                }
            }
            else
            {
                var getFullPath = ((Func<string, string>)Path.GetFullPath).Method;
                var startProcess = ((Func<string, Process>)Process.Start).Method;

                ilGenerator.Emit(OpCodes.Ldstr, ExecuteCommand);
                ilGenerator.EmitCall(OpCodes.Call, getFullPath, null);
                ilGenerator.EmitCall(OpCodes.Call, startProcess, null);
                ilGenerator.Emit(OpCodes.Pop);
            }
        }
    }
}
