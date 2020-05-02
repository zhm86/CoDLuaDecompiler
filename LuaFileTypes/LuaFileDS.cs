using System;
using System.Collections.Generic;
using System.IO;
using DSLuaDecompiler.LuaFileTypes.OpCodeTables;
using DSLuaDecompiler.LuaFileTypes.Structures;
using PhilLibX.IO;

namespace DSLuaDecompiler.LuaFileTypes
{
    public class LuaFileDS : LuaFile
    {
        public LuaFileDS(string filePath, BinaryReader reader) : base(filePath, reader) {}

        public override Dictionary<int, LuaOpCode> OpCodeTable => DefaultOpCodeTable.OpCodeTable;
        public override void ReadHeader()
        {
            Header = new FileHeader()
            {
                Magic = Reader.ReadChars(4).ToString(),
                LuaVersion = Reader.ReadByte(),
                CompilerVersion = Reader.ReadByte(),
                Endianness = Reader.ReadByte(),
                SizeOfInt = Reader.ReadByte(),
                SizeOfSizeT = Reader.ReadByte(),
                SizeOfInstruction = Reader.ReadByte(),
                SizeOfLuaNumber = Reader.ReadByte(),
                IntegralFlag = Reader.ReadByte(),
            };

            Reader.ReadBytes(5);
            Header.ConstantTypeCount = Reader.ReadByte();

            for (int i = 0; i < Header.ConstantTypeCount; i++)
            {
                Reader.ReadInt32();
                Reader.ReadInt32();
                var str = Reader.ReadNullTerminatedString();
            }
        }

        public override void ReadFunctions()
        {
            MainFunction = new Function(this);
        }

        public override void ReadFunctionHeader(Function function)
        {
            function.UpvalCount = Reader.ReadBEInt32();
            function.ParameterCount = Reader.ReadBEInt32();
            function.UsesVarArg = Reader.ReadByte() == 1;
            function.RegisterCount = Reader.ReadBEInt32();
            Reader.ReadBEInt32();
            function.InstructionCount = Reader.ReadBEInt32();
            
            // Add some padding
            int extra = 4 - (int)Reader.BaseStream.Position % 4;
            if (extra > 0 && extra < 4)
                Reader.ReadBytes(extra);
        }

        public override Instruction ReadInstruction(Function function)
        {
            var instr = new Instruction();

            var instruction = Reader.ReadBEInt32();
            var opcode = (instruction & 0xFF000000) >> 25;
            var a = instruction & 0xFF;
            int c = (int)(instruction & 0x1FF00) >> 8;
            int b = (int)(instruction & 0x1FE0000) >> 17;
            bool szero = false;

            if (c >= 256)
            {
                c -= 256;
                szero = true;
            }

            instr.A = (uint) a;
            instr.B = (uint) b;
            instr.C = (uint) c;
            instr.Bx = (uint) ((instruction & 0x1FFFF00) >> 8);
            instr.SBx = (int) (instr.Bx - 65536 + 1);
            instr.ExtraCBit = szero;
            instr.OpCode = LuaOpCode.HKS_OPCODE_UNK;
            if (OpCodeTable.TryGetValue((int) opcode, out LuaOpCode opCode))
                instr.OpCode = opCode;
            
            return instr;
        }

        public override void ReadConstants(Function function)
        {
            function.ConstantCount = Reader.ReadBEInt32();
            for (int i = 0; i < function.ConstantCount; i++)
            {
                var constant = new Constant()
                {
                    Type = (ConstantType) Reader.ReadByte()
                };
                switch (constant.Type)
                {
                    case ConstantType.TBoolean:
                        constant.BoolValue = Reader.ReadByte() == 1;
                        break;
                    case ConstantType.TNumber:
                        constant.NumberValue = Math.Round(Reader.ReadBESingle(), 2, MidpointRounding.ToEven);
                        break;
                    case ConstantType.TString:
                        Reader.ReadInt32(); // String length, not needed since it's null terminated
                        Reader.ReadInt32(); // Unknown, always null
                        constant.StringValue = Reader.ReadNullTerminatedUTF8String();
                        break;
                }
                function.Constants.Add(constant);
            }
        }

        public override void ReadFunctionFooter(Function function)
        {
            Reader.ReadInt64();
            
            function.LocalVarsCount = Reader.ReadBEInt32();
            function.UpvalCount = Reader.ReadBEInt32();
            
            Reader.ReadInt64();
            
            Reader.ReadInt64();
            function.Path = Reader.ReadNullTerminatedString();
            Reader.ReadInt64();
            function.Name = Reader.ReadNullTerminatedString();

            Reader.ReadBytes(4 * function.InstructionCount);

            for (int i = 0; i < function.LocalVarsCount; i++)
            {
                Reader.ReadUInt64();
                var local = new Local()
                {
                    Name = Reader.ReadNullTerminatedString(),
                    Start = Reader.ReadBEInt32(),
                    End = Reader.ReadBEInt32()
                };
                function.Locals.Add(local);
                if (!local.Name.StartsWith("("))
                {
                    if (!function.LocalMap.ContainsKey(local.Start))
                    {
                        function.LocalMap[local.Start] = new List<Local>();
                    }
                    function.LocalMap[local.Start].Add(local);
                }
            }

            for (int i = 0; i < function.UpvalCount; i++)
            {
                Reader.ReadUInt64();
                function.Upvalues.Add(new Upvalue()
                {
                    Name = Reader.ReadNullTerminatedString()
                });
            }

            function.SubFunctionCount = Reader.ReadBEInt32();
        }

        public override void ReadSubFunctions(Function function)
        {
            for (int i = 0; i < function.SubFunctionCount; i++)
            {
                function.ChildFunctions.Add(new Function(this));
            }
        }
    }
}