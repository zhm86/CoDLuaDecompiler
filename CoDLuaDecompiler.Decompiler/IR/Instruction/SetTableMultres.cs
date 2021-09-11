using CoDLuaDecompiler.Decompiler.IR.Identifiers;

namespace CoDLuaDecompiler.Decompiler.IR.Instruction
{
    public class SetTableMultres : IInstruction
    {
        public Identifier TableIdentifier { get; set; }
        public int Index { get; set; }

        public SetTableMultres(Identifier tableIdentifier, int index)
        {
            TableIdentifier = tableIdentifier;
            Index = index;
        }
    }
}