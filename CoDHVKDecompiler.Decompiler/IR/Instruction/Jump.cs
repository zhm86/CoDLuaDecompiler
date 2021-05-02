using System.Collections.Generic;
using CoDHVKDecompiler.Decompiler.CFG;
using CoDHVKDecompiler.Decompiler.IR.Expression;
using CoDHVKDecompiler.Decompiler.IR.Expression.Operator;
using CoDHVKDecompiler.Decompiler.IR.Identifiers;

namespace CoDHVKDecompiler.Decompiler.IR.Instruction
{
    public class Jump : IInstruction
    {
        public Label Dest { get; set; }
        public bool Conditional { get; set; }
        public IExpression Condition { get; set; }
        public BasicBlock BlockDest { get; set; }
        
        // Lua 5.1 and HKS has a post-jump assignment that needs to be put at the top of the successor block
        public Assignment PostTakenAssignment { get; set; }
        
        public Jump(Label dest)
        {
            Dest = dest;
        }

        public Jump(Label dest, IExpression cond)
        {
            Dest = dest;
            Conditional = true;
            Condition = cond;
            if (Condition is BinOp op)
            {
                op.NegateCondition();
            }
        }
        
        public override void Parenthesize()
        {
            if (Conditional)
            {
                Condition.Parenthesize();
            }
        }
        
        public override HashSet<Identifier> GetUses(bool regOnly)
        {
            if (Conditional)
            {
                return Condition.GetUses(regOnly);
            }
            return base.GetUses(regOnly);
        }
        
        public override void RenameUses(Identifier orig, Identifier newId)
        {
            if (Conditional)
            {
                Condition.RenameUses(orig, newId);
            }
        }
        
        public override bool ReplaceUses(Identifier orig, IExpression sub)
        {
            if (Conditional)
            {
                if (IExpression.ShouldReplace(orig, Condition))
                {
                    Condition = sub;
                    return true;
                }
                else
                {
                    return Condition.ReplaceUses(orig, sub);
                }
            }
            return false;
        }
        
        public override List<IExpression> GetExpressions()
        {
            if (Conditional)
                return Condition.GetExpressions();
            return new List<IExpression>();
        }
        
        public override string ToString()
        {
            string ret = "";
            if (Conditional)
            {
                ret += $@"if {Condition} else ";
            }
            if (BlockDest != null)
            {
                ret += "goto " + BlockDest;
            }
            else
            {
                ret += "goto " + Dest;
            }
            return ret;
        }
    }
}