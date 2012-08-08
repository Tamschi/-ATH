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
    public class ColourToken : _ATHToken
    {
        public Colour Colour { get; private set; }

        public ColourToken(Colour colour)
        {
            Colour = colour;
        }

        public static _ATHToken Read(string source, ref int position)
        {
            if (source[position] == '#')
            {
                position++;
                var r = byte.Parse(source.Substring(position, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                position += 2;
                var g = byte.Parse(source.Substring(position, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                position += 2;
                var b = byte.Parse(source.Substring(position, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                position += 2;

                if (source[position] == '#')
                {
                    throw new TokenException("No colour expected at " + position + ".");
                }

                return new ColourToken(new Colour( r, g, b ));
            }

            throw new TokenException("Colour expected at " + position + ".");
        }
    }
}
