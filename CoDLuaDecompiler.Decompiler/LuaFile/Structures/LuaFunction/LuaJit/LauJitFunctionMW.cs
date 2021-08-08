using System.IO;

namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.LuaJit
{
    public class LauJitFunctionMW : LuaJitFunction
    {
        public LauJitFunctionMW(ILuaFile luaFile, BinaryReader reader) : base(luaFile, reader)
        {
        }
    }
}