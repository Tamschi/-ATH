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
using System.Reflection.Emit;
using _ATH.Tokens;

namespace _ATH.Expressions
{
    public class ExpressionBlockExpression : _ATHExpression
    {
        public static string Keyword
        {
            get { return "{"; }
        }

        private _ATHExpression[] _expressions;

        public _ATHExpression[] Expressions
        {
            get
            {
                var result = new _ATHExpression[_expressions.Length];
                Array.Copy(_expressions, result, _expressions.Length);
                return result;
            }
        }


        public ExpressionBlockExpression(
            SkipWhitespaceToken a,
            CurlyCloseDelimitedExpressionListToken expressions)
        {
            _expressions = expressions.Expressions;
        }

        public override void EmitIL(_ATHProgram program, Colour expressionColour, ILGenerator ilGenerator, Dictionary<string, ImportHandle> importHandles, Dictionary<Tuple<string, Colour>, ImportHandle> objects)
        {
            foreach (var expression in _expressions)
            {
                expression.EmitIL(program, expressionColour, ilGenerator, importHandles, objects);
                program.EmitDieIfKilled(ilGenerator, expressionColour);
            }
        }
    }
}
