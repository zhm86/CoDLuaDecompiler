using System.Collections.Generic;
using System.IO;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaConstant;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaConstant.Havok;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Structures;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Structures.Havok;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaOpCodeTable;
using CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaOpCodeTable.Havok;

namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaFunction.Havok
{
    public abstract class HavokLuaFunction : LuaFunction
    {
        public List<IHavokLuaConstant> Constants { get; protected set; }
        public virtual IHavokLuaOpCodeTable OpCodeTable => new HavokDefaultHavokLuaOpCodeTable();
        
        public HavokLuaFunction(ILuaFile luaFile, BinaryReader reader) : base(luaFile, reader)
        {
        }

        public override Function IRFunction { get; set; }        
        protected override void Parse()
        {
            FunctionPos = Reader.BaseStream.Position;
            Header = ReadFunctionHeader();

            for (var i = 0; i < Header.InstructionCount; i++)
            {
                Instructions.Add(ReadInstruction());
            }

            ReadConstants();
            Footer = ReadFunctionFooter();
            ChildFunctions = ReadChildFunctions();
        }

        protected override FunctionHeader ReadFunctionHeader()
        {
            var header = new HavokFunctionHeader()
            {
                UpvaluesCount = Reader.ReadInt32(),
                ParameterCount = Reader.ReadInt32(),
                UsesVarArg = Reader.ReadByte() == 1,
                RegisterCount = Reader.ReadInt32(),
                InstructionCount = Reader.ReadInt32()
            };
            // Some unknown int
            Reader.ReadInt32();
            // Add some padding
            var extra = 4 - (int)Reader.BaseStream.Position % 4;
            if (extra > 0 && extra < 4)
            {
                Reader.ReadBytes(extra);
            }

            return header;
        }
        
        protected virtual FunctionFooter ReadFunctionFooter()
        {
            return new FunctionFooter
            {
                Unknown1 = Reader.ReadInt32(),
                Unknown2 = Reader.ReadSingle(),
                SubFunctionCount = Reader.ReadInt32(),
            };
        }

        protected override Instruction ReadInstruction()
        {
            var instruction = new HavokInstruction();
            // Reading the values attached to the instruction
            // A = 8 bits
            // C = 9 bits
            // B = 8 bits
            // OpCode = 7 bits
            instruction.A = Reader.ReadByte();

            int cValue = Reader.ReadByte();
            byte bValue = Reader.ReadByte();
            if (GetBit(bValue, 0) == 1)
            {
                instruction.ExtraCBit = true;
            }
            instruction.C = (uint) cValue;

            instruction.B = (uint) (bValue >> 1);
            byte flagsB = Reader.ReadByte();
            if (GetBit(flagsB, 0) == 1)
            {
                instruction.B += 128;
            }
            
            instruction.HavokOpCode = LuaHavokOpCode.HKS_OPCODE_UNK;

            if (OpCodeTable.TryGetValue(flagsB >> 1, out var opCode))
            {
                instruction.HavokOpCode = opCode;
            }

            instruction.Bx = (uint) (instruction.B * 512 + instruction.C + (instruction.ExtraCBit ? 256 : 0));
            instruction.SBx = (int) (instruction.Bx - 65536 + 1);
            
            return instruction;
        }

        protected override void ReadConstants()
        {
            var constants = new List<IHavokLuaConstant>();

            var constantCount = Reader.ReadInt32();
            for (var i = 0; i < constantCount; i++)
            {
                constants.Add(new HavokLuaConstantT7(Reader));
            }

            Constants = constants;
        }
        
        protected virtual List<ILuaFunction> ReadChildFunctions()
        {
            var childFunctions = new List<ILuaFunction>();

            for (var i = 0; i < Footer.SubFunctionCount; i++)
            {
                childFunctions.Add(new HavokLuaFunctionT7(LuaFile, Reader));
            }

            return childFunctions;
        }
        
        public byte GetBit(long input, int bit)
        {
            return (byte)((input >> bit) & 1);
        }
    }
}