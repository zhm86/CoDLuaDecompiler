namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaOpCodeTable.Havok
{
    public interface IHavokLuaOpCodeTable
    {
        LuaHavokOpCode GetValue(int parsedOpCode);
        bool TryGetValue(int parsedOpCode, out LuaHavokOpCode luaHavokOpCode);
    }
}