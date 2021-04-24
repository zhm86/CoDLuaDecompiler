using System;
using System.Linq;
using System.Linq.Expressions;
using CoDHVKDecompiler.Decompiler.IR.Expression;
using CoDHVKDecompiler.Decompiler.IR.Expression.Operator;
using CoDHVKDecompiler.Decompiler.IR.Functions;
using CoDHVKDecompiler.Decompiler.IR.Instruction;

namespace CoDHVKDecompiler.Decompiler.Analyzers
{
    public class CompoundConditionalsAnalyzer : IAnalyzer
    {
        public void Analyze(Function f)
        {
            // Loop until nothing changes anymore
            bool changed = true;
            while (changed)
            {
                changed = false;
                // Give each block their reverse index
                f.NumberReversePostorder();
                // Loop over all blocks in reverse
                foreach (var node in f.PostorderTraversal(false))
                {
                    if (node.Instructions.Count > 0 && node.Instructions.Last() is Jump c && c.Conditional && c.Condition is BinOp bo && bo.OperationType == BinOperationType.OpLoopCompare)
                    {
                        continue;
                    }
                    if (node.Successors.Count() == 2 && node.Instructions.Last() is Jump n)
                    {
                        var t = node.Successors[0];
                        var e = node.Successors[1];
                        if (t.Successors.Count() == 2 && t.Instructions.First() is Jump tj && t.Predecessors.Count() == 1)
                        {
                            if (t.Successors[0] == e && t.Successors[1] != e)
                            {
                                //var newCond = new BinOp(new UnaryOp(n.Condition, UnaryOp.OperationType.OpNot), tj.Condition, BinOp.OperationType.OpOr);
                                IExpression newCond;
                                if (n.Condition is BinOp b && b.IsCompare())
                                {
                                    newCond = new BinOp(b.NegateCondition(), tj.Condition, BinOperationType.OpOr);
                                }
                                else
                                {
                                    newCond = new BinOp(new UnaryOp(n.Condition, UnOperationType.OpNot), tj.Condition, BinOperationType.OpOr);
                                }
                                n.Condition = newCond;
                                if (t.Follow != null)
                                {
                                    node.Follow = (node.Follow.ReversePostorderNumber > t.Follow.ReversePostorderNumber) ? node.Follow : t.Follow;
                                }
                                node.Successors[1] = t.Successors[1];
                                n.BlockDest = node.Successors[1];
                                var i = t.Successors[1].Predecessors.IndexOf(t);
                                t.Successors[1].Predecessors[i] = node;
                                node.Successors[0] = e;
                                i = t.Successors[0].Predecessors.IndexOf(t);
                                //e.Predecessors[i] = node;
                                f.Blocks.Remove(t);
                                e.Predecessors.Remove(t);
                                t.Successors[1].Predecessors.Remove(t);
                                changed = true;
                            }
                            else if (t.Successors[1] == e)
                            {
                                var newCond = new BinOp(n.Condition, tj.Condition, BinOperationType.OpAnd);
                                n.Condition = newCond;
                                if (t.Follow != null)
                                {
                                    node.Follow = (node.Follow.ReversePostorderNumber > t.Follow.ReversePostorderNumber) ? node.Follow : t.Follow;
                                }
                                node.Successors[0] = t.Successors[0];
                                var i = t.Successors[0].Predecessors.IndexOf(t);
                                t.Successors[0].Predecessors[i] = node;
                                e.Predecessors.Remove(t);
                                f.Blocks.Remove(t);
                                changed = true;
                            }
                        }
                        else if (e.Successors.Count() == 2 && e.Instructions.First() is Jump ej && e.Predecessors.Count() == 1)
                        {
                            if (e.Successors[0] == t)
                            {
                                var newCond = new BinOp(new UnaryOp(n.Condition, UnOperationType.OpNot), ej.Condition, BinOperationType.OpOr);
                                n.Condition = newCond;
                                if (e.Follow != null)
                                {
                                    node.Follow = (node.Follow.ReversePostorderNumber > e.Follow.ReversePostorderNumber) ? node.Follow : e.Follow;
                                }
                                node.Successors[1] = e.Successors[1];
                                n.BlockDest = node.Successors[1];
                                var i = e.Successors[1].Predecessors.IndexOf(e);
                                e.Successors[1].Predecessors[i] = node;
                                t.Predecessors.Remove(e);
                                f.Blocks.Remove(e);
                                changed = true;
                            }
                            else if (e.Successors[1] == t)
                            {
                                // TODO: not correct
                                throw new Exception("this is used so fix it");
                                var newCond = new BinOp(n.Condition, ej.Condition, BinOperationType.OpOr);
                                n.Condition = newCond;
                                if (e.Follow != null)
                                {
                                    node.Follow = (node.Follow.ReversePostorderNumber > e.Follow.ReversePostorderNumber) ? node.Follow : e.Follow;
                                }
                                node.Successors[1] = e.Successors[0];
                                var i = e.Successors[0].Predecessors.IndexOf(e);
                                e.Successors[0].Predecessors[i] = node;
                                t.Predecessors.Remove(e);
                                f.Blocks.Remove(e);
                                changed = true;
                            }
                        }
                    }
                }
            }
            f.ComputeDominance();
        }
    }
}