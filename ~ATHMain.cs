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
using System.IO;
using System.Linq;
using System.Reflection;
using _ATH.Imports;

namespace _ATH
{
    static class _ATHMain
    {
        static void Main(string[] args)
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var importPath = "./Imports/";
            importPath = Path.GetFullPath(importPath);

            if (!Directory.Exists(importPath))
            {
                Directory.CreateDirectory(importPath);
            }

            var importFiles = Directory.GetFiles(importPath, "*.dll", SearchOption.AllDirectories);
            var externalImports = importFiles.SelectMany(file => Assembly.LoadFrom(file).GetTypes().Where(type => typeof(_ATHImport).IsAssignableFrom(type))).ToArray();

            var internalImports = new[] { typeof(ProcessImport) };

            var imports = externalImports.Concat(internalImports).ToArray();

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
                    try
                    {
                        var program = new _ATHProgram(File.ReadAllText(arg));
                        program.Compile(Path.GetFileName(arg), imports);
                        if (run)
                        {
                            program.Run();
                        }
                        else
                        {
                            program.Save(arg + ".exe");
                        }
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException != null)
                        {
                            Console.WriteLine(ex);
                            Console.WriteLine();
                            ex = ex.InnerException;
                        }
                        Console.WriteLine(ex.ToString());
                        Console.ReadLine();

                    }
                    break;
                }

                Console.WriteLine("Invalid argument: " + arg);
                Console.ReadLine();
                break;
            }
        }
    }
}
