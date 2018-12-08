namespace Lox_
{
    internal abstract class Expr
    {
        internal interface IVisitor<out T>
        {
            T VisitBinaryExpr(Binary expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitLiteralExpr(Literal expr);
            T VisitUnaryExpr(Unary expr);
        }
        internal class Binary : Expr
        {
            internal Binary(Expr left, Token op, Expr right)
            {
                Left = left;
                Op = op;
                Right = right;
            }

            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }

            internal Expr Left { get; }
            internal Token Op { get; }
            internal Expr Right { get; }
        }
        internal class Grouping : Expr
        {
            internal Grouping(Expr expression)
            {
                Expression = expression;
            }

            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }

            internal Expr Expression { get; }
        }
        internal class Literal : Expr
        {
            internal Literal(object value)
            {
                Value = value;
            }

            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }

            internal object Value { get; }
        }
        internal class Unary : Expr
        {
            internal Unary(Token op, Expr right)
            {
                Op = op;
                Right = right;
            }

            internal override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }

            internal Token Op { get; }
            internal Expr Right { get; }
        }

        internal abstract T Accept<T>(IVisitor<T> visitor);
    }
}
