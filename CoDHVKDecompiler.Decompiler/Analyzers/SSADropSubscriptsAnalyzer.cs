using System.Collections.Generic;
using System.Linq;
using CoDHVKDecompiler.Decompiler.IR.Functions;
using CoDHVKDecompiler.Decompiler.IR.Identifiers;
using CoDHVKDecompiler.Decompiler.IR.Instruction;

namespace CoDHVKDecompiler.Decompiler.Analyzers
{
    /// <summary>
    /// Drop SSA by simply dropping the subscripts. This requires no interfence in the live ranges of the definitions
    /// </summary>
    public class SSADropSubscriptsAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            foreach (var b in f.Blocks)
            {
                foreach (var phi in b.PhiFunctions)
                {
                    b.PhiMerged.Add(phi.Value.Left.OriginalIdentifier);
                }
                b.PhiFunctions.Clear();
                foreach (var i in b.Instructions)
                {
                    foreach (var def in i.GetDefines(true))
                    {
                        if (def.OriginalIdentifier != null)
                            i.RenameDefines(def, def.OriginalIdentifier);
                    }
                    foreach (var use in i.GetUses(true))
                    {
                        if (use.OriginalIdentifier != null)
                            i.RenameUses(use, use.OriginalIdentifier);
                    }
                }
            }
            for (int a = 0; a < f.Parameters.Count(); a++)
            {
                if (f.Parameters[a].OriginalIdentifier != null)
                    f.Parameters[a] = f.Parameters[a].OriginalIdentifier;
            }

            int counter = 0;
            Identifier NewName(Identifier orig)
            {
                var newName = new Identifier();
                newName.Name = orig.Name + $@"_{counter}";
                counter++;
                newName.IdentifierType = IdentifierType.Register;
                newName.OriginalIdentifier = orig;
                return newName;
            }

            // If we have debug information, we can split up variables based on if a definition is associated with the start
            // of a local variable. If so, everything dominated by the containing block is renamed to that definition
            void visit(CFG.BasicBlock b, Dictionary<Identifier, Identifier> replacements)
            {
                var newreplacements = new Dictionary<Identifier, Identifier>(replacements);

                bool changed = true;
                while (changed)
                {
                    changed = false;
                    foreach (var phi in b.PhiMerged.ToList())
                    {
                        if (newreplacements.ContainsKey(phi))
                        {
                            b.PhiMerged.Remove(phi);
                            b.PhiMerged.Add(newreplacements[phi]);
                            changed = true;
                        }
                    }
                }
                // Walk down all the instructions, replacing things that need to be replaced and renaming as needed
                foreach (var instruction in b.Instructions)
                {
                    changed = true;
                    bool reassigned = false;
                    Identifier newdef = null;
                    while (changed)
                    {
                        changed = false;
                        foreach (var use in instruction.GetUses(true))
                        {
                            if (newreplacements.ContainsKey(use) && newreplacements[use] != newdef)
                            {
                                instruction.RenameUses(use, newreplacements[use]);
                                changed = true;
                            }
                        }
                        foreach (var def in instruction.GetDefines(true))
                        {
                            /*if (instruction is Assignment a && a.LocalAssignments != null && !reassigned)
                            {
                                var newname = NewName(def);
                                instruction.RenameDefines(def, newname);
                                if (newreplacements.ContainsKey(def))
                                {
                                    newreplacements[def] = newname;
                                    newdef = newname;
                                }
                                else
                                {
                                    newreplacements.Add(def, newname);
                                    newdef = newname;
                                }
                                changed = true;
                                reassigned = true;
                            }
                            else*/ if (newreplacements.ContainsKey(def))
                            {
                                instruction.RenameDefines(def, newreplacements[def]);
                                changed = true;
                            }
                        }
                    }
                }

                // Propogate to children in the dominance heirarchy
                foreach (var succ in b.DominanceTreeSuccessors)
                {
                    visit(succ, newreplacements);
                }
            }
            visit(f.StartBlock, new Dictionary<Identifier, Identifier>());
        }
    }
}