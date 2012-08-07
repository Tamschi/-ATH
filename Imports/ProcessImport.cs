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
using System.Diagnostics;
using System.Reflection.Emit;
using System.IO;

namespace _ATH.Imports
{
    public class ProcessImport : _ATHImport
    {
        public static string Keyword { get { return "process"; } }

        public static void EmitImport(ILGenerator ilGenerator, Colour importColour, Tuple<string, Colour> name) // void()
        {
            // Empty.
        }

        public static void EmitIsAlive(ILGenerator ilGenerator, Tuple<string, Colour> name) // bool()
        {
            var getProcessesByName = ((Func<string, Process[]>)Process.GetProcessesByName).Method;

            ilGenerator.Emit(OpCodes.Ldstr, name.Item1);
            ilGenerator.EmitCall(OpCodes.Call, getProcessesByName, null);
            ilGenerator.Emit(OpCodes.Ldlen);
        }

        public static void EmitDie(ILGenerator ilGenerator, Tuple<string, Colour> name) // void()
        {
            var getProcessesByName = ((Func<string, Process[]>)Process.GetProcessesByName).Method;
            var killProcess = typeof(Process).GetMethod("Kill");

            ilGenerator.Emit(OpCodes.Ldstr, name.Item1);
            ilGenerator.EmitCall(OpCodes.Call, getProcessesByName, null);
            ilGenerator.Emit(OpCodes.Ldc_I4_0);
            ilGenerator.Emit(OpCodes.Ldelem_Ref);
            ilGenerator.EmitCall(OpCodes.Callvirt, killProcess, null);
        }

        public static void EmitExecute(ILGenerator ilGenerator, Tuple<string, Colour> name) // void()
        {
            var getFullPath = ((Func<string, string>)Path.GetFullPath).Method;
            var startProcess = ((Func<string, Process>)Process.Start).Method;

            ilGenerator.Emit(OpCodes.Ldstr, name.Item1);
            ilGenerator.EmitCall(OpCodes.Call, getFullPath, null);
            ilGenerator.EmitCall(OpCodes.Call, startProcess, null);
            ilGenerator.Emit(OpCodes.Pop);
        }
    }
}
