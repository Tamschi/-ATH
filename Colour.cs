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
using System.Text;

namespace _ATH
{
    public struct Colour : IEquatable<Colour>
    {
        private byte[] _colour;

        public int Length { get { return _colour.Length; } }

        public byte this[int index]
        {
            get
            {
                if (_colour == null) return 0;
                return _colour[index];
            }
        }

        public string HexString
        {
            get
            {
                if (_colour == null)
                {
                    return "000000";
                }

                var hexStringBuilder = new StringBuilder();
                for (int i = 0; i < _colour.Length; i++)
                {
                    hexStringBuilder.AppendFormat("{0:x2}", _colour[i]);
                }
                return hexStringBuilder.ToString();
            }
        }

        public Colour(byte r, byte g, byte b)
        {
            _colour = new[] { r, g, b };
        }

        public bool Equals(Colour other)
        {
            if (_colour == null) _colour = new byte[3];
            if (other._colour == null) other._colour = new byte[3];

            return Utils.CompareArray(_colour, other._colour);
        }

        public override bool Equals(object obj)
        {
            if (obj is Colour)
            {
                return Equals((Colour)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (_colour == null) _colour = new byte[3];

            var result = 0;
            for (int i = 0; i < _colour.Length; i++)
            {
                result <<= 8;
                result |= _colour[i];
            }
            return result;
        }

        public static bool operator ==(Colour a, Colour b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Colour a, Colour b)
        {
            return !(a == b);
        }
    }
}
