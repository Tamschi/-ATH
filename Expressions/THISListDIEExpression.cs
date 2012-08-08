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
    public class THISListDIEExpression : _ATHExpression
    {
        public static string Keyword
        {
            get { return "["; }
        }

        public string Target1 { get; private set; }
        public Colour? Target1Colour { get; private set; }

        public string Target2 { get; private set; }
        public Colour? Target2Colour { get; private set; }

        public THISListDIEExpression(
            SkipWhitespaceToken a,
            ColourToken target1Colour,
            NameToken target1,
            SkipWhitespaceToken b,
            CommaToken c,
            SkipWhitespaceToken d,
            ColourToken target2Colour,
            NameToken target2,
            SkipWhitespaceToken e,
            SquareCloseToken f,
            SkipWhitespaceToken g,
            DotToken h,
            SkipWhitespaceToken i,
            DIEToken j,
            SkipWhitespaceToken k,
            RoundOpenToken l,
            SkipWhitespaceToken m,
            RoundCloseToken n,
            SkipWhitespaceToken o,
            SemicolonToken p)
        {
            Target1 = target1.Name;
            Target1Colour = target1Colour.Colour;

            Target2 = target2.Name;
            Target2Colour = target2Colour.Colour;
        }

        public override void EmitIL(_ATHProgram program, Colour expressionColour, ILGenerator ilGenerator, Dictionary<string, ImportHandle> importHandles, Dictionary<Tuple<string, Colour>, ImportHandle> objects)
        {
            throw new _ATHParserException("THIS list DIE expression not valid at this position.");
        }
    }
}
