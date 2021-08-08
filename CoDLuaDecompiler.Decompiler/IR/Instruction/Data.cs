
namespace CoDLuaDecompiler.Decompiler.IR.Instruction
{
    public class Data : IInstruction
    {
        public LuaFile.Structures.LuaFunction.Structures.Instruction Instruction { get; set; }

        public Data(LuaFile.Structures.LuaFunction.Structures.Instruction instruction)
        {
            Instruction = instruction;
        }

        public override string ToString()
        {
            return $"-- Data: {Instruction}";
        }
    }
}