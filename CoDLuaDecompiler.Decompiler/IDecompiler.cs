using CoDLuaDecompiler.Decompiler.LuaFile;

namespace CoDLuaDecompiler.Decompiler
{
    public interface IDecompiler
    {
        public string Decompile(ILuaFile luaFile);
    }
}