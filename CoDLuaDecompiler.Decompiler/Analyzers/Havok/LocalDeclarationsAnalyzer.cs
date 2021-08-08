using System.Collections.Generic;
using System.Linq;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    /// <summary>
    /// Detects and annotates declarations of local variables. These are the first definitions of variables
    /// in a dominance heirarchy.
    /// </summary>
    public class LocalDeclarationsAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            // This is kinda both a pre and post-order traversal of the dominance heirarchy. In the pre traversal,
            // first local definitions are detected, marked, and propogated down the graph so that they aren't marked
            // again. In the postorder traversal, these marked definitions are backpropogated up the dominance heirarchy.
            // If a node gets multiple marked nodes for the same variable from its children in the dominance heirarchy,
            // a new local assignment must be inserted right before the node splits.
            Dictionary<Identifier, List<Assignment>> visit(CFG.BasicBlock b, HashSet<Identifier> declared)
            {
                var newdeclared = new HashSet<Identifier>(declared);
                var declaredAssignments = new Dictionary<Identifier, List<Assignment>>();

                // Go through the graph and mark declared nodes
                foreach (var inst in b.Instructions)
                {
                    if (inst is Assignment a && a.Left.Count() > 0)
                    {
                        foreach (var def in a.GetDefines(true))
                        {
                            if (!newdeclared.Contains(def))
                            {
                                newdeclared.Add(def);
                                a.IsLocalDeclaration = true;
                                declaredAssignments.Add(def, new List<Assignment>() { a });
                            }
                        }
                    }
                }

                // Visit and merge the children in the dominance heirarchy
                var inherited = new Dictionary<Identifier, List<Assignment>>();
                var phiinduced = new HashSet<Identifier>();
                foreach (var succ in b.DominanceTreeSuccessors)
                {
                    var cdeclared = visit(succ, newdeclared);
                    foreach (var entry in cdeclared)
                    {
                        if (!inherited.ContainsKey(entry.Key))
                        {
                            inherited.Add(entry.Key, new List<Assignment>(entry.Value));
                        }
                        else
                        {
                            inherited[entry.Key].AddRange(entry.Value);
                        }
                    }
                    phiinduced.UnionWith(succ.PhiMerged);
                }
                foreach (var entry in inherited)
                {
                    if (entry.Value.Count() > 1 && phiinduced.Contains(entry.Key))
                    {
                        // Multiple incoming declarations that all have the same use need to be merged
                        var assn = new Assignment(entry.Key, null);
                        assn.IsLocalDeclaration = true;
                        b.Instructions.Insert(b.Instructions.Count() - 1, assn);
                        declaredAssignments.Add(entry.Key, new List<Assignment>() { assn });
                        foreach (var e in entry.Value)
                        {
                            e.IsLocalDeclaration = false;
                        }
                    }
                    else
                    {
                        declaredAssignments.Add(entry.Key, entry.Value);
                    }
                }

                return declaredAssignments;
            }

            visit(f.StartBlock, new HashSet<Identifier>(f.Parameters));
        }
    }
}