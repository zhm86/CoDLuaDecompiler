using System.Collections.Generic;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaConstant;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaConstant.LuaJit;

namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Structures.LuaJit
{
    public class LuaJitTable
    {
        public IList<ILuaJitConstant> Array { get; } = new List<ILuaJitConstant>();

        public Dictionary<ILuaJitConstant, ILuaJitConstant> Dictionary { get; } =
            new();
    }
}