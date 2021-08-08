namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaOpCodeTable.LuaJit
{
    public enum LuaJitArgumentType
    {
        Var = 0,    // Variable slot number
        Dst = 1,    // Variable slot number, used as a destination
        Bs = 2,     // Base slot number, read-write
        Rbs = 3,    // Base slot number, read-only
        Uv = 4,     // Upvalue slot number
        Lit = 5,    // Literal
        Slit = 6,   // Signed literal
        Pri = 7,    // Primitive type (0 = nil, 1 = false, 2 = true)
        Num = 8,    // Numeric constant, index into constant table
        Str = 9,    // String constant, negated index into constant table
        Tab = 10,   // Template table, negated index into constant table
        Fun = 11,   // Function prototype, negated index into constant table
        Cdt = 12,   // Cdata constant, negated index into constant table
        Jmp = 13,   // Branch target, relative to next instruction, biased with 0x8000
        None
    }
}