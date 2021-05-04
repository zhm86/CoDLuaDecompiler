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
                if (i > 0 && i < f.Instructions.Count && f.Instructions[i] is Jump j && j.Conditional && f.Instructions.IndexOf(j.Dest) is int d && d > 0 && d < f.Instructions.Count && f.Instructions[d - 1] is Jump j2 && j2.Conditional &&
                    j2.Condition is UnaryOp uo && uo.OperationType == UnOperationType.OpNot && j.Condition is IdentifierReference ir && uo.Expression is IdentifierReference ir2 && 
                    ir.Identifier == ir2.Identifier)
                {
                    // Make sure there aren't any instructions other than assignments with 1 return value
                    bool valid = true;
                    for (int k = i + 1; k < f.Instructions.IndexOf(j.Dest) - 1; k++)
                    {
                        if (!(f.Instructions[k] is Assignment ass && ass.Left.Count == 1))
                        {
                            valid = false;
                            break;
                        }
                    }
                    if (!valid)
                        continue;
                    for (int k = f.Instructions.IndexOf(j.Dest) + 1; k < f.Instructions.IndexOf(j2.Dest); k++)
                    {
                        if (!(f.Instructions[k] is Assignment ass && ass.Left.Count == 1))
                        {
                            valid = false;
                            break;
                        }
                    }
                    if (!valid)
                        continue;
                    // Get the highest regs
                    var maxRegNum = f.SymbolTable.ScopeStack.Peek()
                        .Select(st => st.Key.StartsWith("REG") ? int.Parse(st.Key.Substring(3)) : -1)
                        .Where(reg => reg != -1)
                        .Max();
                    // Make 3 new identifier references
                    var newCond = new Identifier()
                    {
                        IdentifierType = IdentifierType.Register,
                        ValueType = ValueType.Unknown,
                        Name = $@"REG{maxRegNum + 1}",
                        RegNum = (uint) (maxRegNum + 1)
                    };
                    var newLeftIr = new Identifier()
                    {
                        IdentifierType = IdentifierType.Register,
                        ValueType = ValueType.Unknown,
                        Name = $@"REG{maxRegNum + 2}",
                        RegNum = (uint) (maxRegNum + 2)
                    };
                    var newRightIr = new Identifier()
                    {
                        IdentifierType = IdentifierType.Register,
                        ValueType = ValueType.Unknown,
                        Name = $@"REG{maxRegNum + 3}",
                        RegNum = (uint) (maxRegNum + 3)
                    };
                    f.SymbolTable.ScopeStack.Peek().Add(newCond.Name, newCond);
                    f.SymbolTable.ScopeStack.Peek().Add(newLeftIr.Name, newLeftIr);
                    f.SymbolTable.ScopeStack.Peek().Add(newRightIr.Name, newRightIr);
                    // Replace the uses
                    if (i > 0 && f.Instructions[i - 1] is Assignment a && a.Left.Count == 1 &&
                        a.Left[0].Identifier == ir.Identifier)
                    {
                        a.Left[0] = new IdentifierReference(newCond);
                    }

                    if (f.Instructions[f.Instructions.IndexOf(j.Dest) - 2] is Assignment a2 && a2.Left.Count == 1 && !a2.Left[0].HasIndex && a2.Left[0].Identifier == ir.Identifier)
                    {
                        a2.Left[0] = new IdentifierReference(newLeftIr);
                    }
                    else
                    {
                        for (int k = i + 1; k < f.Instructions.IndexOf(j.Dest) - 1; k++)
                        {
                            var instr = f.Instructions[k];
                            ReplaceRegisterUses(ir.Identifier, newLeftIr, instr.GetUses(true), instr);
                            ReplaceRegisterUses(ir.Identifier, newLeftIr, instr.GetDefines(true), instr);
                        }
                    }

                    if (f.Instructions[f.Instructions.IndexOf(j2.Dest) - 1] is Assignment a3 && a3.Left.Count == 1 && !a3.Left[0].HasIndex && a3.Left[0].Identifier == ir.Identifier)
                    {
                        a3.Left[0] = new IdentifierReference(newRightIr);
                    }
                    else
                    {
                        for (int k = f.Instructions.IndexOf(j.Dest) + 1; k < f.Instructions.IndexOf(j2.Dest); k++)
                        {
                            var instr = f.Instructions[k];
                            ReplaceRegisterUses(ir.Identifier, newRightIr, instr.GetUses(true), instr);
                            ReplaceRegisterUses(ir.Identifier, newRightIr, instr.GetDefines(true), instr);
                        }
                    }

                    var inst2 = f.Instructions[f.Instructions.IndexOf(j2.Dest) + 1];
                    ReplaceRegisterUses(ir.Identifier, newRightIr, inst2.GetDefines(true), inst2);
                    
                    // Insert the new ternary operator assignment
                    f.Instructions.Insert(f.Instructions.IndexOf(j2.Dest) + 1, new Assignment(
                        ir, new BinOp(new IdentifierReference(newCond), new BinOp(new IdentifierReference(newLeftIr), new IdentifierReference(newRightIr), BinOperationType.OpOr) { IgnoreParentheses = true }, BinOperationType.OpAnd)));
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