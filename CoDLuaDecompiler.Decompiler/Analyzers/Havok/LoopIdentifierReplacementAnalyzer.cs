using System.Collections.Generic;
using System.Linq;
using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    public class LoopIdentifierReplacementAnalyzer : IAnalyzer
    {
        // We need to replace the identifiers used by loops because the ssa doesn't notice it's used in different scope
        public void Analyze(Function f)
        {
            for (int i = 1; i < f.Blocks.Count; i++)
            {
                var b = f.Blocks[i];
                
                if (!b.IsLoopHead)
                    continue;

                

                if (b.Instructions.Any() && b.Instructions[0] is Assignment a && a.Left.Count == 2 && a.Right is FunctionCall fc && fc.Arguments.Count == 2)
                {
                    if (fc.Function is IdentifierReference ir2 && ir2.Identifier.DefiningInstruction is Assignment a2 && a2.Left.Count == 3 && b.LoopLatches.Any() &&
                        b.LoopLatches[0].Instructions.Any() && b.LoopLatches[0].Instructions[0] is Assignment a3 && a3.Left.Count == 1 && a3.Right is IdentifierReference ir && ir.Identifier == a.Left[0].Identifier)
                    {
                        var newFuncId = f.SymbolTable.GetNewRegister();
                        var newIterId = f.SymbolTable.GetNewRegister();
                        var newIndexId = f.SymbolTable.GetNewRegister();
                        var oldFuncId = ((IdentifierReference)fc.Function).Identifier;
                        var oldIterId = ((IdentifierReference)fc.Arguments[0]).Identifier;
                        var oldIndexId = ((IdentifierReference)fc.Arguments[1]).Identifier;
                        ((IdentifierReference) fc.Function).Identifier = newFuncId;
                        ((IdentifierReference) fc.Arguments[0]).Identifier = newIterId;
                        ((IdentifierReference) fc.Arguments[1]).Identifier = newIndexId;
                        a2.Left[0].Identifier = newFuncId;
                        a2.Left[1].Identifier = newIterId;
                        a2.Left[2].Identifier = newIndexId;
                        a3.Left[0].Identifier = newIndexId;
                    }
                    
                    var newKeyId = f.SymbolTable.GetNewRegister();
                    var newValueId = f.SymbolTable.GetNewRegister();
                    var oldKeyId = a.Left[0].Identifier;
                    var oldValueId = a.Left[1].Identifier;

                    for (int j = i + 1; j < f.Blocks.Count; j++)
                    {
                        var b2 = f.Blocks[j];
                        if (b2 == b.LoopFollow)
                            break;

                        for (int k = 0; k < b2.Instructions.Count; k++)
                        {
                            var instr = b2.Instructions[k];
                            ReplaceRegisterUses(oldKeyId, newKeyId, instr.GetUses(true), instr);
                            ReplaceRegisterUses(oldKeyId, newKeyId, instr.GetDefines(true), instr);
                            ReplaceRegisterUses(oldValueId, newValueId, instr.GetUses(true), instr);
                            ReplaceRegisterUses(oldValueId, newValueId, instr.GetDefines(true), instr);
                        }
                    }
                    
                    a.Left[0].Identifier = newKeyId;
                    a.Left[1].Identifier = newValueId;
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