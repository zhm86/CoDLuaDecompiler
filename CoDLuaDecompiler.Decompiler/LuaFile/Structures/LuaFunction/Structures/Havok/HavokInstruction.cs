using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaOpCodeTable;

namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Structures.Havok
{
    public class HavokInstruction : Instruction
    {
        public LuaHavokOpCode HavokOpCode { get; set; }
        public uint C { get; set; }
        public bool ExtraCBit { get; set; } = false;
        public uint Bx { get; set; }
        public int SBx { get; set; }

        public override string ToString()
        {
            return $"OpCode: {HavokOpCode}, A: {A}, B: {B}, C: {C}, ExtraCBit: {ExtraCBit}, Bx: {Bx}: SBx: {SBx}";
        }
    }
}