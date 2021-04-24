using CoDHVKDecompiler.Common;

namespace CoDHVKDecompiler.Decompiler
{
    public interface IDecompiler
    {
        public string Decompile(ILuaFile luaFile);
    }
}