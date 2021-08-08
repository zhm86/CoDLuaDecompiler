namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Structures.LuaJit
{
    public class LuaJitFunctionHeader : FunctionHeader
    {
        public bool HasFfi { get; set; }
        public bool HasILoop { get; set; }
        public bool JitDisabled { get; set; }
        public bool HasChild { get; set; }
        
        public ulong ComplexConstantsCount { get; set; }
        public ulong NumericConstantsCount { get; set; }
        
        public ulong DebugInfoSize { get; set; } = 0;
        public ulong FirstLineNumber { get; set; }
        public ulong LinesCount { get; set; }
    }
}