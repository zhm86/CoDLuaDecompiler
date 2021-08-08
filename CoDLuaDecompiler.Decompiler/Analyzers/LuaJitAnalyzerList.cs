using System.Collections.Generic;
using CoDLuaDecompiler.Decompiler.Analyzers.Havok;
using CoDLuaDecompiler.Decompiler.Analyzers.LuaJit;

namespace CoDLuaDecompiler.Decompiler.Analyzers
{
    public class LuaJitAnalyzerList : IAnalyzerList
    {
        public List<IAnalyzer> GetAnalyzers()
        {
            return new List<IAnalyzer>()
            {
                new LabelApplyAnalyzer(),
                new UnneededReturnAnalyzer(),
                
                new VarargListAssignmentAnalyzer(),
                new MultiBoolAssignmentAnalyzer(),
                new RedundantAssignmentsAnalyzer(),
                new LuaJitConditionalJumpsAnalyzer(),
                new LuaJitConditionalAssignmentsAnalyzer(),
                new ControlFlowIntegrityAnalyzer(),
                new UnusedLabelsAnalyzer(),
                new RemovingDataAnalyzer(),
                
                new TestsetAnalyzer(),
                new OrAndOperatorAnalyzer(),
                new TernaryOperatorAnalyzer(),
                new ConstructCfgAnalyzer(),
                new LuaJitIndeterminateArgumentsAnalyzer(),
                new Lua51LoopsAnalyzer(),
                
                new ConvertToSsaAnalyzer(),
                // Data flow passes
                new DeadAssignmentsAnalyzer(),
                new UnusedPhiFunctionsAnalyzer(),
                new ExpressionPropagationAnalyzer(),
                new ListInitializersAnalyzer(),
                
                new CompoundConditionalsAnalyzer(),
                new LoopsAnalyzer(),
                new LoopConditionalsAnalyzer(),
                new TwoWayConditionalsAnalyzer(),
                new IfElseFollowChainAnalyzer(),

                new DeadAssignmentsAnalyzer(),
                new ExpressionPropagationAnalyzer(),
                new LivenessNoInterferenceAnalyzer(),

                // Convert out of SSA and rename variables
                new SSADropSubscriptsAnalyzer(),
                new LocalDeclarationsAnalyzer(),

                new RenameVariablesAnalyzer(),
                new ParenthesizeAnalyzer(),
                new WidgetEmptyLinesAnalyzer(),
                new InlineClosuresAnalyzer(),
                new MultipleNotsAnalyzer(),

                new UnnecessaryReturnsAnalyzer(),

                new ConvertToASTAnalyzer(),
            };
        }
    }
}