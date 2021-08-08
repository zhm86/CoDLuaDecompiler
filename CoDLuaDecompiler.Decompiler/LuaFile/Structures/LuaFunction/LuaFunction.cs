using System.Collections.Generic;
using System.IO;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaConstant;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Structures;

namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction
{
    public abstract class LuaFunction : ILuaFunction
    {
        public ILuaFile LuaFile { get; }
        public FunctionHeader Header { get; protected set; }
        public FunctionFooter Footer { get; protected set; }
        public long FunctionPos { get; protected set; }
        public long FunctionLength { get; protected set; }
        public List<Instruction> Instructions { get; } = new List<Instruction>();
        public List<ILuaFunction> ChildFunctions { get; protected set; } = new List<ILuaFunction>();
        public List<Upvalue> Upvalues { get; set; } = new List<Upvalue>();
        public abstract Function IRFunction { get; set; }

        // private variables
        protected readonly BinaryReader Reader;

        protected LuaFunction(ILuaFile luaFile, BinaryReader reader)
        {
            LuaFile = luaFile;
            Reader = reader;

            Parse();
        }
        
        protected virtual void Parse()
        {
            FunctionPos = Reader.BaseStream.Position;
            Header = ReadFunctionHeader();
            if (Header == null)
                return;

            for (var i = 0; i < Header.InstructionCount; i++)
            {
                Instructions.Add(ReadInstruction());
            }

            ReadConstants();
        }

        protected abstract FunctionHeader ReadFunctionHeader();
        protected abstract Instruction ReadInstruction();
        protected abstract void ReadConstants();
    }
}