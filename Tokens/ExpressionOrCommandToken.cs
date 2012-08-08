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

using System.Text;

namespace _ATH.Tokens
{
    public class ExpressionOrCommandToken : _ATHToken
    {
        public _ATHExpression Expression { get; private set; }
        public string Command { get; private set; }

        public ExpressionOrCommandToken(_ATHExpression expression)
        {
            Expression = expression;
        }

        public ExpressionOrCommandToken(string command)
        {
            Command = command;
        }

        public static _ATHToken Read(string source, ref int position, _ATHLexer lexer, Colour expressionColour)
        {
            try
            {
                var expression = lexer.ReadExpression(source, ref position, expressionColour, typeof(SemicolonToken));
                return new ExpressionOrCommandToken(expression);
            }
            catch
            {
                StringBuilder commandBuilder = new StringBuilder();
                int bracketDepth = 0;
                while (bracketDepth != 0 || source[position] != ')')
                {
                    if (source[position] == '(')
                    {
                        bracketDepth++;
                    }
                    else if (source[position] == ')')
                    {
                        bracketDepth--;
                    }
                    commandBuilder.Append(source[position]);
                    position++;
                }
                return new ExpressionOrCommandToken(commandBuilder.ToString());
            }
        }
    }
}
