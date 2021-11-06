using System.Linq;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    public class DebugLineInfoAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            if (f.FunctionDebugInfo == null)
                return;

            AddLines(f);
        }

        public void AddLines(Function f)
        {
            f.Closures.ForEach(AddLines);
            for (int bi = 0; bi < f.Blocks.Count; bi++)
            {
                var b = f.Blocks[bi];
                if (b.Instructions.Count <= 1)
                    continue;
                var prevLine = b.Instructions[0].LineLocation == 0 && bi > 0 ? b.Instructions[1].LineLocation - 1 : b.Instructions[0].LineLocation;
                var prevLength = b.Instructions[0].ToString()!.Count(s => s == '\n') + 1;
                for (int i = 1; i < b.Instructions.Count; i++)
                {
                    var instr = b.Instructions[i];
                    for (int j = 0; j < instr.LineLocation - prevLine - prevLength; j++)
                    {
                        b.Instructions.Insert(i, new NewLine());
                        i++;
                    }

                    prevLine = instr.LineLocation == 0 ? prevLine + 1 : instr.LineLocation;
                    prevLength = instr.ToString()!.Count(s => s == '\n') + 1;
                }
            }
        }
    }
}