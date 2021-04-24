using System.Collections.Generic;
using CoDHVKDecompiler.Decompiler.IR.Expression;
using CoDHVKDecompiler.Decompiler.IR.Functions;
using CoDHVKDecompiler.Decompiler.IR.Identifiers;
using CoDHVKDecompiler.Decompiler.IR.Instruction;

namespace CoDHVKDecompiler.Decompiler.Analyzers
{
    public class VarargListAssignmentAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            // Pretty hacky way to do this, don't know another way to add it if the table already has items
            for (int i = 0; i < f.Instructions.Count - 1; i++)
            {
                if (f.Instructions[i] is Assignment {IsIndeterminateVararg: true} && f.Instructions[i + 1] is Assignment
                {
                    IsIndeterminateVararg: true
                } a3)
                {
                    f.Instructions[i] = new Assignment(new List<IdentifierReference>(), new FunctionCall(new IdentifierReference(new Identifier() {Name = "table.insert", IdentifierType = IdentifierType.Global}), new List<IExpression>(){new IdentifierReference(f.SymbolTable.GetRegister(a3.VarargAssignmentReg)), new IdentifierReference(f.SymbolTable.GetVarargs())}));
                    f.Instructions.RemoveAt(i + 1);
                }
            }
        }
    }
}