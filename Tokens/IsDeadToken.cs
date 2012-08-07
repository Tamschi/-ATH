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
    public class IsDeadToken : _ATHToken
    {
        public bool Invert { get; private set; }
        public string Name { get; private set; }

        public IsDeadToken(bool invert, string name)
        {
            Invert = invert;
            Name = name;
        }

        public static _ATHToken Read(string source, ref int position)
        {
            if (position >= source.Length)
            {
                throw new TokenException("Name or ! expected at " + position + ".");
            }

            bool invert = false;

            if (source[position] == '!')
            {
                invert = true;
                position++;
            }

            var first = true;
            var name = new StringBuilder();

            while (source.Length > position && (char.IsLetter(source, position) || ((first == false) && char.IsLetterOrDigit(source, position))))
            {
                name.Append(source[position]);
                position++;
            }
            if (name.Length > 0)
            {
                return new IsDeadToken(invert, name.ToString());
            }
            else
            {
                throw new TokenException("Name or ! expected at " + position + ".");
            }
        }
    }
}
