using System.Collections.Generic;
using CoDHVKDecompiler.Common.LuaConstant;
using CoDHVKDecompiler.Common.LuaFunction;
using CoDHVKDecompiler.Common.Structures;

namespace CoDHVKDecompiler.Common
{
    public interface ILuaFile
    {
        FileHeader Header { get; }
        IList<ILuaConstant> Constants { get; }
        ILuaFunction MainFunction { get; }
    }
}