using System.IO;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.LuaJit;

namespace CoDLuaDecompiler.Decompiler.LuaFile.LuaJit
{
    public class LuaJitFileMw2 : LuaJitFile
    {
        public LuaJitFileMw2(BinaryReader reader) : base(reader)
        {
        }

        protected override LuaJitFunction ReadFunction()
        {
            return new LuaJitFunctionMw2(this, Reader);
        }
    }
}