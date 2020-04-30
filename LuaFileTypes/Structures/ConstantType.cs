namespace DSLuaDecompiler.LuaFileTypes.Structures
{
    public enum ConstantType : byte
    {
        TNil,
        TBoolean,
        TLightUserData,
        TNumber,
        TString,
        TTable,
        TFunction,
        TUserData,
        TThread,
        TIFunction,
        TCFunction,
        TUI64,
        TStruct,
        THash,
        TUnk,
    }
}