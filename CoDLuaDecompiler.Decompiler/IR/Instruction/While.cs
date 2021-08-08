using System.Collections.Generic;
using System.Linq;
using CoDLuaDecompiler.Decompiler.CFG;
using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Functions;

namespace CoDLuaDecompiler.Decompiler.IR.Instruction
{
    public class While : IInstruction
    {
        public IExpression Condition { get; set; }
        public BasicBlock Body { get; set; }
        public BasicBlock Follow { get; set; }
        public bool IsPostTested { get; set; } = false;
        public bool IsBlockInlined { get; set; } = false;

        public override List<IExpression> GetExpressions()
        {
            return new List<IExpression>() {Condition};
        }

        public override string WriteLua(int indentLevel)
        {
            string ret;
            if (IsPostTested)
            {
                ret = $@"repeat" + "\n";
            }
            else
            {
                ret = $@"while {Condition} do" + "\n";
            }
            
            Function.IndentLevel += 1;
            ret += Body.PrintBlock(indentLevel + 1, IsBlockInlined);
            Function.IndentLevel -= 1;
            ret += "\n";
            for (int i = 0; i < indentLevel; i++)
            {
                ret += "\t";
            }
            if (IsPostTested)
            {
                ret += $@"until {Condition}";
            }
            else
            {
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