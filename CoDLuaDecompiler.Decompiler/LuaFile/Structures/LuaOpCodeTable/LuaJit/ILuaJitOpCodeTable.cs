namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaOpCodeTable.LuaJit
{
    public interface ILuaJitOpCodeTable
    {
        LuaJitOpCodeDef GetValue(int parsedOpCode);
        bool TryGetValue(int parsedOpCode, out LuaJitOpCodeDef luaLuaJitOpCode);
    }
}