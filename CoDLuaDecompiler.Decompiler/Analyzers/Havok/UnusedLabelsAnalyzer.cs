using System.Collections.Generic;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    public class UnusedLabelsAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            var usedLabels = new List<Label>();
            var allLabelList = new Dictionary<int, Label>();
            for (int i = 0; i < f.Instructions.Count; i++)
            {
                if (f.Instructions[i] is Label l)
                {
                    allLabelList.Add(i, l);
                }

                if (f.Instructions[i] is Jump j)
                {
                    usedLabels.Add(j.Dest);
                }
            }
            
            foreach(var label in allLabelList)
            {
                if (!usedLabels.Contains(label.Value))
                {
                    f.Instructions.Remove(label.Value);
                }
            }
        }
    }
}