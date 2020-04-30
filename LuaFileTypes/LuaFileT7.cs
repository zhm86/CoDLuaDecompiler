using System;
using System.Collections.Generic;
using System.IO;
using DSLuaDecompiler.LuaFileTypes.Structures;
using PhilLibX.IO;

namespace DSLuaDecompiler.LuaFileTypes
{
    public class LuaFileT7 : LuaFile
    {
        public LuaFileT7(string filePath, BinaryReader stream) : base(filePath, stream) {}
        public override Dictionary<int, LuaOpCode> OpCodeTable => new Dictionary<int, LuaOpCode>()
        {
            { 0,        LuaOpCode.HKS_OPCODE_GETFIELD },
            { 1,        LuaOpCode.HKS_OPCODE_TEST },
            { 2,        LuaOpCode.HKS_OPCODE_CALL_I },
            { 3,        LuaOpCode.HKS_OPCODE_CALL_C },
            { 4,        LuaOpCode.HKS_OPCODE_EQ },
            { 5,        LuaOpCode.HKS_OPCODE_EQ_BK },
            { 6,        LuaOpCode.HKS_OPCODE_GETGLOBAL },
            { 7,        LuaOpCode.HKS_OPCODE_MOVE },
            { 8,        LuaOpCode.HKS_OPCODE_SELF },
            { 9,        LuaOpCode.HKS_OPCODE_RETURN },
            { 10,        LuaOpCode.HKS_OPCODE_GETTABLE_S },
            { 11,        LuaOpCode.HKS_OPCODE_GETTABLE_N },
            { 12,        LuaOpCode.HKS_OPCODE_GETTABLE },
            { 13,        LuaOpCode.HKS_OPCODE_LOADBOOL },
            { 14,        LuaOpCode.HKS_OPCODE_TFORLOOP },
            { 15,        LuaOpCode.HKS_OPCODE_SETFIELD },
            { 16,        LuaOpCode.HKS_OPCODE_SETTABLE_S },
            { 17,        LuaOpCode.HKS_OPCODE_SETTABLE_S_BK },
            { 18,        LuaOpCode.HKS_OPCODE_SETTABLE_N },
            { 19,        LuaOpCode.HKS_OPCODE_SETTABLE_N_BK },
            { 20,        LuaOpCode.HKS_OPCODE_SETTABLE },
            { 21,        LuaOpCode.HKS_OPCODE_SETTABLE_BK },
            { 22,        LuaOpCode.HKS_OPCODE_TAILCALL_I },
            { 23,        LuaOpCode.HKS_OPCODE_TAILCALL_C },
            { 24,        LuaOpCode.HKS_OPCODE_TAILCALL_M },
            { 25,        LuaOpCode.HKS_OPCODE_LOADK },
            { 26,        LuaOpCode.HKS_OPCODE_LOADNIL },
            { 27,        LuaOpCode.HKS_OPCODE_SETGLOBAL },
            { 28,        LuaOpCode.HKS_OPCODE_JMP },
            { 29,        LuaOpCode.HKS_OPCODE_CALL_M },
            { 30,        LuaOpCode.HKS_OPCODE_CALL },
            { 31,        LuaOpCode.HKS_OPCODE_INTRINSIC_INDEX },
            { 32,        LuaOpCode.HKS_OPCODE_INTRINSIC_NEWINDEX },
            { 33,        LuaOpCode.HKS_OPCODE_INTRINSIC_SELF },
            { 34,        LuaOpCode.HKS_OPCODE_INTRINSIC_INDEX_LITERAL },
            { 35,        LuaOpCode.HKS_OPCODE_INTRINSIC_NEWINDEX_LITERAL },
            { 36,        LuaOpCode.HKS_OPCODE_INTRINSIC_SELF_LITERAL },
            { 37,        LuaOpCode.HKS_OPCODE_TAILCALL },
            { 38,        LuaOpCode.HKS_OPCODE_GETUPVAL },
            { 39,        LuaOpCode.HKS_OPCODE_SETUPVAL },
            { 40,        LuaOpCode.HKS_OPCODE_ADD },
            { 41,        LuaOpCode.HKS_OPCODE_ADD_BK },
            { 42,        LuaOpCode.HKS_OPCODE_SUB },
            { 43,        LuaOpCode.HKS_OPCODE_SUB_BK },
            { 44,        LuaOpCode.HKS_OPCODE_MUL },
            { 45,        LuaOpCode.HKS_OPCODE_MUL_BK },
            { 46,        LuaOpCode.HKS_OPCODE_DIV },
            { 47,        LuaOpCode.HKS_OPCODE_DIV_BK },
            { 48,        LuaOpCode.HKS_OPCODE_MOD },
            { 49,        LuaOpCode.HKS_OPCODE_MOD_BK },
            { 50,        LuaOpCode.HKS_OPCODE_POW },
            { 51,        LuaOpCode.HKS_OPCODE_POW_BK },
            { 52,        LuaOpCode.HKS_OPCODE_NEWTABLE },
            { 53,        LuaOpCode.HKS_OPCODE_UNM },
            { 54,        LuaOpCode.HKS_OPCODE_NOT },
            { 55,        LuaOpCode.HKS_OPCODE_LEN },
            { 56,        LuaOpCode.HKS_OPCODE_LT },
            { 57,        LuaOpCode.HKS_OPCODE_LT_BK },
            { 58,        LuaOpCode.HKS_OPCODE_LE },
            { 59,        LuaOpCode.HKS_OPCODE_LE_BK },
            { 60,        LuaOpCode.HKS_OPCODE_SHIFT_LEFT },
            { 61,        LuaOpCode.HKS_OPCODE_SHIFT_LEFT_BK },
            { 62,        LuaOpCode.HKS_OPCODE_SHIFT_RIGHT },
            { 63,        LuaOpCode.HKS_OPCODE_SHIFT_RIGHT_BK },
            { 64,        LuaOpCode.HKS_OPCODE_BITWISE_AND },
            { 65,        LuaOpCode.HKS_OPCODE_BITWISE_AND_BK },
            { 66,        LuaOpCode.HKS_OPCODE_BITWISE_OR },
            { 67,        LuaOpCode.HKS_OPCODE_BITWISE_OR_BK },
            { 68,        LuaOpCode.HKS_OPCODE_CONCAT },
            { 69,        LuaOpCode.HKS_OPCODE_TESTSET },
            { 70,        LuaOpCode.HKS_OPCODE_FORPREP },
            { 71,        LuaOpCode.HKS_OPCODE_FORLOOP },
            { 72,        LuaOpCode.HKS_OPCODE_SETLIST },
            { 73,        LuaOpCode.HKS_OPCODE_CLOSE },
            { 74,        LuaOpCode.HKS_OPCODE_CLOSURE },
            { 75,        LuaOpCode.HKS_OPCODE_VARARG },
            { 76,        LuaOpCode.HKS_OPCODE_TAILCALL_I_R1 },
            { 77,        LuaOpCode.HKS_OPCODE_CALL_I_R1 },
            { 78,        LuaOpCode.HKS_OPCODE_SETUPVAL_R1 },
            { 79,        LuaOpCode.HKS_OPCODE_TEST_R1 },
            { 80,        LuaOpCode.HKS_OPCODE_NOT_R1 },
            { 81,        LuaOpCode.HKS_OPCODE_GETFIELD_R1 },
            { 82,        LuaOpCode.HKS_OPCODE_SETFIELD_R1 },
            { 83,        LuaOpCode.HKS_OPCODE_NEWSTRUCT },
            { 84,        LuaOpCode.HKS_OPCODE_DATA },
            { 85,        LuaOpCode.HKS_OPCODE_SETSLOTN },
            { 86,        LuaOpCode.HKS_OPCODE_SETSLOTI },
            { 87,        LuaOpCode.HKS_OPCODE_SETSLOT },
            { 88,        LuaOpCode.HKS_OPCODE_SETSLOTS },
            { 89,        LuaOpCode.HKS_OPCODE_SETSLOTMT },
            { 90,        LuaOpCode.HKS_OPCODE_CHECKTYPE },
            { 91,        LuaOpCode.HKS_OPCODE_CHECKTYPES },
            { 92,        LuaOpCode.HKS_OPCODE_GETSLOT },
            { 93,        LuaOpCode.HKS_OPCODE_GETSLOTMT },
            { 94,        LuaOpCode.HKS_OPCODE_SELFSLOT },
            { 95,        LuaOpCode.HKS_OPCODE_SELFSLOTMT },
            { 96,        LuaOpCode.HKS_OPCODE_GETFIELD_MM },
            { 97,        LuaOpCode.HKS_OPCODE_CHECKTYPE_D },
            { 98,        LuaOpCode.HKS_OPCODE_GETSLOT_D },
            { 99,        LuaOpCode.HKS_OPCODE_GETGLOBAL_MEM },
            { 100,        LuaOpCode.HKS_OPCODE_MAX },
        };

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
                GameByte = Reader.ReadByte(),
            };
            Reader.ReadByte();
            Header.ConstantTypeCount = Reader.ReadInt32();

            for (int i = 0; i < Header.ConstantTypeCount; i++)
            {
                /*ConstantTypes.Add(new ConstantType()
                {
                    Type = (ConstantType.Types)Reader.ReadInt32(),
                    TypeLength = Reader.ReadInt32() - 1,
                    TypeName = Reader.ReadNullTerminatedString()
                });*/

                var type = Reader.ReadInt32();
                var strLength = Reader.ReadInt32();
                Reader.ReadBytes(strLength);
            }
        }

        public override void ReadFunctions()
        {
            MainFunction = new Function(this);
        }

        public override void ReadFunctionHeader(Function function)
        {
            function.UpvalCount = Reader.ReadInt32();
            function.ParameterCount = Reader.ReadInt32();
            function.UsesVarArg = Reader.ReadByte() == 1;
            function.RegisterCount = Reader.ReadInt32();
            function.InstructionCount = Reader.ReadInt32();
            // Some unknown int
            Reader.ReadInt32();
            // Add some padding
            int extra = 4 - (int)Reader.BaseStream.Position % 4;
            if (extra > 0 && extra < 4)
                Reader.ReadBytes(extra);
        }

        public override Instruction ReadInstruction(Function function)
        {
            var instruction = new Instruction();
            // Reading the values attached to the instruction
            // A = 8 bits
            // C = 9 bits
            // B = 8 bits
            // OpCode = 7 bits
            instruction.A = Reader.ReadByte();

            int cValue = Reader.ReadByte();
            byte bValue = Reader.ReadByte();
            if (GetBit(bValue, 0) == 1)
                instruction.ExtraCBit = true;
            instruction.C = (uint) cValue;

            instruction.B = (uint) (bValue >> 1);
            byte flagsB = Reader.ReadByte();
            if (GetBit(flagsB, 0) == 1)
                instruction.B += 128;
            
            instruction.OpCode = LuaOpCode.HKS_OPCODE_UNK;
            if (OpCodeTable.TryGetValue(flagsB >> 1, out LuaOpCode opCode))
                instruction.OpCode = opCode;
            
            return instruction;
        }

        public override void ReadConstants(Function function)
        {
            function.ConstantCount = Reader.ReadInt32();
            for (int i = 0; i < function.ConstantCount; i++)
            {
                var constant = new Constant()
                {
                    Type = (ConstantType) Reader.ReadByte()
                };
                switch (constant.Type)
                {
                    case ConstantType.TBoolean:
                        constant.NumberValue = Reader.ReadByte();
                        break;
                    case ConstantType.TNumber:
                        constant.NumberValue = Math.Round(Reader.ReadSingle(), 2, MidpointRounding.ToEven);
                        break;
                    case ConstantType.TString:
                        Reader.ReadInt32(); // String length, not needed since it's null terminated
                        Reader.ReadInt32(); // Unknown, always null
                        constant.StringValue = Reader.ReadNullTerminatedString();
                        break;
                    case ConstantType.THash:
                        constant.HashValue = Reader.ReadUInt64();
                        break;
                }
                function.Constants.Add(constant);
            }
        }

        public override void ReadFunctionFooter(Function function)
        {
            Reader.ReadInt32(); // Unknown int
            Reader.ReadSingle(); // Unknown float
            function.SubFunctionCount = Reader.ReadInt32();
        }

        public override void ReadSubFunctions(Function function)
        {
            for (int i = 0; i < function.SubFunctionCount; i++)
            {
                function.ChildFunctions.Add(new Function(this));
            }
        }

        private byte GetBit(long input, int bit)
        {
            return (byte)((input >> bit) & 1);
        }
    }
}