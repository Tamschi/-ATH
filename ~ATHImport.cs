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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _ATH
{
    public abstract class _ATHImport
    {
        //Implementations must have this property:
        //public static string Keyword { abstract get; }
        //
        // Deriving classes must implement:
        //
        // public static void EmitImport(ILGenerator ilGenerator, byte[] importColour, Tuple<string, Colour> name); // void()
        // 
        // public static void EmitIsAlive(ILGenerator ilGenerator, Tuple<string, Colour> name); // bool()
        // 
        // public static void Die(ILGenerator ilGenerator, Tuple<string, Colour> name); // void()
        // 
        // public static void Execute(ILGenerator ilGenerator, Tuple<string, Colour> name); // void()
    }
}
