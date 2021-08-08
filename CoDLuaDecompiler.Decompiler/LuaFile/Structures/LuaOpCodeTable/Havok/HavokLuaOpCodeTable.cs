using System;
using System.Collections.Generic;

namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaOpCodeTable.Havok
{
    public abstract class HavokLuaOpCodeTable : IHavokLuaOpCodeTable
    {
        protected Dictionary<int, LuaHavokOpCode> OpCodeTable;

        protected HavokLuaOpCodeTable()
        {
            Parse();
        }

        private void Parse()
        {
            OpCodeTable = ParseLuaOpCodes();
        }

        protected abstract Dictionary<int, LuaHavokOpCode> ParseLuaOpCodes();

        public LuaHavokOpCode GetValue(int parsedOpCode)
        {
            return OpCodeTable[parsedOpCode];
        }

        public bool TryGetValue(int parsedOpCode, out LuaHavokOpCode luaHavokOpCode)
        {
            try
            {
                luaHavokOpCode = GetValue(parsedOpCode);
                return true;
            }
            catch (Exception)
            {
                // ignored
            }

            luaHavokOpCode = LuaHavokOpCode.HKS_OPCODE_UNK;
            return false;
        }
    }
}