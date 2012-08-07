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
using System.Reflection.Emit;
using _ATH.Tokens;
using System.Reflection;

namespace _ATH.Expressions
{
    public class BifurcateExpression : _ATHExpression
    {
        public static string Keyword
        {
            get { return "bifurcate"; }
        }


        public string Target { get; private set; }

        private Colour[] _colours;

        public Colour[] Colours
        {
            get
            {
                return _colours;
            }
        }

        public Tuple<Colour, _ATHExpression>[] ColouredExpressions { get; private set; }

        public BifurcateExpression(
            SkipWhitespaceToken a,
            NameToken target,
            SkipWhitespaceToken b,
            SquareOpenToken c,
            SkipWhitespaceToken d,
            ColourToken splitColour1,
            NameToken splitTarget1,
            SkipWhitespaceToken e,
            CommaToken f,
            SkipWhitespaceToken g,
            ColourToken splitColour2,
            NameToken splitTarget2,
            SkipWhitespaceToken h,
            SquareCloseToken i,
            SkipWhitespaceToken j,
            SemicolonToken k,
            SkipWhitespaceToken l,
            ColouredExpressionsEndingWithTHISListDIEToken colouredLines)
        {
            Target = target.Name;

            if (splitTarget1.Name != Target || splitTarget2.Name != Target)
            {
                throw new _ATHParserException("Split target name mismatch.");
            }

            _colours = new Colour[2];
            _colours[0] = splitColour1.Colour;
            _colours[1] = splitColour2.Colour;

            List<string>[] lines = new List<string>[_colours.Length];
            for (int ili = 0; ili < lines.Length; ili++)
            {
                lines[ili] = new List<string>();
            }

            ColouredExpressions = colouredLines.ColouredExpressions;
        }

        public override void EmitIL(_ATHProgram program, Colour expressionColour, ILGenerator ilGenerator, Dictionary<string, ImportHandle> importHandles, Dictionary<Tuple<string, Colour>, ImportHandle> objects)
        {
            EnsureTHISListDIEMatch(expressionColour, ColouredExpressions[ColouredExpressions.Length - 1]);

            MethodBuilder[] methodBuilders;

            {
                List<_ATHExpression>[] sortedExpressions = new List<_ATHExpression>[Colours.Length];
                for (int i = 0; i < sortedExpressions.Length; i++)
                {
                    sortedExpressions[i] = new List<_ATHExpression>();
                }

                methodBuilders = new MethodBuilder[Colours.Length];
                ILGenerator[] methodILGenerators = new ILGenerator[Colours.Length];
                for (int i = 0; i < methodBuilders.Length; i++)
                {
                    methodBuilders[i] = program.CreateTHISMethod(Colours[i]);
                    methodILGenerators[i] = methodBuilders[i].GetILGenerator();
                }

                for (int ice = 0; ice < ColouredExpressions.Length - 1; ice++)
                {
                    var colour = ColouredExpressions[ice].Item1;
                    var expression = ColouredExpressions[ice].Item2;

                    if (expression is CommentExpression)
                    {
                        //Optimization
                        continue;
                    }

                    var matched = false;
                    for (int ic = 0; ic < Colours.Length; ic++)
                    {
                        if (Colours[ic] == colour || expressionColour == colour)
                        {
                            program.EmitDieIfKilled(methodILGenerators[ic], Colours[ic]);

                            expression.EmitIL(program, colour, methodILGenerators[ic], importHandles, objects);
                            matched = true;
                        }
                    }

                    if (matched == false)
                    {
                        throw new _ATHParserException("Unknown expression colour #" + colour.HexString + ".");
                    }
                }

                for (int ic = 0; ic < Colours.Length; ic++)
                {
                    program.EmitDieIfKilled(methodILGenerators[ic], Colours[ic]);
                    program.EmitKillTHIS(methodILGenerators[ic], Colours[ic]);
                    program.EmitDieIfKilled(methodILGenerators[ic], Colours[ic]);
                    methodILGenerators[ic].ThrowException(typeof(InvalidProgramException));
                }
            }

            program.EmitTHISFork(ilGenerator, Colours, methodBuilders);

            program.EmitTHISJoin(ilGenerator, expressionColour, Colours);
        }

        private void EnsureTHISListDIEMatch(Colour expressionColour, Tuple<Colour, _ATHExpression> lastColouredExpression)
        {
            var exp = lastColouredExpression.Item2 as THISListDIEExpression;

            if (exp == null)
            {
                throw new _ATHParserException("Last expression isn't a THISListDIEExpression.");
            }

            if (expressionColour != lastColouredExpression.Item1)
            {
                throw new _ATHParserException("THIS list colour mismatch.");
            }

            if ((Target != exp.Target1) || (Target != exp.Target2))
            {
                throw new _ATHParserException("THIS list target mismatch.");
            }

            if (Colours[0] == exp.Target1Colour && Colours[1] == exp.Target2Colour)
            {
                return;
            }

            if (Colours[1] == exp.Target1Colour && Colours[0] == exp.Target2Colour)
            {
                return;
            }

            throw new _ATHParserException("THIS list target colour mismatch.");
        }
    }
}
