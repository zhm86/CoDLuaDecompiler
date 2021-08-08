namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaOpCodeTable.LuaJit
{
    public class LuaJitOpCodeDef
    {
        public LuaJitOpCodeDef(string name, LuaJitArgumentType aType, LuaJitArgumentType bType, LuaJitArgumentType cdType)
        {
            Name = name;
            AType = aType;
            BType = bType;
            CDType = cdType;
        }

        public string Name { get; set; }
        public LuaJitArgumentType AType { get; set; }
        public LuaJitArgumentType BType { get; set; }
        public LuaJitArgumentType CDType { get; set; }

        public int ArgumentsCount =>
            (AType != LuaJitArgumentType.None ? 1 : 0) + (BType != LuaJitArgumentType.None ? 1 : 0) +
            (CDType != LuaJitArgumentType.None ? 1 : 0);
    }
}