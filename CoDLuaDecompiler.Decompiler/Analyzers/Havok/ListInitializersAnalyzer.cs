using System;
using System.Linq;
using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    /// <summary>
    /// Detects list initializers as a series of statements that serially add data to a newly initialized list
    /// </summary>
    public class ListInitializersAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            bool changed = true;
            while (changed)
            {
                changed = false;
                foreach (var b in f.Blocks)
                {
                    for (int i = 0; i < b.Instructions.Count(); i++)
                    {
                        if (b.Instructions[i] is Assignment a && a.Left.Count() == 1 && !a.Left[0].HasIndex && a.Right is InitializerList il /*&& il.Exprs.Count() == 0*/)
                        {
                            // Eat up any statements that follow that match the initializer list pattern
                            int initIndex = il.Expressions.Count() + 1;
                            while (i + 1 < b.Instructions.Count())
                            {
                                if (b.Instructions[i + 1] is Assignment a2 && a2.Left.Count() == 1 && a2.Left[0].Identifier == a.Left[0].Identifier && a2.Left[0].HasIndex && a2.Left[0].TableIndices.Count == 1)
                                {
                                    if (a2.Left[0].TableIndices[0] is Constant c && Math.Abs(c.Number - initIndex) < 0.01)
                                    {
                                        il.Expressions.Add(a2.Right);
                                    }
                                    else
                                    {
                                        il.Expressions.Add(new ListAssignment(a2.Left[0].TableIndices[0], a2.Right));
                                    }
                                    if (a2.LocalAssignments != null)
                                    {
                                        a.LocalAssignments.AddRange(a2.LocalAssignments);
                                    }
                                    a2.Left[0].Identifier.UseCount--;
                                    b.Instructions.RemoveAt(i + 1);
                                    initIndex++;
                                    changed = true;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    // Do this after every pass so we are sure tables in tables are handled aswell
                    new ExpressionPropagationAnalyzer().Analyze(f);
                }
            }
        }
    }
}