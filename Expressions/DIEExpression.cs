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

using System.Collections.Generic;
using System.Reflection.Emit;
using System;
using _ATH.Tokens;

namespace _ATH.Expressions
{
    public class DIEExpression : _ATHExpression
    {
        public static string Keyword
        {
            get { return null; }
        }

        public string Target { get; private set; }
        public Colour? TargetColour { get; private set; }

        public DIEExpression(
            MaybeColouredNameToken target,
            SkipWhitespaceToken a,
            DotToken b,
            SkipWhitespaceToken c,
            DIEToken d,
            SkipWhitespaceToken e,
            RoundOpenToken f,
            SkipWhitespaceToken g,
            RoundCloseToken h,
            SkipWhitespaceToken i,
            SemicolonToken j)
        {
            Target = target.Name;
            TargetColour = target.Colour;
        }

        public override void EmitIL(_ATHProgram program, Colour expressionColour, ILGenerator ilGenerator, Dictionary<string, ImportHandle> importHandles, Dictionary<Tuple<string, Colour>, ImportHandle> objects)
        {
            if (Target == "THIS")
            {
                program.EmitKillTHIS(ilGenerator, TargetColour ?? expressionColour);
            }
            else
            {
                var target = Tuple.Create(Target, TargetColour ?? expressionColour);
                objects[target].EmitDie(ilGenerator, target);
            }
        }
    }
}
