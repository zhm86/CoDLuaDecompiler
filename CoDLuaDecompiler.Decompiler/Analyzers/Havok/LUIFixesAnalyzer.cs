using CoDLuaDecompiler.Decompiler.IR.Expression;
using CoDLuaDecompiler.Decompiler.IR.Functions;
using CoDLuaDecompiler.Decompiler.IR.Identifiers;
using CoDLuaDecompiler.Decompiler.IR.Instruction;

namespace CoDLuaDecompiler.Decompiler.Analyzers.Havok
{
    public class LUIFixesAnalyzer : IAnalyzer
    {
        /// <summary>
        /// Fixes general issues with Black Ops 3 LUI decompilation issues
        /// </summary>
        /// <param name="f"></param>
        public void Analyze(Function f)
        {
            FixWidgetSelfVariable(f);
        }

        /// <summary>
        /// Fixes the issue where in some widgets there is an extra variable used instead of the self variable.
        ///
        /// Changes: 
        /// local f7_local1 = self
        /// self.anyChildUsesUpdateState = true
        /// local Texture = CoD.AbilityWheel_Texture.new( f7_local1, controller )
        ///
        /// To:
        ///
        /// self.anyChildUsesUpdateState = true
        /// local Texture = CoD.AbilityWheel_Texture.new( self, controller )
        /// </summary>
        /// <param name="f"></param>
        public void FixWidgetSelfVariable(Function f)
        {
            foreach (var b in f.Blocks)
            {
                for (int i = 0; i < b.Instructions.Count - 1; i++)
                {
                    var instr = b.Instructions[i];
                    if (instr is Assignment a && a.Left.Count == 1 && a.Right is IdentifierReference ir &&
                        b.Instructions[i + 1] is Assignment a2 && a2.Left.Count == 1 && a2.Left[0].HasIndex && a2.Left[0].Identifier == ir.Identifier && 
                        a2.Left[0].TableIndices.Count == 1 && a2.Left[0].TableIndices[0] is Constant c && c.Type == ValueType.String && c.String == "anyChildUsesUpdateState" && a2.Right is Constant)
                    {
                        a.Left[0].Identifier.Name = "self";
                        b.Instructions.RemoveAt(i);
                        return;
                    }
                }
            }
        }
    }
}