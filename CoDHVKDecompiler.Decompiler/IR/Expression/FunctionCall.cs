using System;
using System.Collections.Generic;
using System.Linq;
using CoDHVKDecompiler.Decompiler.IR.Identifiers;
using ValueType = CoDHVKDecompiler.Decompiler.IR.Identifiers.ValueType;

namespace CoDHVKDecompiler.Decompiler.IR.Expression
{
    public class FunctionCall : IExpression
    {
        public IExpression Function { get; set; }
        public List<IExpression> Arguments { get; set; }

        public uint BeginArg { get; set; } = 0;
        public bool IsIndeterminateReturnCount { get; set; } = false;
        public bool IsIndeterminateArgumentCount { get; set; } = false;
        public bool IsFunctionCalledOnSelf { get; set; } = false;
        /// <summary>
        /// Index of where the function def register was originally defined. Used to help decide what expressions to inline
        /// </summary>
        public int FunctionDefIndex { get; set; } = 0;
        
        public FunctionCall(IExpression fun, List<IExpression> args)
        {
            Function = fun;
            Arguments = args;
        }
        
        public override void Parenthesize()
        {
            Function.Parenthesize();
            Arguments.ForEach(x => x.Parenthesize());
        }
        
        public override HashSet<Identifier> GetUses(bool regOnly)
        {
            var ret = new HashSet<Identifier>();
            foreach (var arg in Arguments)
            {
                ret.UnionWith(arg.GetUses(regOnly));
            }
            ret.UnionWith(Function.GetUses(regOnly));
            return ret;
        }

        public override void RenameUses(Identifier orig, Identifier newId)
        {
            Function.RenameUses(orig, newId);
            foreach (var arg in Arguments)
            {
                arg.RenameUses(orig, newId);
            }
        }

        public override bool ReplaceUses(Identifier orig, IExpression sub)
        {
            bool replaced = false;
            if (ShouldReplace(orig, Function) && (sub is IdentifierReference || sub is Constant))
            {
                Function = sub;
                replaced = true;
            }
            else
            {
                replaced = Function.ReplaceUses(orig, sub);
            }
            for (int i = 0; i < Arguments.Count; i++)
            {
                if (ShouldReplace(orig, Arguments[i]))
                {
                    Arguments[i] = sub;
                    replaced = true;
                }
                else
                {
                    replaced = replaced || Arguments[i].ReplaceUses(orig, sub);
                }
            }
            return replaced;
        }

        public override List<IExpression> GetExpressions()
        {
            var ret = new List<IExpression>() { this };
            foreach (var exp in Arguments)
            {
                ret.AddRange(exp.GetExpressions());
            }
            ret.AddRange(Function.GetExpressions());
            return ret;
        }
        
        public override int GetLowestConstantId()
        {
            var id = Function.GetLowestConstantId();
            foreach (var idx in Arguments)
            {
                var nid = idx.GetLowestConstantId();
                if (id == -1)
                {
                    id = nid;
                }
                else if (nid != -1)
                {
                    id = Math.Min(id, idx.GetLowestConstantId());
                }
            }
            return id;
        }

        public override string ToString()
        {
            string ret = "";

            // Pattern match special lua this call
            int beginarg = 0;
            if (Function is IdentifierReference ir && ir.TableIndices.Count >= 1 &&
                ir.TableIndices[^1] is Constant c && c.Type == ValueType.String)
            {
                if (IsFunctionCalledOnSelf)
                {
                    ret += $@"{ir.ToString().Substring(0, ir.ToString().Length - c.String.Length - 1)}:{c.String}(";
                }
                else if (Arguments.Any() && Arguments[0] is IdentifierReference thisIr && thisIr.TableIndices.Count == 0 && thisIr.Identifier == ir.Identifier)
                {
                    ret += $@"{ir.Identifier}:{c.String}(";
                    beginarg = 1;
                }
                else if (Arguments.Any() && Arguments[0] is IdentifierReference ir2 && ir2.TableIndices.Count >= 1)
                {
                    var str = ir.Identifier.ToString();
                    for(int i = 0; i < ir.TableIndices.Count - 1; i++)
                    {
                        var idx = ir.TableIndices[i];
                        if (idx is Constant c2 && c2.Type == ValueType.String)
                        {
                            str += "." + idx.ToString().Replace("\"", "");
                        }
                        else
                        {
                            str += $@"[{idx}]";
                        }
                    }

                    if (str == ir2.ToString() || IsFunctionCalledOnSelf)
                    {
                        ret += $@"{ir2}:{c.String}(";
                        beginarg = 1;
                    }
                    else
                    {
                        ret += $@"{Function}(";
                    }
                }
                else
                {
                    ret += $@"{Function}(";
                }
            }
            else
            {
                if (IsFunctionCalledOnSelf)
                {
                    
                }
                ret += Function + "(";
            }
            for (int i = beginarg; i < Arguments.Count(); i++)
            {
                ret += Arguments[i].ToString();
                if (i != Arguments.Count() - 1)
                {
                    ret += ", ";
                }
            }
            ret += ")";
            return ret;
        }
    }
}