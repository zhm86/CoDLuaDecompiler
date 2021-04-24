using System;
using System.Collections.Generic;
using CoDHVKDecompiler.Decompiler.IR.Identifiers;

namespace CoDHVKDecompiler.Decompiler.IR.Expression.Operator
{
    public class BinOp : IExpression, IOperator
    {
        public IExpression Left { get; set; }
        public IExpression Right { get; set; }
        public BinOperationType OperationType { get; set; }
        public bool HasParentheses { get; set; } = false;
        
        public BinOp(IExpression left, IExpression right, BinOperationType op)
        {
            Left = left;
            Right = right;
            OperationType = op;
        }
        
        public int GetPrecedence()
        {
            switch (OperationType)
            {
                case BinOperationType.OpPow:
                    return 0;
                case BinOperationType.OpMul:
                case BinOperationType.OpDiv:
                case BinOperationType.OpMod:
                    return 2;
                case BinOperationType.OpAdd:
                case BinOperationType.OpSub:
                    return 3;
                case BinOperationType.OpShiftRight:
                case BinOperationType.OpShiftLeft:
                    return 4;
                case BinOperationType.OpBAnd:
                    return 5;
                case BinOperationType.OpBOr:
                    return 6;
                case BinOperationType.OpEqual:
                case BinOperationType.OpNotEqual:
                case BinOperationType.OpLessThan:
                case BinOperationType.OpLessEqual:
                case BinOperationType.OpGreaterThan:
                case BinOperationType.OpGreaterEqual:
                case BinOperationType.OpLoopCompare:
                    return 7;
                case BinOperationType.OpAnd:
                    return 8;
                case BinOperationType.OpOr:
                    return 9;
                default:
                    return 99999;
            }
        }

        public void SetHasParentheses(bool paren)
        {
            HasParentheses = paren;
        }
        
        public BinOp NegateCondition()
        {
            switch (OperationType)
            {
                case BinOperationType.OpEqual:
                    OperationType = BinOperationType.OpNotEqual;
                    break;
                case BinOperationType.OpNotEqual:
                    OperationType = BinOperationType.OpEqual;
                    break;
                case BinOperationType.OpLessThan:
                    OperationType = BinOperationType.OpGreaterEqual;
                    break;
                case BinOperationType.OpLessEqual:
                    OperationType = BinOperationType.OpGreaterThan;
                    break;
                case BinOperationType.OpGreaterThan:
                    OperationType = BinOperationType.OpLessEqual;
                    break;
                case BinOperationType.OpGreaterEqual:
                    OperationType = BinOperationType.OpLessThan;
                    break;
                case BinOperationType.OpLoopCompare:
                    break;
                default:
                    throw new Exception("Attempting to negate non-conditional binary operation");
            }
            return this;
        }
        
        public override void Parenthesize()
        {
            // If left has a lower precedence than this op, then add parantheses to evaluate it first
            if (Left is IOperator op1 && op1.GetPrecedence() > GetPrecedence())
            {
                op1.SetHasParentheses(true);
            }
            if (Right is IOperator op2 && op2.GetPrecedence() > GetPrecedence())
            {
                op2.SetHasParentheses(true);
            }
            
            // If we're a comparison op, we may need to swap the left and right if they both refer to constants
            int leftConstId = Left.GetLowestConstantId();
            int rightConstId = Right.GetLowestConstantId();

            if (IsCompare() && OperationType != BinOperationType.OpLoopCompare && leftConstId != -1 && rightConstId != -1 && leftConstId > rightConstId)
            {
                // We need to swap the left and right to keep matching recompiles
                var tmp = Right;
                Right = Left;
                Left = tmp;
                if (OperationType == BinOperationType.OpLessThan)
                {
                    OperationType = BinOperationType.OpGreaterThan;
                }
                else if (OperationType == BinOperationType.OpGreaterThan)
                {
                    OperationType = BinOperationType.OpLessThan;
                }
                else if (OperationType == BinOperationType.OpLessEqual)
                {
                    OperationType = BinOperationType.OpGreaterEqual;
                }
                else if (OperationType == BinOperationType.OpGreaterEqual)
                {
                    OperationType = BinOperationType.OpLessEqual;
                }
            }

            Left.Parenthesize();
            Right.Parenthesize();
        }
        
        public bool IsCompare()
        {
            switch (OperationType)
            {
                case BinOperationType.OpEqual:
                case BinOperationType.OpNotEqual:
                case BinOperationType.OpLessThan:
                case BinOperationType.OpLessEqual:
                case BinOperationType.OpGreaterThan:
                case BinOperationType.OpGreaterEqual:
                case BinOperationType.OpLoopCompare:
                    return true;
            }
            return false;
        }
        
        public override HashSet<Identifier> GetUses(bool regOnly)
        {
            var ret = new HashSet<Identifier>();
            ret.UnionWith(Left.GetUses(regOnly));
            ret.UnionWith(Right.GetUses(regOnly));
            return ret;
        }

        public override void RenameUses(Identifier orig, Identifier newId)
        {
            Left.RenameUses(orig, newId);
            Right.RenameUses(orig, newId);
        }

        public override bool ReplaceUses(Identifier orig, IExpression sub)
        {
            bool replaced = false;
            if (ShouldReplace(orig, Left))
            {
                Left = sub;
                replaced = true;
            }
            else
            {
                replaced = replaced || Left.ReplaceUses(orig, sub);
            }
            if (ShouldReplace(orig, Right))
            {
                Right = sub;
                replaced = true;
            }
            else
            {
                replaced = replaced || Right.ReplaceUses(orig, sub);
            }
            return replaced;
        }

        public override List<IExpression> GetExpressions()
        {
            var ret = new List<IExpression>() { this };
            ret.AddRange(Left.GetExpressions());
            ret.AddRange(Right.GetExpressions());
            return ret;
        }
        
        public override int GetLowestConstantId()
        {
            int left = Left.GetLowestConstantId();
            int right = Right.GetLowestConstantId();
            if (left == -1)
            {
                return right;
            }
            if (right == -1)
            {
                return left;
            }
            return Math.Min(left, right);
        }
        
        public override string ToString()
        {
            string op = "";
            switch (OperationType)
            {
                case BinOperationType.OpAdd:
                    op = "+";
                    break;
                case BinOperationType.OpDiv:
                    op = "/";
                    break;
                case BinOperationType.OpMod:
                    op = "%";
                    break;
                case BinOperationType.OpMul:
                    op = "*";
                    break;
                case BinOperationType.OpPow:
                    op = "^";
                    break;
                case BinOperationType.OpSub:
                    op = "-";
                    break;
                case BinOperationType.OpEqual:
                    op = "==";
                    break;
                case BinOperationType.OpNotEqual:
                    op = "~=";
                    break;
                case BinOperationType.OpLessThan:
                    op = "<";
                    break;
                case BinOperationType.OpLessEqual:
                    op = "<=";
                    break;
                case BinOperationType.OpGreaterThan:
                    op = ">";
                    break;
                case BinOperationType.OpGreaterEqual:
                    op = ">=";
                    break;
                case BinOperationType.OpAnd:
                    op = "and";
                    break;
                case BinOperationType.OpOr:
                    op = "or";
                    break;
                case BinOperationType.OpBAnd:
                    op = "&";
                    break;
                case BinOperationType.OpBOr:
                    op = "|";
                    break;
                case BinOperationType.OpShiftRight:
                    op = ">>";
                    break;
                case BinOperationType.OpShiftLeft:
                    op = "<<";
                    break;
                case BinOperationType.OpLoopCompare:
                    op = ">?=";
                    break;
            }
            string ret = "";
            if (HasParentheses)
            {
                ret += "(";
            }
            ret += $@"{Left} {op} {Right}";
            if (HasParentheses)
            {
                ret += ")";
            }
            return ret;
        }
    }
}