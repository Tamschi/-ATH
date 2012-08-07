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

namespace _ATH.Tokens
{
    public class CurlyCloseDelimitedExpressionListToken : _ATHToken
    {
        private _ATHExpression[] _expressions;

        public _ATHExpression[] Expressions
        {
            get { return _expressions; }
        }

        public CurlyCloseDelimitedExpressionListToken(_ATHExpression[] expressions)
        {
            _expressions = expressions;
        }

        public static _ATHToken Read(string source, ref int position, _ATHLexer lexer, Colour expressionColour)
        {
            var expressions = new List<_ATHExpression>();

            while (source.Length > position && source[position] != '}')
            {
                while (_ATHLexer.MaybeSkipColour(source, ref position, expressionColour))
                {
                    _ATHLexer.SkipWhitespace(source, ref position);
                }
                expressions.Add(lexer.ReadExpression(source, ref position, expressionColour));
                _ATHLexer.SkipWhitespace(source, ref position);
            }

            if (position >= source.Length)
            {
                throw new TokenException("} expected at " + position + ".");
            }

            position++;
            return new CurlyCloseDelimitedExpressionListToken(expressions.ToArray());
        }
    }
}
