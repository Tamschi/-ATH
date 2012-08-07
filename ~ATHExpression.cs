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

namespace _ATH
{
    public abstract class _ATHExpression
    {
        //Implementations must have this property:
        //public static string Keyword { abstract get; }
        //
        //The default expression returns null.

        public abstract void EmitIL(_ATHProgram program, Colour expressionColour, ILGenerator ilGenerator, Dictionary<string, ImportHandle> importHandles, Dictionary<Tuple<string, Colour>, ImportHandle> objects);
    }
}
