using System.Collections.Generic;
using CoDHVKDecompiler.Common.LuaConstant;
using CoDHVKDecompiler.Common.LuaFunction.Structures;

namespace CoDHVKDecompiler.Common.LuaFunction
{
    public interface ILuaFunction
    {
        ILuaFile LuaFile { get; }
        FunctionHeader Header { get; }
        long FunctionPos { get; }
        long FunctionLength { get; }
        List<Instruction> Instructions { get; }
        List<ILuaConstant> Constants { get; }
        List<ILuaFunction> ChildFunctions { get; }
        List<Upvalue> Upvalues { get; }
        List<Local> Locals { get; }
        Dictionary<int, List<Local>> LocalMap { get; }
        List<Local> LocalsAt(int i);
    }
}