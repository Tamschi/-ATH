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
    public class MaybeColouredNameToken : _ATHToken
    {
        public Colour? Colour { get; private set; }
        public string Name { get; private set; }

        public MaybeColouredNameToken(Colour? colour, string name)
        {
            Colour = colour;
            Name = name;
        }

        public static _ATHToken Read(string source, ref int position)
        {
            Colour? colour = null;

            if (source[position] == '#')
            {
                position++;
                var r = byte.Parse(source.Substring(position, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                position += 2;
                var g = byte.Parse(source.Substring(position, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                position += 2;
                var b = byte.Parse(source.Substring(position, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                position += 2;

                colour = new Colour(r, g, b);
            }

            var name = new StringBuilder();

            while (source.Length > position && char.IsLetterOrDigit(source, position))
            {
                name.Append(source[position]);
                position++;
            }
            if (name.Length > 0)
            {
                return new MaybeColouredNameToken(colour, name.ToString());
            }
            else
            {
                throw new TokenException("Name expected at " + position + ".");
            }
        }
    }
}
