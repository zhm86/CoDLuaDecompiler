using System.Collections.Generic;
using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Expression.Operator;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    public class TestsetAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            bool changes = true;
            for (int i = 0; i < f.Instructions.Count - 5; i++)
            {
                if (f.Instructions[i] is Jump j && j.IsTestSet && j.Condition is IdentifierReference ir &&
                    f.Instructions.IndexOf(j.Dest) is var d && d > 0 && d < f.Instructions.Count && f.Instructions[d - 1] is Jump j2)
                {
                    
                    var newLeftId = f.SymbolTable.GetNewRegister();
                    var newRightId = f.SymbolTable.GetNewRegister();
                    
                    if (f.Instructions[f.Instructions.IndexOf(j.Dest) - 2] is Assignment a2 && a2.Left.Count == 1 && !a2.Left[0].HasIndex && a2.Left[0].Identifier == j.TestsetLocation)
                        a2.Left[0] = new IdentifierReference(newLeftId);
                    else
                        ReplaceBlockUses(f, i + 1, f.Instructions.IndexOf(j.Dest) - 1,
                            j.TestsetLocation, newLeftId);
                    
                    if (f.Instructions[f.Instructions.IndexOf(j2.Dest) - 1] is Assignment a3 && a3.Left.Count == 1 && !a3.Left[0].HasIndex && a3.Left[0].Identifier == j.TestsetLocation)
                        a3.Left[0] = new IdentifierReference(newRightId);
                    else
                        ReplaceBlockUses(f, f.Instructions.IndexOf(j.Dest) + 1, f.Instructions.IndexOf(j2.Dest),
                            j.TestsetLocation, newRightId);
                    // Sometimes its an empty block
                    if (f.Instructions.IndexOf(j2.Dest) - f.Instructions.IndexOf(j2) == 1)
                    {
                        f.Instructions.Insert(f.Instructions.IndexOf(j2.Dest) - 1, new Assignment(newRightId, new IdentifierReference(j.TestsetLocation)));
                    }
                    
                    f.Instructions.Insert(f.Instructions.IndexOf(j2.Dest), new Assignment(new IdentifierReference(j.TestsetLocation), 
                        new BinOp(new IdentifierReference(newLeftId), new IdentifierReference(newRightId), j.TestsetType)));
                    f.Instructions.RemoveAt(i);
                    f.Instructions.Remove(j.Dest);
                    f.Instructions.Remove(j2);
                    if (j2.Dest.UsageCount > 1)
                        j2.Dest.UsageCount--;
                    else
                        f.Instructions.Remove(j2.Dest);
                }
            }
        }
        
        private void ReplaceBlockUses(Function f, int startIndex, int endIndex, Identifier original, Identifier newId)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                var instr = f.Instructions[i];
                ReplaceRegisterUses(original, newId, instr.GetUses(true), instr);
                ReplaceRegisterUses(original, newId, instr.GetDefines(true), instr);
            }
        }

        private void ReplaceRegisterUses(Identifier original, Identifier newId, HashSet<Identifier> ids, IInstruction instr)
        {
            foreach (var uses in ids)
            {
                if (uses == original)
                {
                    instr.ReplaceUses(uses, new IdentifierReference(newId));
                }
            }
        }
    }
}