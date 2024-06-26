using System.Collections.Generic;

namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaOpCodeTable.Havok
{
    public class HavokDefaultHavokLuaOpCodeTable : HavokLuaOpCodeTable
    {
        protected override Dictionary<int, LuaHavokOpCode> ParseLuaOpCodes()
        {
            return new Dictionary<int, LuaHavokOpCode>()
            {
                { 0,        LuaHavokOpCode.HKS_OPCODE_GETFIELD },
                { 1,        LuaHavokOpCode.HKS_OPCODE_TEST },
                { 2,        LuaHavokOpCode.HKS_OPCODE_CALL_I },
                { 3,        LuaHavokOpCode.HKS_OPCODE_CALL_C },
                { 4,        LuaHavokOpCode.HKS_OPCODE_EQ },
                { 5,        LuaHavokOpCode.HKS_OPCODE_EQ_BK },
                { 6,        LuaHavokOpCode.HKS_OPCODE_GETGLOBAL },
                { 7,        LuaHavokOpCode.HKS_OPCODE_MOVE },
                { 8,        LuaHavokOpCode.HKS_OPCODE_SELF },
                { 9,        LuaHavokOpCode.HKS_OPCODE_RETURN },
                { 10,        LuaHavokOpCode.HKS_OPCODE_GETTABLE_S },
                { 11,        LuaHavokOpCode.HKS_OPCODE_GETTABLE_N },
                { 12,        LuaHavokOpCode.HKS_OPCODE_GETTABLE },
                { 13,        LuaHavokOpCode.HKS_OPCODE_LOADBOOL },
                { 14,        LuaHavokOpCode.HKS_OPCODE_TFORLOOP },
                { 15,        LuaHavokOpCode.HKS_OPCODE_SETFIELD },
                { 16,        LuaHavokOpCode.HKS_OPCODE_SETTABLE_S },
                { 17,        LuaHavokOpCode.HKS_OPCODE_SETTABLE_S_BK },
                { 18,        LuaHavokOpCode.HKS_OPCODE_SETTABLE_N },
                { 19,        LuaHavokOpCode.HKS_OPCODE_SETTABLE_N_BK },
                { 20,        LuaHavokOpCode.HKS_OPCODE_SETTABLE },
                { 21,        LuaHavokOpCode.HKS_OPCODE_SETTABLE_BK },
                { 22,        LuaHavokOpCode.HKS_OPCODE_TAILCALL_I },
                { 23,        LuaHavokOpCode.HKS_OPCODE_TAILCALL_C },
                { 24,        LuaHavokOpCode.HKS_OPCODE_TAILCALL_M },
                { 25,        LuaHavokOpCode.HKS_OPCODE_LOADK },
                { 26,        LuaHavokOpCode.HKS_OPCODE_LOADNIL },
                { 27,        LuaHavokOpCode.HKS_OPCODE_SETGLOBAL },
                { 28,        LuaHavokOpCode.HKS_OPCODE_JMP },
                { 29,        LuaHavokOpCode.HKS_OPCODE_CALL_M },
                { 30,        LuaHavokOpCode.HKS_OPCODE_CALL },
                { 31,        LuaHavokOpCode.HKS_OPCODE_INTRINSIC_INDEX },
                { 32,        LuaHavokOpCode.HKS_OPCODE_INTRINSIC_NEWINDEX },
                { 33,        LuaHavokOpCode.HKS_OPCODE_INTRINSIC_SELF },
                { 34,        LuaHavokOpCode.HKS_OPCODE_INTRINSIC_INDEX_LITERAL },
                { 35,        LuaHavokOpCode.HKS_OPCODE_INTRINSIC_NEWINDEX_LITERAL },
                { 36,        LuaHavokOpCode.HKS_OPCODE_INTRINSIC_SELF_LITERAL },
                { 37,        LuaHavokOpCode.HKS_OPCODE_TAILCALL },
                { 38,        LuaHavokOpCode.HKS_OPCODE_GETUPVAL },
                { 39,        LuaHavokOpCode.HKS_OPCODE_SETUPVAL },
                { 40,        LuaHavokOpCode.HKS_OPCODE_ADD },
                { 41,        LuaHavokOpCode.HKS_OPCODE_ADD_BK },
                { 42,        LuaHavokOpCode.HKS_OPCODE_SUB },
                { 43,        LuaHavokOpCode.HKS_OPCODE_SUB_BK },
                { 44,        LuaHavokOpCode.HKS_OPCODE_MUL },
                { 45,        LuaHavokOpCode.HKS_OPCODE_MUL_BK },
                { 46,        LuaHavokOpCode.HKS_OPCODE_DIV },
                { 47,        LuaHavokOpCode.HKS_OPCODE_DIV_BK },
                { 48,        LuaHavokOpCode.HKS_OPCODE_MOD },
                { 49,        LuaHavokOpCode.HKS_OPCODE_MOD_BK },
                { 50,        LuaHavokOpCode.HKS_OPCODE_POW },
                { 51,        LuaHavokOpCode.HKS_OPCODE_POW_BK },
                { 52,        LuaHavokOpCode.HKS_OPCODE_NEWTABLE },
                { 53,        LuaHavokOpCode.HKS_OPCODE_UNM },
                { 54,        LuaHavokOpCode.HKS_OPCODE_NOT },
                { 55,        LuaHavokOpCode.HKS_OPCODE_LEN },
                { 56,        LuaHavokOpCode.HKS_OPCODE_LT },
                { 57,        LuaHavokOpCode.HKS_OPCODE_LT_BK },
                { 58,        LuaHavokOpCode.HKS_OPCODE_LE },
                { 59,        LuaHavokOpCode.HKS_OPCODE_LE_BK },
                { 60,        LuaHavokOpCode.HKS_OPCODE_CONCAT },
                { 61,        LuaHavokOpCode.HKS_OPCODE_TESTSET },
                { 62,        LuaHavokOpCode.HKS_OPCODE_FORPREP },
                { 63,        LuaHavokOpCode.HKS_OPCODE_FORLOOP },
                { 64,        LuaHavokOpCode.HKS_OPCODE_SETLIST },
                { 65,        LuaHavokOpCode.HKS_OPCODE_CLOSE },
                { 66,        LuaHavokOpCode.HKS_OPCODE_CLOSURE },
                { 67,        LuaHavokOpCode.HKS_OPCODE_VARARG },
                { 68,        LuaHavokOpCode.HKS_OPCODE_TAILCALL_I_R1 },
                { 69,        LuaHavokOpCode.HKS_OPCODE_CALL_I_R1 },
                { 70,        LuaHavokOpCode.HKS_OPCODE_SETUPVAL_R1 },
                { 71,        LuaHavokOpCode.HKS_OPCODE_TEST_R1 },
                { 72,        LuaHavokOpCode.HKS_OPCODE_NOT_R1 },
                { 73,        LuaHavokOpCode.HKS_OPCODE_GETFIELD_R1 },
                { 74,        LuaHavokOpCode.HKS_OPCODE_SETFIELD_R1 },
                { 75,        LuaHavokOpCode.HKS_OPCODE_NEWSTRUCT },
                { 76,        LuaHavokOpCode.HKS_OPCODE_DATA },
                { 77,        LuaHavokOpCode.HKS_OPCODE_SETSLOTN },
                { 78,        LuaHavokOpCode.HKS_OPCODE_SETSLOTI },
                { 79,        LuaHavokOpCode.HKS_OPCODE_SETSLOT },
                { 80,        LuaHavokOpCode.HKS_OPCODE_SETSLOTS },
                { 81,        LuaHavokOpCode.HKS_OPCODE_SETSLOTMT },
                { 82,        LuaHavokOpCode.HKS_OPCODE_CHECKTYPE },
                { 83,        LuaHavokOpCode.HKS_OPCODE_CHECKTYPES },
                { 84,        LuaHavokOpCode.HKS_OPCODE_GETSLOT },
                { 85,        LuaHavokOpCode.HKS_OPCODE_GETSLOTMT },
                { 86,        LuaHavokOpCode.HKS_OPCODE_SELFSLOT },
                { 87,        LuaHavokOpCode.HKS_OPCODE_SELFSLOTMT },
                { 88,        LuaHavokOpCode.HKS_OPCODE_GETFIELD_MM },
                { 89,        LuaHavokOpCode.HKS_OPCODE_CHECKTYPE_D },
                { 90,        LuaHavokOpCode.HKS_OPCODE_GETSLOT_D },
                { 91,        LuaHavokOpCode.HKS_OPCODE_GETGLOBAL_MEM },
                { 92,        LuaHavokOpCode.HKS_OPCODE_MAX },
            };
        }
    }
}