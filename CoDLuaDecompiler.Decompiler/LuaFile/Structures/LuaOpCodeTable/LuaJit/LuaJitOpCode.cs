namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaOpCodeTable.LuaJit
{
    public static class LuaJitOpCode
    {
        // Comparison ops
        public static LuaJitOpCodeDef ISLT = new LuaJitOpCodeDef("ISLT", LuaJitArgumentType.Var, LuaJitArgumentType.None, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef ISGE = new LuaJitOpCodeDef("ISGE", LuaJitArgumentType.Var, LuaJitArgumentType.None, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef ISLE = new LuaJitOpCodeDef("ISLE", LuaJitArgumentType.Var, LuaJitArgumentType.None, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef ISGT = new LuaJitOpCodeDef("ISGT", LuaJitArgumentType.Var, LuaJitArgumentType.None, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef ISEQV = new LuaJitOpCodeDef("ISEQV", LuaJitArgumentType.Var, LuaJitArgumentType.None, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef ISNEV = new LuaJitOpCodeDef("ISNEV", LuaJitArgumentType.Var, LuaJitArgumentType.None, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef ISEQS = new LuaJitOpCodeDef("ISEQS", LuaJitArgumentType.Var, LuaJitArgumentType.None, LuaJitArgumentType.Str);
        public static LuaJitOpCodeDef ISNES = new LuaJitOpCodeDef("ISNES", LuaJitArgumentType.Var, LuaJitArgumentType.None, LuaJitArgumentType.Str);
        public static LuaJitOpCodeDef ISEQN = new LuaJitOpCodeDef("ISEQN", LuaJitArgumentType.Var, LuaJitArgumentType.None, LuaJitArgumentType.Num);
        public static LuaJitOpCodeDef ISNEN = new LuaJitOpCodeDef("ISNEN", LuaJitArgumentType.Var, LuaJitArgumentType.None, LuaJitArgumentType.Num);
        public static LuaJitOpCodeDef ISEQP = new LuaJitOpCodeDef("ISEQP", LuaJitArgumentType.Var, LuaJitArgumentType.None, LuaJitArgumentType.Pri);
        public static LuaJitOpCodeDef ISNEP = new LuaJitOpCodeDef("ISNEP", LuaJitArgumentType.Var, LuaJitArgumentType.None, LuaJitArgumentType.Pri);

        // Unary test and copy ops
        public static LuaJitOpCodeDef ISTC = new LuaJitOpCodeDef("ISTC", LuaJitArgumentType.Dst, LuaJitArgumentType.None, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef ISFC = new LuaJitOpCodeDef("ISFC", LuaJitArgumentType.Dst, LuaJitArgumentType.None, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef IST = new LuaJitOpCodeDef("IST", LuaJitArgumentType.None, LuaJitArgumentType.None, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef ISF = new LuaJitOpCodeDef("ISF", LuaJitArgumentType.None, LuaJitArgumentType.None, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef ISTYPE = new LuaJitOpCodeDef("ISTYPE", LuaJitArgumentType.Var, LuaJitArgumentType.None, LuaJitArgumentType.Lit);
        public static LuaJitOpCodeDef ISNUM = new LuaJitOpCodeDef("ISNUM", LuaJitArgumentType.Var, LuaJitArgumentType.None, LuaJitArgumentType.Lit);

        // Unary ops
        public static LuaJitOpCodeDef MOV = new LuaJitOpCodeDef("MOV", LuaJitArgumentType.Dst, LuaJitArgumentType.None, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef NOT = new LuaJitOpCodeDef("NOT", LuaJitArgumentType.Dst, LuaJitArgumentType.None, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef UNM = new LuaJitOpCodeDef("UNM", LuaJitArgumentType.Dst, LuaJitArgumentType.None, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef LEN = new LuaJitOpCodeDef("LEN", LuaJitArgumentType.Dst, LuaJitArgumentType.None, LuaJitArgumentType.Var);

        // Binary ops
        public static LuaJitOpCodeDef ADDVN = new LuaJitOpCodeDef("ADDVN", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Num);
        public static LuaJitOpCodeDef SUBVN = new LuaJitOpCodeDef("SUBVN", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Num);
        public static LuaJitOpCodeDef MULVN = new LuaJitOpCodeDef("MULVN", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Num);
        public static LuaJitOpCodeDef DIVVN = new LuaJitOpCodeDef("DIVVN", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Num);
        public static LuaJitOpCodeDef MODVN = new LuaJitOpCodeDef("MODVN", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Num);
        public static LuaJitOpCodeDef ADDNV = new LuaJitOpCodeDef("ADDNV", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Num);
        public static LuaJitOpCodeDef SUBNV = new LuaJitOpCodeDef("SUBNV", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Num);
        public static LuaJitOpCodeDef MULNV = new LuaJitOpCodeDef("MULNV", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Num);
        public static LuaJitOpCodeDef DIVNV = new LuaJitOpCodeDef("DIVNV", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Num);
        public static LuaJitOpCodeDef MODNV = new LuaJitOpCodeDef("MODNV", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Num);
        public static LuaJitOpCodeDef ADDVV = new LuaJitOpCodeDef("ADDVV", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef SUBVV = new LuaJitOpCodeDef("SUBVV", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef MULVV = new LuaJitOpCodeDef("MULVV", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef DIVVV = new LuaJitOpCodeDef("DIVVV", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef MODVV = new LuaJitOpCodeDef("MODVV", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef POW = new LuaJitOpCodeDef("POW", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef CAT = new LuaJitOpCodeDef("CAT", LuaJitArgumentType.Dst, LuaJitArgumentType.Rbs, LuaJitArgumentType.Rbs);

        // Constant ops
        public static LuaJitOpCodeDef KSTR = new LuaJitOpCodeDef("KSTR", LuaJitArgumentType.Dst, LuaJitArgumentType.None, LuaJitArgumentType.Str);
        public static LuaJitOpCodeDef KHASH = new LuaJitOpCodeDef("KHASH", LuaJitArgumentType.Dst, LuaJitArgumentType.None, LuaJitArgumentType.Str);
        public static LuaJitOpCodeDef KCDATA = new LuaJitOpCodeDef("KCDATA", LuaJitArgumentType.Dst, LuaJitArgumentType.None, LuaJitArgumentType.Cdt);
        public static LuaJitOpCodeDef KSHORT = new LuaJitOpCodeDef("KSHORT", LuaJitArgumentType.Dst, LuaJitArgumentType.None, LuaJitArgumentType.Slit);
        public static LuaJitOpCodeDef KNUM = new LuaJitOpCodeDef("KNUM", LuaJitArgumentType.Dst, LuaJitArgumentType.None, LuaJitArgumentType.Num);
        public static LuaJitOpCodeDef KPRI = new LuaJitOpCodeDef("KPRI", LuaJitArgumentType.Dst, LuaJitArgumentType.None, LuaJitArgumentType.Pri);
        public static LuaJitOpCodeDef KNIL = new LuaJitOpCodeDef("KNIL", LuaJitArgumentType.Bs, LuaJitArgumentType.None, LuaJitArgumentType.Bs);

        // Upvalue and function ops
        public static LuaJitOpCodeDef UGET = new LuaJitOpCodeDef("UGET", LuaJitArgumentType.Dst, LuaJitArgumentType.None, LuaJitArgumentType.Uv);
        public static LuaJitOpCodeDef USETV = new LuaJitOpCodeDef("USETV", LuaJitArgumentType.Uv, LuaJitArgumentType.None, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef USETS = new LuaJitOpCodeDef("USETS", LuaJitArgumentType.Uv, LuaJitArgumentType.None, LuaJitArgumentType.Str);
        public static LuaJitOpCodeDef USETN = new LuaJitOpCodeDef("USETN", LuaJitArgumentType.Uv, LuaJitArgumentType.None, LuaJitArgumentType.Num);
        public static LuaJitOpCodeDef USETP = new LuaJitOpCodeDef("USETP", LuaJitArgumentType.Uv, LuaJitArgumentType.None, LuaJitArgumentType.Pri);
        public static LuaJitOpCodeDef UCLO = new LuaJitOpCodeDef("UCLO", LuaJitArgumentType.Rbs, LuaJitArgumentType.None, LuaJitArgumentType.Jmp);
        public static LuaJitOpCodeDef FNEW = new LuaJitOpCodeDef("FNEW", LuaJitArgumentType.Dst, LuaJitArgumentType.None, LuaJitArgumentType.Fun);

        // Table ops
        public static LuaJitOpCodeDef TNEW = new LuaJitOpCodeDef("TNEW", LuaJitArgumentType.Dst, LuaJitArgumentType.None, LuaJitArgumentType.Lit);
        public static LuaJitOpCodeDef TDUP = new LuaJitOpCodeDef("TDUP", LuaJitArgumentType.Dst, LuaJitArgumentType.None, LuaJitArgumentType.Tab);
        public static LuaJitOpCodeDef GGET = new LuaJitOpCodeDef("GGET", LuaJitArgumentType.Dst, LuaJitArgumentType.None, LuaJitArgumentType.Str);
        public static LuaJitOpCodeDef GSET = new LuaJitOpCodeDef("GSET", LuaJitArgumentType.Var, LuaJitArgumentType.None, LuaJitArgumentType.Str);
        public static LuaJitOpCodeDef TGETV = new LuaJitOpCodeDef("TGETV", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef TGETS = new LuaJitOpCodeDef("TGETS", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Str);
        public static LuaJitOpCodeDef TGETB = new LuaJitOpCodeDef("TGETB", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Lit);
        public static LuaJitOpCodeDef TGETR = new LuaJitOpCodeDef("TGETR", LuaJitArgumentType.Dst, LuaJitArgumentType.Var, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef TSETV = new LuaJitOpCodeDef("TSETV", LuaJitArgumentType.Var, LuaJitArgumentType.Var, LuaJitArgumentType.Var);
        public static LuaJitOpCodeDef TSETS = new LuaJitOpCodeDef("TSETS", LuaJitArgumentType.Var, LuaJitArgumentType.Var, LuaJitArgumentType.Str);
        public static LuaJitOpCodeDef TSETB = new LuaJitOpCodeDef("TSETB", LuaJitArgumentType.Var, LuaJitArgumentType.Var, LuaJitArgumentType.Lit);
        public static LuaJitOpCodeDef TSETM = new LuaJitOpCodeDef("TSETM", LuaJitArgumentType.Bs, LuaJitArgumentType.None, LuaJitArgumentType.Num);
        public static LuaJitOpCodeDef TSETR = new LuaJitOpCodeDef("TSETR", LuaJitArgumentType.Var, LuaJitArgumentType.Var, LuaJitArgumentType.Var);

        // Calls and vararg handling. T = tail call
        public static LuaJitOpCodeDef CALLM = new LuaJitOpCodeDef("CALLM", LuaJitArgumentType.Bs, LuaJitArgumentType.Lit, LuaJitArgumentType.Lit);
        public static LuaJitOpCodeDef CALL = new LuaJitOpCodeDef("CALL", LuaJitArgumentType.Bs, LuaJitArgumentType.Lit, LuaJitArgumentType.Lit);
        public static LuaJitOpCodeDef CALLMT = new LuaJitOpCodeDef("CALLMT", LuaJitArgumentType.Bs, LuaJitArgumentType.None, LuaJitArgumentType.Lit);
        public static LuaJitOpCodeDef CALLT = new LuaJitOpCodeDef("CALLT", LuaJitArgumentType.Bs, LuaJitArgumentType.None, LuaJitArgumentType.Lit);
        public static LuaJitOpCodeDef ITERC = new LuaJitOpCodeDef("ITERC", LuaJitArgumentType.Bs, LuaJitArgumentType.Lit, LuaJitArgumentType.Lit);
        public static LuaJitOpCodeDef ITERN = new LuaJitOpCodeDef("ITERN", LuaJitArgumentType.Bs, LuaJitArgumentType.Lit, LuaJitArgumentType.Lit);
        public static LuaJitOpCodeDef VARG = new LuaJitOpCodeDef("VARG", LuaJitArgumentType.Bs, LuaJitArgumentType.Lit, LuaJitArgumentType.Lit);
        public static LuaJitOpCodeDef ISNEXT = new LuaJitOpCodeDef("ISNEXT", LuaJitArgumentType.Bs, LuaJitArgumentType.None, LuaJitArgumentType.Jmp);

        // Returns
        public static LuaJitOpCodeDef RETM = new LuaJitOpCodeDef("RETM", LuaJitArgumentType.Bs, LuaJitArgumentType.None, LuaJitArgumentType.Lit);
        public static LuaJitOpCodeDef RET = new LuaJitOpCodeDef("RET", LuaJitArgumentType.Rbs, LuaJitArgumentType.None, LuaJitArgumentType.Lit);
        public static LuaJitOpCodeDef RET0 = new LuaJitOpCodeDef("RET0", LuaJitArgumentType.Rbs, LuaJitArgumentType.None, LuaJitArgumentType.Lit);
        public static LuaJitOpCodeDef RET1 = new LuaJitOpCodeDef("RET1", LuaJitArgumentType.Rbs, LuaJitArgumentType.None, LuaJitArgumentType.Lit);

        // Loops and branches. I/J = interp/JIT, I/C/L = init/call/loop
        public static LuaJitOpCodeDef FORI = new LuaJitOpCodeDef("FORI", LuaJitArgumentType.Bs, LuaJitArgumentType.None, LuaJitArgumentType.Jmp);
        public static LuaJitOpCodeDef JFORI = new LuaJitOpCodeDef("JFORI", LuaJitArgumentType.Bs, LuaJitArgumentType.None, LuaJitArgumentType.Jmp);
        public static LuaJitOpCodeDef FORL = new LuaJitOpCodeDef("FORL", LuaJitArgumentType.Bs, LuaJitArgumentType.None, LuaJitArgumentType.Jmp);
        public static LuaJitOpCodeDef IFORL = new LuaJitOpCodeDef("IFORL", LuaJitArgumentType.Bs, LuaJitArgumentType.None, LuaJitArgumentType.Jmp);
        public static LuaJitOpCodeDef JFORL = new LuaJitOpCodeDef("JFORL", LuaJitArgumentType.Bs, LuaJitArgumentType.None, LuaJitArgumentType.Jmp);
        public static LuaJitOpCodeDef ITERL = new LuaJitOpCodeDef("ITERL", LuaJitArgumentType.Bs, LuaJitArgumentType.None, LuaJitArgumentType.Jmp);
        public static LuaJitOpCodeDef IITERL = new LuaJitOpCodeDef("IITERL", LuaJitArgumentType.Bs, LuaJitArgumentType.None, LuaJitArgumentType.Jmp);
        public static LuaJitOpCodeDef JITERL = new LuaJitOpCodeDef("JITERL", LuaJitArgumentType.Bs, LuaJitArgumentType.None, LuaJitArgumentType.Lit);
        public static LuaJitOpCodeDef LOOP = new LuaJitOpCodeDef("LOOP", LuaJitArgumentType.Rbs, LuaJitArgumentType.None, LuaJitArgumentType.Jmp);
        public static LuaJitOpCodeDef ILOOP = new LuaJitOpCodeDef("ILOOP", LuaJitArgumentType.Rbs, LuaJitArgumentType.None, LuaJitArgumentType.Jmp);
        public static LuaJitOpCodeDef JLOOP = new LuaJitOpCodeDef("JLOOP", LuaJitArgumentType.Rbs, LuaJitArgumentType.None, LuaJitArgumentType.Lit);
        public static LuaJitOpCodeDef JMP = new LuaJitOpCodeDef("JMP", LuaJitArgumentType.Rbs, LuaJitArgumentType.None, LuaJitArgumentType.Jmp);

        // Function headers. I/J = interp/JIT, F/V/C = fixarg/vararg/C func
        // Shouldn't be ever seen - they are not stored in raw dump?
        public static LuaJitOpCodeDef FUNCF = new LuaJitOpCodeDef("FUNCF", LuaJitArgumentType.Rbs, LuaJitArgumentType.None, LuaJitArgumentType.None);
        public static LuaJitOpCodeDef IFUNCF = new LuaJitOpCodeDef("IFUNCF", LuaJitArgumentType.Rbs, LuaJitArgumentType.None, LuaJitArgumentType.None);
        public static LuaJitOpCodeDef JFUNCF = new LuaJitOpCodeDef("JFUNCF", LuaJitArgumentType.Rbs, LuaJitArgumentType.None, LuaJitArgumentType.Lit);
        public static LuaJitOpCodeDef FUNCV = new LuaJitOpCodeDef("FUNCV", LuaJitArgumentType.Rbs, LuaJitArgumentType.None, LuaJitArgumentType.None);
        public static LuaJitOpCodeDef IFUNCV = new LuaJitOpCodeDef("IFUNCV", LuaJitArgumentType.Rbs, LuaJitArgumentType.None, LuaJitArgumentType.None);
        public static LuaJitOpCodeDef JFUNCV = new LuaJitOpCodeDef("JFUNCV", LuaJitArgumentType.Rbs, LuaJitArgumentType.None, LuaJitArgumentType.Lit);
        public static LuaJitOpCodeDef FUNCC = new LuaJitOpCodeDef("FUNCC", LuaJitArgumentType.Rbs, LuaJitArgumentType.None, LuaJitArgumentType.None);
        public static LuaJitOpCodeDef FUNCCW = new LuaJitOpCodeDef("FUNCCW", LuaJitArgumentType.Rbs, LuaJitArgumentType.None, LuaJitArgumentType.None);
        public static LuaJitOpCodeDef UNKNW = new LuaJitOpCodeDef("UNKNW", LuaJitArgumentType.Lit, LuaJitArgumentType.Lit, LuaJitArgumentType.Lit);
    }
}