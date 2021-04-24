using System;
using System.Linq;
using CoDHVKDecompiler.Decompiler.IR.Expression;
using CoDHVKDecompiler.Decompiler.IR.Functions;
using CoDHVKDecompiler.Decompiler.IR.Identifiers;
using CoDHVKDecompiler.Decompiler.IR.Instruction;

namespace CoDHVKDecompiler.Decompiler.Analyzers
{
    /// <summary>
    /// Resolves function calls that have 0 as its "b", which means the actual arguments are determined by a
    /// previous function with an indefinite return count being the last argument
    /// </summary>
    public class IndeterminateArgumentsAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            // This analysis should not need any intrablock analysis
            foreach (var b in f.Blocks)
            {
                Identifier lastIndeterminantRet = null;
                foreach (var i in b.Instructions)
                {
                    if (i is Assignment a2 && a2.Right is FunctionCall fc2 && fc2.IsIndeterminateArgumentCount)
                    {
                        if (lastIndeterminantRet == null)
                        {
                            Console.WriteLine($"Error: Indeterminant argument function call without preceding indeterminant arguments");
                            continue;
                        }
                        for (uint r = fc2.BeginArg; r <= lastIndeterminantRet.RegNum; r++)
                        {
                            fc2.Arguments.Add(new IdentifierReference(f.SymbolTable.GetRegister(r)));
                        }
                        lastIndeterminantRet = null;
                    }

                    else if (i is Return ret2 && ret2.Expressions.Count == 1 &&
                        ret2.Expressions[^1] is FunctionCall fc3 && fc3.IsIndeterminateArgumentCount)
                    {
                        if (lastIndeterminantRet == null)
                        {
                            Console.WriteLine("Error: Indeterminant argument function call without preceding indeterminant arguments");
                            continue;
                        }
                        for (uint r = fc3.BeginArg; r <= lastIndeterminantRet.RegNum; r++)
                        {
                            fc3.Arguments.Add(new IdentifierReference(f.SymbolTable.GetRegister(r)));
                        }
                        lastIndeterminantRet = null;
                    }
                    if (i is Return ret && ret.IsIndeterminateReturnCount)
                    {
                        if (lastIndeterminantRet == null)
                        {
                            Console.WriteLine("Error: Indeterminant return without preceding indeterminant return function call");
                            continue;
                        }
                        for (uint r = ret.BeginRet; r <= lastIndeterminantRet.RegNum; r++)
                        {
                            ret.Expressions.Add(new IdentifierReference(f.SymbolTable.GetRegister(r)));
                        }
                    }
                    if (i is Assignment a && a.Left.Count() == 1 && !a.Left[0].HasIndex && a.Right is FunctionCall fc && fc.IsIndeterminateReturnCount)
                    {
                        lastIndeterminantRet = a.Left[0].Identifier;
                    }

                    // Vararg can also set the indeterminant register number
                    if (i is Assignment a3 && a3.IsIndeterminateVararg)
                    {
                        lastIndeterminantRet = a3.Left[0].Identifier;
                    }
                }
            }
        }
    }
}