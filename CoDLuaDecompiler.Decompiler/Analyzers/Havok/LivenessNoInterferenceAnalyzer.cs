using System;
using System.Collections.Generic;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    /// <summary>
    /// Does global liveness analysis to verify no copies are needed coming out of SSA form
    /// </summary>
    public class LivenessNoInterferenceAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            // Just computes liveout despite the name
            f.ComputeGlobalLiveness(f.SSAVariables);

            var globalLiveness = new Dictionary<Identifier, HashSet<Identifier>>();
            // Initialise the disjoint sets
            foreach (var id in f.SSAVariables)
            {
                globalLiveness.Add(id, new HashSet<Identifier>() { id });
            }

            // Do a super shitty unoptimal union find algorithm to merge all the global ranges using phi functions
            // Rewrite this with a proper union-find if performance becomes an issue (lol)
            foreach (var b in f.Blocks)
            {
                foreach (var phi in b.PhiFunctions.Values)
                {
                    foreach (var r in phi.Right)
                    {
                        if (phi != null && phi.Left != null && r != null && globalLiveness.ContainsKey(phi.Left) && globalLiveness.ContainsKey(r) && globalLiveness[phi.Left] != globalLiveness[r])
                        {
                            globalLiveness[phi.Left].UnionWith(globalLiveness[r]);
                            globalLiveness[r] = globalLiveness[phi.Left];
                        }
                    }
                }
            }

            foreach (var b in f.Blocks)
            {
                var liveNow = new HashSet<Identifier>(b.LiveOut);
                for (int i = b.Instructions.Count - 1; i >= 0; i--)
                {
                    var defs = b.Instructions[i].GetDefines(true);
                    foreach (var def in defs)
                    {
                        foreach (var live in liveNow)
                        {
                            if (live != def && live.OriginalIdentifier == def.OriginalIdentifier)
                            {
#if DEBUG
                                Console.WriteLine($@"Warning: SSA live range interference detected in function {f.Id}. {live.OriginalIdentifier} Results are probably wrong.");
#endif
                            }
                        }
                        liveNow.Remove(def);
                    }
                    foreach (var use in b.Instructions[i].GetUses(true))
                    {
                        liveNow.Add(use);
                    }
                }
            }
        }
    }
}