namespace CoDLuaDecompiler.Decompiler.IR.Identifiers
{
    public enum ValueType : byte
    {
        Unknown,
        Number,
        Boolean,
        String,
        Table,
        Closure,
        Nil,
        Hash,
        VarArgs
    }
}