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
    public class EverythingExceptNewlineToken : _ATHToken
    {
        public string Content { get; private set; }

        public EverythingExceptNewlineToken(string content)
        {
            Content = content;
        }

        public static _ATHToken Read(string source, ref int position)
        {
            var content = source.Substring(position).Split(new[] { '\r', '\n' }, 2)[0];

            position += content.Length;

            return new EverythingExceptNewlineToken(content);
        }
    }
}
