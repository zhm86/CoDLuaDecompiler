using System.Collections.Generic;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    public class DeadAssignmentsAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            bool changed = true;
            while (changed)
            {
                changed = false;
                var usageCounts = new Dictionary<Identifier, int>();
                foreach (var arg in f.Parameters)
                {
                    usageCounts.Add(arg, 0);
                }

                // Used for phi function cycle detection
                var singleUses = new Dictionary<Identifier, PhiFunction>();

                // Do a reverse-postorder traversal to register all the definitions and uses
                foreach (var b in f.PostorderTraversal(true))
                {
                    // Defines/uses in phi functions
                    foreach (var phi in b.PhiFunctions)
                    {
                        var usedUses = new HashSet<Identifier>();
                        foreach (var use in phi.Value.Right)
                        {
                            // If a phi function has multiple uses of the same identifier, only count it as one use for the purposes of this analysis
                            if (use != null && !usedUses.Contains(use))
                            {
                                if (!usageCounts.ContainsKey(use))
                                {
                                    usageCounts.Add(use, 0);
                                }
                                usageCounts[use]++;
                                if (usageCounts[use] == 1)
                                {
                                    if (singleUses.ContainsKey(use))
                                    {
                                        singleUses.Remove(use);
                                    }
                                    else
                                    {
                                        singleUses.Add(use, phi.Value);
                                    }
                                }
                                else
                                {
                                    if (singleUses.ContainsKey(use))
                                    {
                                        singleUses.Remove(use);
                                    }
                                }
                                usedUses.Add(use);
                            }
                        }
                        if (!usageCounts.ContainsKey(phi.Value.Left))
                        {
                            usageCounts.Add(phi.Value.Left, 0);
                        }
                    }

                    // Defines/uses for everything else
                    foreach (var inst in b.Instructions)
                    {
                        foreach (var use in inst.GetUses(true))
                        {
                            if (!usageCounts.ContainsKey(use))
                            {
                                usageCounts.Add(use, 0);
                            }
                            usageCounts[use]++;
                        }
                        foreach (var def in inst.GetDefines(true))
                        {
                            if (!usageCounts.ContainsKey(def))
                            {
                                usageCounts.Add(def, 0);
                            }
                        }
                    }
                }

                // Do an elimination pass
                foreach (var b in f.Blocks)
                {
                    // Eliminate unused phi functions
                    var phiToRemove = new List<Identifier>();
                    foreach (var phi in b.PhiFunctions)
                    {
                        if (usageCounts[phi.Value.Left] == 0)
                        {
                            changed = true;
                            phiToRemove.Add(phi.Value.Left);
                        }

                        // If this phi function has a single use, which is also a phi function, and that phi function has
                        // a single use, which is this phi function, then we have a useless phi function dependency cycle
                        // that can be broken and removed
                        if (singleUses.ContainsKey(phi.Value.Left) && singleUses.ContainsKey(singleUses[phi.Value.Left].Left) &&
                                singleUses[singleUses[phi.Value.Left].Left] == phi.Value)
                        {
                            changed = true;
                            phiToRemove.Add(phi.Value.Left);
                            singleUses[phi.Value.Left].RenameUses(phi.Value.Left, null);
                        }
                    }
                    foreach (var rem in phiToRemove)
                    {
                        foreach (var i in b.PhiFunctions[rem.OriginalIdentifier].Right)
                        {
                            if (i != null)
                            {
                                i.UseCount--;
                            }
                        }
                    }
                    phiToRemove.ForEach(x => b.PhiFunctions.Remove(x.OriginalIdentifier));

                    /*// Eliminate unused assignments
                    var toRemove = new List<IInstruction>();
                    foreach (var inst in b.Instructions)
                    {
                        var defs = inst.GetDefines(true);
                        if (defs.Count == 1 && usageCounts[defs.First()] == 0)
                        {
                            if (inst is Assignment a && a.Right is FunctionCall && !phiOnly)
                            {
                                a.Left.Clear();
                            }
                            else
                            {
                                if (!phiOnly)
                                {
                                    changed = true;
                                    toRemove.Add(inst);
                                }
                            }
                        }
                    }
                    toRemove.ForEach(x => b.Instructions.Remove(x));*/
                }
            }
        }
    }
}