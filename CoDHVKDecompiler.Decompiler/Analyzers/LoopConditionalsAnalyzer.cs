using System.Collections.Generic;
using System.Linq;
using CoDHVKDecompiler.Decompiler.CFG;
using CoDHVKDecompiler.Decompiler.IR.Functions;
using CoDHVKDecompiler.Decompiler.IR.Instruction;

namespace CoDHVKDecompiler.Decompiler.Analyzers
{
    /// <summary>
    /// If conditional structuring won't detect if statements that lead to a break or continue. This pass aims to identify and structure those
    /// </summary>
    public class LoopConditionalsAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            var visited = new HashSet<BasicBlock>();
            void Visit(BasicBlock b, BasicBlock loopHead)
            {
                visited.Add(b);
                var lhead = loopHead;
                if (b.IsLoopHead)
                {
                    lhead = b;
                }
                foreach (var succ in b.Successors)
                {
                    if (!visited.Contains(succ))
                    {
                        Visit(succ, lhead);
                    }
                }

                // Detect unstructured if statements
                if (lhead != null && b.Successors.Count() == 2 && b.Instructions.Last() is Jump jmp && !(b.IsLoopHead && b.LoopType == CFG.LoopType.LoopPretested))
                {
                    // An if statement is unstructured but recoverable if it has a forward edge to the loop follow (break) or head (continue) on the left or right
                    bool isBreak = false;
                    bool isContinue = false;
                    foreach (var succ in b.DominanceTreeSuccessors)
                    {
                        if (succ.IsLoopLatch)
                        {
                            continue;
                        }

                        // Mark breaks
                        if (succ.Successors.Contains(lhead.LoopFollow))
                        {
                            succ.IsBreakNode = true;
                            b.LoopBreakFollow = lhead.LoopFollow;
                        }
                        // Mark continues
                        if (succ.Successors.Contains(lhead))
                        {
                            succ.IsContinueNode = true;
                            b.LoopContinueFollow = lhead.LoopContinueFollow;
                        }
                    }
                }
            }
            Visit(f.StartBlock, null);
        }
    }
}