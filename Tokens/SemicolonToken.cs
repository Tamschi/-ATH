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


namespace _ATH.Tokens
{
    public class SemicolonToken : _ATHToken
    {
        public static _ATHToken Read(string source, ref int position)
        {
            if (source.Length > position && source[position] == ';')
            {
                position++;
                return new SemicolonToken();
            }
            else
            {
                throw new TokenException("; expected at " + position + ".");
            }
        }
    }
}
