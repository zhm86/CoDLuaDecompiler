using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.IR.Identifiers
{
    public class Identifier
    {
        public string Name { get; set; }
        public IdentifierType IdentifierType { get; set; }
        public ValueType ValueType { get; set; }
        public int ConstantId { get; set; }
        public Identifier OriginalIdentifier { get; set; }
        public uint RegNum { get; set; }
        public IInstruction DefiningInstruction { get; set; }
        public int UseCount { get; set; } = 0;
        public int PhiUseCount { get; set; } = 0;
        public bool UpValueResolved { get; set; } = false;
        public bool IsClosureBound { get; set; } = false;
        
        public override string ToString()
        {
            if (IdentifierType == IdentifierType.Varargs)
            {
                return "...";
            }
            return Name;
        }
    }
}