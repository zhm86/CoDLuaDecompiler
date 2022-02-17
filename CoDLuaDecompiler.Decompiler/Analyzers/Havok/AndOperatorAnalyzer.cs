using System;
using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Expression.Operator;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    public class AndOperatorAnalyzer : IAnalyzer
    {
        /// <summary>
        /// Very specific analyzer to conditions that are AND'ed together and immediately returned.
        /// Example:
        /// local f19_local0 = IsCampaign()
        /// if f19_local0 then
        ///     f19_local0 = not IsCybercoreMenuDisabled( controller )
        /// end
        /// return f19_local0
        /// To:
        /// return IsCampaign() and not IsCybercoreMenuDisabled( controller )
        /// </summary>
        /// <param name="f"></param>
        public void Analyze(Function f)
        {
            foreach (var b in f.Blocks)
            {
                for (int i = 0; i < b.Instructions.Count - 1; i++)
                {
                    if (b.Instructions[i] is Assignment a && a.Left.Count == 1 && !a.Left[0].HasIndex &&
                        b.Instructions[i + 1] is IfStatement ifs && ifs.Condition != null && ifs.Condition is IdentifierReference ir && ir.Identifier == a.Left[0].Identifier &&
                        ifs.TrueBody != null && ifs.FalseBody == null && ifs.TrueBody.Instructions.Count == 1 && ifs.TrueBody.Instructions[0] is Assignment a2 &&
                        a2.Left.Count == 1 && a2.Left[0].Identifier == a.Left[0].Identifier && ifs.Follow != null && ifs.Follow.Instructions.Count == 1 && ifs.Follow.Instructions[0] is Return r &&
                        r.Expressions.Count == 1 && r.Expressions[0] is IdentifierReference ir2 && ir2.Identifier == a.Left[0].Identifier)
                    {
                        b.Instructions.Clear();
                        b.Instructions.Add(new Return(new BinOp(a.Right, a2.Right, BinOperationType.OpAnd)));
                        break;
                    }
                }
            }
        }
    }
}