using System.Collections.Generic;
using System.Linq;
using CoDLuaDecompiler.Decompiler.CFG;
using CoDLuaDecompiler.Decompiler.Extensions;
using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Functions;

namespace CoDLuaDecompiler.Decompiler.IR.Instruction
{
    public class NumericFor : IInstruction
    {
        public Assignment Initial { get; set; }
        public IExpression Limit { get; set; }
        public IExpression Increment { get; set; }

        public BasicBlock Body { get; set; }
        public BasicBlock Follow { get; set; }

        public override List<IExpression> GetExpressions()
        {
            return new List<IExpression>() {Limit, Increment};
        }

        public override string WriteLua(int indentLevel)
        {
            if (Initial is { } a)
            {
                a.IsLocalDeclaration = false;
            }
            var ret = $@"for {Initial}, {Limit}, {Increment} do" + "\n";

            ret += Body.PrintBlock(indentLevel).AddIndent();
            ret += "\n";
            for (int i = 0; i < indentLevel; i++)
                ret += "\t";
            ret += "end";
            if (Follow != null && Follow.Instructions.Any())
            {
                ret += "\n";
                ret += Follow.PrintBlock(indentLevel);
            }
            return ret;
        }
    }
}