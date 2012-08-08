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
    public class ImportExpression : _ATHExpression
    {
        public static string Keyword
        {
            get { return "import"; }
        }

        public string Type { get; private set; }
        public Colour? TypeColour { get; private set; }
        public string Name { get; private set; }
        public Colour? NameColour { get; private set; }

        public ImportExpression(
            SkipWhitespaceToken a,
            MaybeColouredNameToken type,
            SkipWhitespaceToken b,
            MaybeColouredNameToken name,
            SkipWhitespaceToken c,
            SemicolonToken d)
        {
            Type = type.Name;
            TypeColour = type.Colour;
            Name = name.Name;
            NameColour = name.Colour;
        }

        public override void EmitIL(_ATHProgram program, Colour expressionColour, ILGenerator ilGenerator, Dictionary<string, ImportHandle> importHandles, Dictionary<Tuple<string, Colour>, ImportHandle> objects)
        {
            var importHandle = importHandles[Type];
            var colouredName = Tuple.Create(Name, NameColour ?? expressionColour);

            if (objects.ContainsKey(colouredName))
            {
                if (objects[colouredName] != importHandle)
                {
                    throw new _ATHParserException("Tried to use same object name and colour with different import types.");
                }
            }
            else
            {
                objects[colouredName] = importHandle;
            }

            importHandle.EmitImport(program, ilGenerator, TypeColour ?? expressionColour, colouredName);
        }
    }
}
