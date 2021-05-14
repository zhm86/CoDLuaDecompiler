
namespace CoDHVKDecompiler.Decompiler.IR.Instruction
{
    public class Data : IInstruction
    {
        public Common.LuaFunction.Structures.Instruction Instruction { get; set; }

        public Data(Common.LuaFunction.Structures.Instruction instruction)
        {
            Instruction = instruction;
        }

        public override string ToString()
        {
            return $"-- Data: {Instruction}";
        }
    }
}