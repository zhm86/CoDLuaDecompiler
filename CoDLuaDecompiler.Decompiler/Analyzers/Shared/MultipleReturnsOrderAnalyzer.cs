using System.Collections.Generic;
using System.ComponentModel;
using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Shared
{
    public class MultipleReturnsOrderAnalyzer : IAnalyzer
    {
        // Propagates assignments of functioncalls with multiple returns that have a different order
        // local varA, varB = someFunction()
        // anothervarA = varB
        // anothervarB = varA
        //   to
        // anothervarB, anothervarA = someFunction()
        public void Analyze(Function f)
        {
            foreach (var b in f.Blocks)
            {
                for (int i = 0; i < b.Instructions.Count - 2; i++)
                {
                    if (b.Instructions[i] is Assignment {Right: FunctionCall} a && a.Left.Count > 1 && b.Instructions.Count > i + a.Left.Count)
                    {
                        bool valid = true;
                        List<Identifier> usedIdentifiers = new List<Identifier>();
                        for (int j = i + 1; j < i + 1 + a.Left.Count; j++)
                        {
                            if (b.Instructions[j] is Assignment a2 && a2.Left.Count == 1 && a2.Right is IdentifierReference rRef &&
                                a.Left.Find(idRef => idRef.Identifier == rRef.Identifier) != null &&
                                !usedIdentifiers.Contains(rRef.Identifier))
                            {
                                usedIdentifiers.Add(rRef.Identifier);
                            }
                            else
                            {
                                valid = false;
                                break;
                            }
                        }
                        // Check if it's right
                        if (!valid || usedIdentifiers.Count != a.Left.Count)
                            continue;

                        
                        for (int j = i + 1; j < i + 1 + a.Left.Count; j++)
                        {
                            var assign = (Assignment) b.Instructions[j];
                            var idRef = assign.Left[0];
                            var origIndex = a.Left.IndexOf(a.Left.Find(l =>
                                l.Identifier == ((IdentifierReference) assign.Right).Identifier));
                            a.Left[origIndex] = idRef;
                        }
                        b.Instructions.RemoveRange(i + 1, a.Left.Count);
                    }
                }
            }
        }
    }
}