using System.Collections.Generic;
using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    public class VarargListAssignmentAnalyzer : IAnalyzer
    {
        // Adds the vararg to a table
        public void Analyze(Function f)
        {
            for (int i = 0; i < f.Instructions.Count - 1; i++)
            {
                if (f.Instructions[i] is Assignment {IsIndeterminateVararg: true} a1 && f.Instructions[i + 1] is Assignment
                {
                    IsIndeterminateVararg: true
                } a3)
                {
                    var idRef = new IdentifierReference(a3.Left[0].Identifier);
                    idRef.TableIndices.Add(new Constant(ValueType.VarArgs, -1));
                    var assn = new Assignment(idRef, new IdentifierReference(f.SymbolTable.GetVarargs()));
                    assn.LocalAssignments = a3.LocalAssignments;
                    f.Instructions[i] = assn;
                    f.Instructions.RemoveAt(i + 1);
                }
            }
        }
    }
}