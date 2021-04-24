using System.Collections.Generic;
using CoDHVKDecompiler.Common.LuaFunction.Structures;

namespace CoDHVKDecompiler.Decompiler.IR.Instruction
{
    public class Data : IInstruction
    {
        public IInstruction Instruction { get; set; }
    }
}