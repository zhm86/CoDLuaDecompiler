using System;
using CoDHVKDecompiler.Decompiler.IR.Functions;
using CoDHVKDecompiler.Decompiler.IR.Instruction;

namespace CoDHVKDecompiler.Decompiler.Analyzers
{
    /// <summary>
    /// Make sure every jump has a defined destination
    /// </summary>
    public class ControlFlowIntegrityAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            for (int i = 0; i < f.Instructions.Count; i++)
            {
                if (f.Instructions[i] is Jump j && f.Instructions.IndexOf(j.Dest) == -1)
                {
                    throw new Exception($"Control flow is corrupted in function {f.Id} @ instruction {i}");
                }
            }
        }
    }
}