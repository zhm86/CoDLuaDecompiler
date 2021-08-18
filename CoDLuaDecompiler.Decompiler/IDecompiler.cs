using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.LuaFile;

namespace CoDLuaDecompiler.Decompiler
{
    public interface IDecompiler
    {
        public string Decompile(ILuaFile luaFile);
        Function GetDecompiledFile(ILuaFile luaFile);
    }
}