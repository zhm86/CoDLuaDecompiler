using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoDLuaDecompiler.Decompiler.CFG;
using CoDLuaDecompiler.Decompiler.Extensions;
using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Functions;

namespace CoDLuaDecompiler.Decompiler.IR.Instruction
{
    public class IfStatement : IInstruction
    {
        public IExpression Condition { get; set; }
        public BasicBlock TrueBody { get; set; }
        public BasicBlock FalseBody { get; set; }
        public BasicBlock Follow { get; set; }
        public bool IsElseIf { get; set; } = false;

        public override List<IExpression> GetExpressions()
        {
            return new List<IExpression>() {Condition};
        }

        public override string WriteLua(int indentLevel)
        {
            StringBuilder str = new StringBuilder();
            if (IsElseIf)
                str.Append($"elseif {Condition} then\n");
            else
                str.Append($"if {Condition} then\n");
            if (TrueBody != null)
            {
                str.Append(TrueBody.PrintBlock(indentLevel).AddIndent());
            }
            if (FalseBody != null)
            {
                str.Append("\n");
                // Check for elseif
                if (FalseBody.Instructions.Count() == 1 && FalseBody.Instructions.First() is IfStatement s && s.Follow == null)
                {
                    s.IsElseIf = true;
                    str.Append(FalseBody.PrintBlock(indentLevel));
                }
                else
                {
                    for (int i = 0; i < indentLevel; i++)
                        str.Append("\t");
                    str.Append("else\n");
                    str.Append(FalseBody.PrintBlock(indentLevel).AddIndent());
                }
            }
            if (!IsElseIf)
            {
                str.Append("\n");
            }
            if (!IsElseIf)
            {
                for (int i = 0; i < indentLevel; i++)
                {
                    str.Append("\t");
                }
                str.Append("end");
            }
            if (Follow != null && Follow.Instructions.Any())
            {
                str.Append("\n");
                str.Append(Follow.PrintBlock(indentLevel));
            }
            return str.ToString();
        }
    }
}