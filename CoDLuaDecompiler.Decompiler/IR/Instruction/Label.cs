namespace CoDLuaDecompiler.Decompiler.IR.Instruction
{
    /// <summary>
    /// A label that represents a jump target
    /// </summary>
    public class Label : IInstruction
    {
        /// <summary>
        /// Used to generate unique label names
        /// </summary>
        public static int LabelCount = 0;
        public string Name { get; set; }
        /// <summary>
        /// How many instructions use this label. Used to delete labels in some optimizations
        /// </summary>
        public int UsageCount { get; set; }
        
        public Label()
        {
            Name = $@"Label_{LabelCount}";
            LabelCount++;
        }
        
        public override string ToString()
        {
            return $@"{Name}:";
        }
    }
}