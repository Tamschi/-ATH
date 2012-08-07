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
using System.IO;
using _ATH.Imports;

namespace _ATH
{
    static class _ATHMain
    {
        static void Main(string[] args)
        {
            bool run = false;

            foreach (var arg in args)
            {
                if (arg == "-r")
                {
                    run = true;
                    continue;
                }

                if (arg == "-c")
                {
                    run = false;
                    continue;
                }

                if (arg.EndsWith(".~ATH"))
                {
                    var program = new _ATHProgram(File.ReadAllText(arg));
                    program.Compile(Path.GetFileName(arg), new[] { typeof(ProcessImport) });
                    if (run)
                    {
                        program.Run();
                    }
                    else
                    {
                        program.Compile(Path.GetFileName(arg), new[] { typeof(ProcessImport) });
                        program.Save(arg + ".exe");
                    }
                    continue;
                }

                Console.WriteLine("Invalid argument: " + arg);
            }
        }
    }
}
