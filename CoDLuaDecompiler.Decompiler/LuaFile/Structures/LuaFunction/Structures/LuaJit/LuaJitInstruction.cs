using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaOpCodeTable.LuaJit;

namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Structures.LuaJit
{
    public class LuaJitInstruction : Instruction
    {
        public LuaJitOpCodeDef OpCode { get; set; }
        public long CD { get; set; }
    }
}