using System.Collections.Generic;
using System.Linq;
using CoDHVKDecompiler.Decompiler.CFG;
using CoDHVKDecompiler.Decompiler.IR.Functions;
using CoDHVKDecompiler.Decompiler.IR.Identifiers;
using CoDHVKDecompiler.Decompiler.IR.Instruction;

namespace CoDHVKDecompiler.Decompiler.Analyzers
{
    public class ConvertToSsaAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            var allRegisters = f.SymbolTable.GetAllRegistersInScope();
            allRegisters.UnionWith(new HashSet<Identifier>(f.Parameters));
            f.ComputeDominance();
            ComputeDominanceFrontier(f);
            f.ComputeGlobalLiveness(allRegisters);
            
            // Now insert all the needed phi functions
            foreach (var g in f.GlobalIdentifiers)
            {
                var work = new Queue<CFG.BasicBlock>();
                var visitedSet = new HashSet<CFG.BasicBlock>();
                foreach (var b in f.Blocks)
                {
                    if (b != f.EndBlock && b.KilledIdentifiers.Contains(g))
                    {
                        work.Enqueue(b);
                        visitedSet.Add(b);
                    }
                }
                while (work.Any())
                {
                    var b = work.Dequeue();
                    foreach (var d in b.DominanceFrontier)
                    {
                        if (d != f.EndBlock && !d.PhiFunctions.ContainsKey(g))
                        {
                            // Heuristic: if the block is just a single return, we don't need phi functions
                            if (d.Instructions.Any() && d.Instructions.First() is Return r && !r.Expressions.Any())
                            {
                                continue;
                            }

                            var phiargs = new List<Identifier>();
                            for (int i = 0; i < d.Predecessors.Count(); i++)
                            {
                                phiargs.Add(g);
                            }
                            d.PhiFunctions.Add(g, new PhiFunction(g, phiargs));
                            //if (!visitedSet.Contains(d))
                            //{
                                work.Enqueue(d);
                                visitedSet.Add(d);
                            //}
                        }
                    }
                }
            }



            // Prepare for renaming
            var counters = new Dictionary<Identifier, int>();
            var stacks = new Dictionary<Identifier, Stack<Identifier>>();
            foreach (var reg in allRegisters)
            {
                counters.Add(reg, 0);
                stacks.Add(reg, new Stack<Identifier>());
            }

            // Creates a new identifier based on an original identifier
            Identifier NewName(Identifier orig)
            {
                var newName = new Identifier();
                newName.Name = orig.Name + $@"_{counters[orig]}";
                newName.IdentifierType = IdentifierType.Register;
                newName.OriginalIdentifier = orig;
                newName.IsClosureBound = orig.IsClosureBound;
                stacks[orig].Push(newName);
                counters[orig]++;
                f.SSAVariables.Add(newName);
                return newName;
            }

            void RenameBlock(CFG.BasicBlock b)
            {
                // Rewrite phi function definitions
                foreach (var phi in b.PhiFunctions)
                {
                    phi.Value.RenameDefines(phi.Key, NewName(phi.Key));
                }

                // Rename other instructions
                foreach (var inst in b.Instructions)
                {
                    foreach (var use in inst.GetUses(true))
                    {
                        if (use.IsClosureBound)
                        {
                            continue;
                        }
                        if (stacks[use].Count != 0)
                        {
                            inst.RenameUses(use, stacks[use].Peek());
                        }
                    }
                    foreach (var def in inst.GetDefines(true))
                    {
                        inst.RenameDefines(def, NewName(def));
                    }
                }
                
                // Rename successor phi functions
                foreach (var succ in b.Successors)
                {
                    if (succ != f.EndBlock)
                    {
                        var index = succ.Predecessors.IndexOf(b);
                        foreach (var phi in succ.PhiFunctions)
                        {
                            if (stacks[phi.Value.Right[index]].Any())
                            {
                                phi.Value.Right[index] = stacks[phi.Value.Right[index]].Peek();
                                phi.Value.Right[index].UseCount++;
                            }
                            else
                            {
                                // Sometimes a phi function is forced when one of the predecessor paths don't actually define the register.
                                // These phi functions are usually not needed and optimized out in a later pass, so we set it to null to detect
                                // errors in case the phi function result is actually used.
                                phi.Value.Right[index] = null;
                            }
                        }
                    }
                }
                
                // Rename successors in the dominator tree
                foreach (var succ in b.DominanceTreeSuccessors)
                {
                    if (succ != f.EndBlock)
                    {
                        RenameBlock(succ);
                    }
                    
                    // Add to the scope killed set based on the domtree successor's killed and scope killed
                    foreach (var killed in succ.KilledIdentifiers)
                    {
                        if (killed.IdentifierType == IdentifierType.Register)
                        {
                            b.ScopeKilled.Add(killed.RegNum);
                        }
                        b.ScopeKilled.UnionWith(succ.ScopeKilled);
                    }
                }

                // Pop off anything we pushed
                foreach (var phi in b.PhiFunctions)
                {
                    stacks[phi.Value.Left.OriginalIdentifier].Pop();
                }
                foreach (var inst in b.Instructions)
                {
                    foreach (var def in inst.GetDefines(true))
                    {
                        stacks[def.OriginalIdentifier].Pop();
                    }
                }
            }

            // Rename the arguments first
            for (int i = 0; i < f.Parameters.Count(); i++)
            {
                f.Parameters[i] = NewName(f.Parameters[i]);
            }

            // Rename everything else recursively
            RenameBlock(f.StartBlock);
        }

        private void ComputeDominanceFrontier(Function f)
        {
            for (int i = 0; i < f.Blocks.Count(); i++)
            {
                if (f.Blocks[i].Predecessors.Count() > 1)
                {
                    foreach (var p in f.Blocks[i].Predecessors)
                    {
                        var runner = p;
                        while (runner != f.Blocks[i].ImmediateDominator)
                        {
                            runner.DominanceFrontier.UnionWith(new[] { f.Blocks[i] });
                            runner = runner.ImmediateDominator;
                        }
                    }
                }
            }
        }
    }
}