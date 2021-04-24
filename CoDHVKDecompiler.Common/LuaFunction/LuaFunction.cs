using System.Collections.Generic;
using System.IO;
using CoDHVKDecompiler.Common.LuaConstant;
using CoDHVKDecompiler.Common.LuaFunction.Structures;

namespace CoDHVKDecompiler.Common.LuaFunction
{
    public abstract class LuaFunction : ILuaFunction
    {
        public ILuaFile LuaFile { get; }
        public FunctionHeader Header { get; private set; }
        public FunctionFooter Footer { get; private set; }
        public long FunctionPos { get; private set; }
        public long FunctionLength { get; private set; }
        public List<Instruction> Instructions { get; } = new List<Instruction>();
        public List<ILuaConstant> Constants { get; private set; }
        public List<ILuaFunction> ChildFunctions { get; private set; }
        public List<Upvalue> Upvalues { get; set; } = new List<Upvalue>();
        public List<Local> Locals { get; set; } = new List<Local>();
        public Dictionary<int, List<Local>> LocalMap { get; set; } = new Dictionary<int, List<Local>>();
        
        // private variables
        protected readonly BinaryReader Reader;

        protected LuaFunction(ILuaFile luaFile, BinaryReader reader)
        {
            LuaFile = luaFile;
            Reader = reader;

            Parse();
        }
        
        private void Parse()
        {
            FunctionPos = Reader.BaseStream.Position;
            Header = ReadFunctionHeader();

            for (var i = 0; i < Header.InstructionCount; i++)
            {
                Instructions.Add(ReadInstruction());
            }

            Constants = ReadConstants();
            Footer = ReadFunctionFooter();
            ChildFunctions = ReadChildFunctions();
        }

        protected abstract FunctionHeader ReadFunctionHeader();
        protected abstract FunctionFooter ReadFunctionFooter();
        protected abstract Instruction ReadInstruction();
        protected abstract List<ILuaConstant> ReadConstants();
        protected abstract List<ILuaFunction> ReadChildFunctions();

        public List<Local> LocalsAt(int i)
        {
            if (LocalMap.ContainsKey(i))
            {
                return LocalMap[i];
            }
            
            return null;
        }
    }
}