using System.Linq;
using System.Text;
using CoDLuaDecompiler.Decompiler.CFG;
using CoDLuaDecompiler.Decompiler.Extensions;
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
            if (Iterator is { } a)
            {
                a.IsLocalDeclaration = false;
                a.IsGenericForAssignment = true;
            }
            StringBuilder str = new StringBuilder($@"for {Iterator} do" + "\n");

            str.Append(Body.PrintBlock(indentLevel).AddIndent());
            
            str.Append("\n");
            for (int i = 0; i < indentLevel; i++)
                str.Append("\t");
            str.Append("end");
            if (Follow != null && Follow.Instructions.Count() > 0)
            {
                str.Append("\n" + Follow.PrintBlock(indentLevel));
            }
            return str.ToString();
        }
    }
}