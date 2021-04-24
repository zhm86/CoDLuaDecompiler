using System.Collections.Generic;
using System.Linq;
using CoDHVKDecompiler.Decompiler.CFG;
using CoDHVKDecompiler.Decompiler.IR.Expression;
using CoDHVKDecompiler.Decompiler.IR.Functions;

namespace CoDHVKDecompiler.Decompiler.IR.Instruction
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
            string ret = "";
            if (IsElseIf)
            {
                ret = $@"elseif {Condition} then" + "\n";
            }
            else
            {
                ret = $@"if {Condition} then" + "\n";
            }
            if (TrueBody != null)
            {
                Function.IndentLevel += 1;
                ret += TrueBody.PrintBlock(indentLevel + 1);
                Function.IndentLevel -= 1;
            }
            if (FalseBody != null)
            {
                ret += "\n";
                // Check for elseif
                if (FalseBody.Instructions.Count() == 1 && FalseBody.Instructions.First() is IfStatement s && s.Follow == null)
                {
                    s.IsElseIf = true;
                    ret += FalseBody.PrintBlock(indentLevel);
                }
                else
                {
                    for (int i = 0; i < indentLevel; i++)
                    {
                        ret += "\t";
                    }
                    ret += "else\n";
                    Function.IndentLevel += 1;
                    ret += FalseBody.PrintBlock(indentLevel + 1);
                    Function.IndentLevel -= 1;
                }
            }
            if (!IsElseIf)
            {
                ret += "\n";
            }
            if (!IsElseIf)
            {
                for (int i = 0; i < indentLevel; i++)
                {
                    ret += "\t";
                }
                ret += "end";
            }
            if (Follow != null && Follow.Instructions.Any())
            {
                ret += "\n";
                ret += Follow.PrintBlock(indentLevel);
            }
            return ret;
        }
    }
}