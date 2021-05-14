using System.Collections.Generic;
using System.Linq;
using CoDHVKDecompiler.Decompiler.IR.Expression;
using CoDHVKDecompiler.Decompiler.IR.Expression.Operator;
using CoDHVKDecompiler.Decompiler.IR.Functions;
using CoDHVKDecompiler.Decompiler.IR.Identifiers;
using CoDHVKDecompiler.Decompiler.IR.Instruction;

namespace CoDHVKDecompiler.Decompiler.Analyzers
{
    public class TernaryOperatorAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            for (int i = 0; i < f.Instructions.Count; i++)
            {
                if (i > 0 && i < f.Instructions.Count && f.Instructions[i] is Jump j && j.Conditional && f.Instructions.IndexOf(j.Dest) is var d && d > 0 && d < f.Instructions.Count && f.Instructions[d - 1] is Jump j2 && f.Instructions.IndexOf(j2.Dest) > f.Instructions.IndexOf(j2) && j2.Conditional &&
                    j2.Condition is UnaryOp uo && uo.OperationType == UnOperationType.OpNot && uo.Expression is IdentifierReference ir2 && 
                    (j.Condition is IdentifierReference iir && iir.Identifier == ir2.Identifier || j.Condition is BinOp bo && bo.Left is IdentifierReference iir2 && iir2.Identifier == ir2.Identifier))
                {
                    var ir = (IdentifierReference) (j.Condition is IdentifierReference ? j.Condition : ((BinOp)j.Condition).Left);
                    // Make sure there aren't any instructions other than assignments with 1 return value
                    int firstLabelPos = f.Instructions.IndexOf(j.Dest);
                    if (!IsBlockValid(f, i + 1, firstLabelPos - 1, ir.Identifier) ||
                        !IsBlockValid(f, firstLabelPos + 1, f.Instructions.IndexOf(j2.Dest), ir.Identifier))
                    {
                        continue;
                    }
                    // Make 3 new identifiers
                    var newCondId = f.SymbolTable.GetNewRegister();
                    var newLeftId = f.SymbolTable.GetNewRegister();
                    var newRightId = f.SymbolTable.GetNewRegister();
                    // Replace the uses
                    if (f.Instructions[i - 1] is Assignment a && a.Left.Count == 1 &&
                        a.Left[0].Identifier == ir.Identifier)
                    {
                        a.Left[0] = new IdentifierReference(newCondId);
                    }

                    if (f.Instructions[f.Instructions.IndexOf(j.Dest) - 2] is Assignment a2 && a2.Left.Count == 1 && !a2.Left[0].HasIndex && a2.Left[0].Identifier == ir.Identifier)
                        a2.Left[0] = new IdentifierReference(newLeftId);
                    else
                        ReplaceBlockUses(f, i + 1, f.Instructions.IndexOf(j.Dest) - 1,
                            ir.Identifier, newLeftId);

                    if (f.Instructions[f.Instructions.IndexOf(j2.Dest) - 1] is Assignment a3 && a3.Left.Count == 1 && !a3.Left[0].HasIndex && a3.Left[0].Identifier == ir.Identifier)
                        a3.Left[0] = new IdentifierReference(newRightId);
                    else
                        ReplaceBlockUses(f, f.Instructions.IndexOf(j.Dest) + 1, f.Instructions.IndexOf(j2.Dest),
                            ir.Identifier, newRightId);

                    var inst2 = f.Instructions[f.Instructions.IndexOf(j2.Dest) + 1];
                    ReplaceRegisterUses(ir.Identifier, newRightId, inst2.GetDefines(true), inst2);
                    
                    // Insert the new ternary operator assignment
                    f.Instructions.Insert(f.Instructions.IndexOf(j2.Dest) + 1, new Assignment(
                        ir, new BinOp(new IdentifierReference(newCondId), new BinOp(new IdentifierReference(newLeftId), new IdentifierReference(newRightId), BinOperationType.OpOr) { IgnoreParentheses = true }, BinOperationType.OpAnd)));
                    // Remove jumps and labels
                    var goBackCount = 2;
                    if (j.Dest.UsageCount == 1)
                    {
                        j.Dest.UsageCount--;
                        f.Instructions.Remove(j.Dest);
                        goBackCount++;
                    }
                    f.Instructions.Remove(j);
                    if (j2.Dest.UsageCount == 1)
                    {
                        j2.Dest.UsageCount--;
                        f.Instructions.Remove(j2.Dest);
                        goBackCount++;
                    }
                    f.Instructions.Remove(j2);
                    i -= goBackCount;
                }
            }
        }

        private bool IsBlockValid(Function f, int startIndex, int endIndex, Identifier id)
        {
            for (int k = startIndex; k < endIndex; k++)
            {
                if (!(f.Instructions[k]is Assignment))
                    return false;
                
                Assignment a = (Assignment) f.Instructions[k];
                if (a.Left.Count != 1)
                    return false;
            }

            if (!(f.Instructions[endIndex] is Assignment a2 && a2.Left[0].Identifier == id) && !(f.Instructions[startIndex] is Assignment a3 && a3.Left[0].Identifier == id))
                return false;
            return true;
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