namespace CoDLuaDecompiler.Decompiler.LuaFile.Structures.LuaConstant.Havok
{
    public abstract class IHavokLuaConstant
    {
        public HavokConstantType Type { get; protected set; }
        public string StringValue { get; protected set; }
        public double NumberValue { get; protected set; }
        public bool BoolValue { get; protected set; }
        public ulong HashValue { get; protected set; }
    }
}