namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Structures
{
    public abstract class FunctionHeader
    {
        public int UpvaluesCount { get; set; }
        public int ParameterCount { get; set; }
        public bool UsesVarArg { get; set; }
        public int RegisterCount { get; set; }
        public int InstructionCount { get; set; }
    }
}