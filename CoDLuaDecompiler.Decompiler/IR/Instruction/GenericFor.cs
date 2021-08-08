using System.Linq;
using CoDLuaDecompiler.Decompiler.CFG;
using CoDLuaDecompiler.Decompiler.IR.Functions;

namespace CoDLuaDecompiler.Decompiler.IR.Instruction
{
    public class GenericFor : IInstruction
    {
        public Assignment Iterator { get; set; }
        public BasicBlock Body { get; set; }
        public BasicBlock Follow { get; set; }
        
        public override string WriteLua(int indentLevel)
        {
            string ret = "";
            if (Iterator is { } a)
            {
                a.IsLocalDeclaration = false;
                a.IsGenericForAssignment = true;
            }
            ret = $@"for {Iterator} do" + "\n";

            Function.IndentLevel += 1;
            ret += Body.PrintBlock(indentLevel + 1);
            Function.IndentLevel -= 1;
            ret += "\n";
            for (int i = 0; i < indentLevel; i++)
            {
                ret += "\t";
            }
            ret += "end";
            if (Follow != null && Follow.Instructions.Count() > 0)
            {
                ret += "\n";
                ret += Follow.PrintBlock(indentLevel);
            }
            return ret;
        }
    }
}