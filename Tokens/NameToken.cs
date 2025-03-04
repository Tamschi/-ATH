﻿/*
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
    public class NameToken : _ATHToken
    {
        public string Name { get; private set; }

        public NameToken(string name)
        {
            Name = name;
        }

        public static _ATHToken Read(string source, ref int position)
        {
            var name = new StringBuilder();

            while (source.Length > position && char.IsLetterOrDigit(source, position))
            {
                name.Append(source[position]);
                position++;
            }
            if (name.Length > 0)
            {
                return new NameToken(name.ToString());
            }
            else
            {
                throw new TokenException("Name expected at " + position + ".");
            }
        }
    }
}
