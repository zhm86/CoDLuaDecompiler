namespace CoDHVKDecompiler.Common.LuaOpCodeTable
{
    public interface ILuaOpCodeTable
    {
        LuaOpCode GetValue(int parsedOpCode);
        bool TryGetValue(int parsedOpCode, out LuaOpCode luaOpCode);
    }
}