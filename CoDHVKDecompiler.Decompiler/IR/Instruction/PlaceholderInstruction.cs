namespace CoDHVKDecompiler.Decompiler.IR.Instruction
{
    public class PlaceholderInstruction : IInstruction
    {
        public string Placeholder { get; }

        public PlaceholderInstruction(string placeholder)
        {
            Placeholder = placeholder;
        }

        public override string ToString()
        {
            return Placeholder;
        }
    }
}