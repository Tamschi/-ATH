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
using _ATH.Expressions;

namespace _ATH.Tokens
{
    public class ColouredExpressionsEndingWithTHISListDIEToken : _ATHToken
    {
        public Tuple<Colour, _ATHExpression>[] ColouredExpressions { get; private set; }

        public ColouredExpressionsEndingWithTHISListDIEToken(Tuple<Colour, _ATHExpression>[] colouredExpressions)
        {
            ColouredExpressions = colouredExpressions;
        }


        public static _ATHToken Read(string source, ref int position, _ATHLexer lexer, Colour expressionColour)
        {
            bool finished = false;
            var expressions = new List<Tuple<Colour, _ATHExpression>>();
            var lineBuffer = new Dictionary<Colour, string>();
            var lineBufferPositions = new Dictionary<Colour, int>();

            while (finished == false)
            {
                ProcessLine(source, ref position, out finished, lexer, expressionColour, ref expressions, ref lineBuffer, ref lineBufferPositions);
            }

            foreach (var colour in lineBuffer.Keys)
            {
                if (lineBufferPositions[colour] != lineBuffer[colour].Length)
                {
                    throw new _ATHParserException("Unfinished expression in colour #" + colour.HexString.ToUpperInvariant() + ".");
                }
            }

            return new ColouredExpressionsEndingWithTHISListDIEToken(expressions.ToArray());
        }

        private static void ProcessLine(string source, ref int position, out bool finished, _ATHLexer lexer, Colour expressionColour, ref List<Tuple<Colour, _ATHExpression>> expressions, ref Dictionary<Colour, string> lineBuffer, ref Dictionary<Colour, int> lineBufferPositions)
        {
            Colour lineColour = expressionColour;

            _ATHLexer.SkipWhitespace(source, ref position);
            if (source[position] == '#')
            {
                lineColour = ((ColourToken)ColourToken.Read(source, ref position)).Colour;
            }

            if (lineBuffer.ContainsKey(lineColour) == false)
            {
                lineBuffer[lineColour] = "";
                lineBufferPositions[lineColour] = 0;
            }

            lineBuffer[lineColour] += ((EverythingExceptNewlineToken)EverythingExceptNewlineToken.Read(source, ref position)).Content + "\n";

            while (position < source.Length && (source[position] == '\r' || source[position] == '\n'))
            {
                position++;
            }

            try
            {
                var pos = lineBufferPositions[lineColour];
                _ATHLexer.SkipWhitespace(lineBuffer[lineColour], ref pos);
                var expression = lexer.ReadExpression(lineBuffer[lineColour], ref pos, lineColour);
                expressions.Add(Tuple.Create(lineColour, expression));
                _ATHLexer.SkipWhitespace(lineBuffer[lineColour], ref pos);
                lineBufferPositions[lineColour] = pos;
            }
            catch (Exception)
            {
                // Empty.
            }

            if (expressions.Count > 0)
            {
                if (expressions[expressions.Count - 1].Item1 == expressionColour)
                {
                    if (expressions[expressions.Count - 1].Item2 is THISListDIEExpression)
                    {
                        finished = true;
                        return;
                    }
                }
            }

            finished = false;
        }
    }
}