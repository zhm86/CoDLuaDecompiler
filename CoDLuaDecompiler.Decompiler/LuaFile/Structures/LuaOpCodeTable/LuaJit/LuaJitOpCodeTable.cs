using System;
using System.Collections.Generic;

namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaOpCodeTable.LuaJit
{
    public class LuaJitOpCodeTable : ILuaJitOpCodeTable
    {
        protected Dictionary<int, LuaJitOpCodeDef> OpCodeTable;

        public LuaJitOpCodeTable()
        {
            Parse();
        }

        public LuaJitOpCodeDef GetValue(int parsedOpCode)
        {
            return OpCodeTable[parsedOpCode];
        }

        public bool TryGetValue(int parsedOpCode, out LuaJitOpCodeDef luaLuaJitOpCode)
        {
            try
            {
                luaLuaJitOpCode = GetValue(parsedOpCode);
                return true;
            }
            catch (Exception)
            {
                // ignored
            }

            luaLuaJitOpCode = LuaJitOpCode.UNKNW;
            return false;
        }

        private void Parse()
        {
            OpCodeTable = ParseLuaOpCodes();
        }

        protected virtual Dictionary<int, LuaJitOpCodeDef> ParseLuaOpCodes()
        {
            return new Dictionary<int, LuaJitOpCodeDef>
            {
                {0, LuaJitOpCode.ISLT},
                {1, LuaJitOpCode.ISGE},
                {2, LuaJitOpCode.ISLE},
                {3, LuaJitOpCode.ISGT},
                {4, LuaJitOpCode.ISEQV},
                {5, LuaJitOpCode.ISNEV},
                {6, LuaJitOpCode.ISEQS},
                {7, LuaJitOpCode.ISNES},
                {8, LuaJitOpCode.ISEQN},
                {9, LuaJitOpCode.ISNEN},
                {10, LuaJitOpCode.ISEQP},
                {11, LuaJitOpCode.ISNEP},
                {12, LuaJitOpCode.ISTC},
                {13, LuaJitOpCode.ISFC},
                {14, LuaJitOpCode.IST},
                {15, LuaJitOpCode.ISF},
                {16, LuaJitOpCode.ISTYPE},
                {17, LuaJitOpCode.ISNUM},
                {18, LuaJitOpCode.MOV},
                {19, LuaJitOpCode.NOT},
                {20, LuaJitOpCode.UNM},
                {21, LuaJitOpCode.LEN},
                {22, LuaJitOpCode.ADDVN},
                {23, LuaJitOpCode.SUBVN},
                {24, LuaJitOpCode.MULVN},
                {25, LuaJitOpCode.DIVVN},
                {26, LuaJitOpCode.MODVN},
                {27, LuaJitOpCode.ADDNV},
                {28, LuaJitOpCode.SUBNV},
                {29, LuaJitOpCode.MULNV},
                {30, LuaJitOpCode.DIVNV},
                {31, LuaJitOpCode.MODNV},
                {32, LuaJitOpCode.ADDVV},
                {33, LuaJitOpCode.SUBVV},
                {34, LuaJitOpCode.MULVV},
                {35, LuaJitOpCode.DIVVV},
                {36, LuaJitOpCode.MODVV},
                {37, LuaJitOpCode.POW},
                {38, LuaJitOpCode.CAT},
                {39, LuaJitOpCode.KSTR},
                {40, LuaJitOpCode.KCDATA},
                {41, LuaJitOpCode.KSHORT},
                {42, LuaJitOpCode.KNUM},
                {43, LuaJitOpCode.KPRI},
                {44, LuaJitOpCode.KNIL},
                {45, LuaJitOpCode.UGET},
                {46, LuaJitOpCode.USETV},
                {47, LuaJitOpCode.USETS},
                {48, LuaJitOpCode.USETN},
                {49, LuaJitOpCode.USETP},
                {50, LuaJitOpCode.UCLO},
                {51, LuaJitOpCode.FNEW},
                {52, LuaJitOpCode.TNEW},
                {53, LuaJitOpCode.TDUP},
                {54, LuaJitOpCode.GGET},
                {55, LuaJitOpCode.GSET},
                {56, LuaJitOpCode.TGETV},
                {57, LuaJitOpCode.TGETS},
                {58, LuaJitOpCode.TGETB},
                {59, LuaJitOpCode.TGETR},
                {60, LuaJitOpCode.TSETV},
                {61, LuaJitOpCode.TSETS},
                {62, LuaJitOpCode.TSETB},
                {63, LuaJitOpCode.TSETM},
                {64, LuaJitOpCode.TSETR},
                {65, LuaJitOpCode.CALLM},
                {66, LuaJitOpCode.CALL},
                {67, LuaJitOpCode.CALLMT},
                {68, LuaJitOpCode.CALLT},
                {69, LuaJitOpCode.ITERC},
                {70, LuaJitOpCode.ITERN},
                {71, LuaJitOpCode.VARG},
                {72, LuaJitOpCode.ISNEXT},
                {73, LuaJitOpCode.RETM},
                {74, LuaJitOpCode.RET},
                {75, LuaJitOpCode.RET0},
                {76, LuaJitOpCode.RET1},
                {77, LuaJitOpCode.FORI},
                {78, LuaJitOpCode.JFORI},
                {79, LuaJitOpCode.FORL},
                {80, LuaJitOpCode.IFORL},
                {81, LuaJitOpCode.JFORL},
                {82, LuaJitOpCode.ITERL},
                {83, LuaJitOpCode.IITERL},
                {84, LuaJitOpCode.JITERL},
                {85, LuaJitOpCode.LOOP},
                {86, LuaJitOpCode.ILOOP},
                {87, LuaJitOpCode.JLOOP},
                {88, LuaJitOpCode.JMP},
                {89, LuaJitOpCode.FUNCF},
                {90, LuaJitOpCode.IFUNCF},
                {91, LuaJitOpCode.JFUNCF},
                {92, LuaJitOpCode.FUNCV},
                {93, LuaJitOpCode.IFUNCV},
                {94, LuaJitOpCode.JFUNCV},
                {95, LuaJitOpCode.FUNCC},
                {96, LuaJitOpCode.FUNCCW},
                {97, LuaJitOpCode.UNKNW}
            };
        }
    }
}