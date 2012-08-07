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
using System.IO;
using System.Reflection;
using _ATH.Tokens;

namespace _ATH
{
    public class _ATHLexer
    {
        Type[] _expressionTypes;
        string[] _expressionKeywords;
        Type[][] _expressionTokenTypes;

        public _ATHLexer(Type[] expressionTypes)
        {
            for (int i = 0; i < expressionTypes.Length; i++)
            {
                if (typeof(_ATHExpression).IsAssignableFrom(expressionTypes[i]) == false)
                {
                    throw new ArgumentException(expressionTypes[i].ToString() + " isn't assignable to " + typeof(_ATHExpression).ToString(), "expressionTypes");
                }
            }

            _expressionTypes = expressionTypes;
            _expressionKeywords = _expressionTypes.Select(x => (string)x.GetProperty("Keyword", typeof(string)).GetGetMethod().Invoke(null, null)).ToArray();
            _expressionTokenTypes = _expressionTypes.Select(x => x.GetConstructors()[0].GetParameters().Select(p => p.ParameterType).Where(p => typeof(_ATHToken).IsAssignableFrom(p)).ToArray()).ToArray();
        }

        public _ATHExpression ReadExpression(string source, ref int position, Colour expressionColour, Type ignoreFinal = null)
        {
            //SkipWhitespace(source, ref position);

            for (int exi = 0; exi < _expressionTypes.Length; exi++)
            {
                var keyword = _expressionKeywords[exi];
                if (keyword != null &&
                    source.Length > position + keyword.Length &&
                    source.Substring(position, keyword.Length) == keyword &&
                    (char.IsLetterOrDigit(source, position + keyword.Length) == false))
                {
                    var expressionType = _expressionTypes[exi];
                    var expressionTokenTypes = _expressionTokenTypes[exi];

                    var expression = ReadExpression(source, ref position, keyword.Length, expressionType, expressionTokenTypes, expressionColour, ignoreFinal);

                    return expression;
                }
            }

            // Default expression
            for (int exi = 0; exi < _expressionTypes.Length; exi++)
            {
                if (_expressionKeywords[exi] == null)
                {
                    var expressionType = _expressionTypes[exi];
                    var expressionTokenTypes = _expressionTokenTypes[exi];

                    var expression = ReadExpression(source, ref position, 0, expressionType, expressionTokenTypes, expressionColour, ignoreFinal);

                    return expression;
                }
            }

            throw new _ATHParserException("Expression expected at " + position + ".");
        }

        private _ATHExpression ReadExpression(string source, ref int position, int keywordLength, Type expressionType, Type[] expressionTokenTypes, Colour expressionColour, Type ignoreFinal = null)
        {
            position += keywordLength;

            var tokens = new _ATHToken[expressionTokenTypes.Length];
            for (int ti = 0; ti < tokens.Length; ti++)
            {
                var tokenType = expressionTokenTypes[ti];
                if ((ignoreFinal != null && ti == tokens.Length - 1 && tokenType == ignoreFinal) == false)
                {
                    var readMethod = tokenType.GetMethod("Read", BindingFlags.Public | BindingFlags.Static);

                    object[] parameters = null;

                    switch (readMethod.GetParameters().Length)
                    {
                        case 2:
                            parameters = new object[] { source, position };
                            break;
                        case 3:
                            parameters = new object[] { source, position, this };
                            break;
                        case 4:
                            parameters = new object[] { source, position, this, expressionColour };
                            break;
                    }

                    tokens[ti] = (_ATHToken)readMethod.Invoke(null, parameters);
                    position = (int)parameters[1];
                }
                else
                {
                    tokens[ti] = new SemicolonToken();
                }
            }

            var expression = (_ATHExpression)expressionType.GetConstructor(expressionTokenTypes).Invoke(tokens);
            return expression;
        }

        public static void SkipWhitespace(string source, ref int position)
        {
            while (position < source.Length && char.IsWhiteSpace(source, position))
            {
                position++;
            }
        }

        public static bool MaybeSkipColour(string source, ref int position, params Colour[] colours)
        {
            if (source[position] != '#')
            {
                return false;
            }

            position++;

            foreach (var colour in colours)
            {
                var colourString = colour.HexString;
                if (source.Substring(position, colourString.Length).ToUpperInvariant() == colourString.ToUpperInvariant())
                {
                    position += colourString.Length;
                    return true;
                }
            }

            position--;
            throw new _ATHParserException("Wrong colour at " + position + ".");
        }
    }
}