namespace Lox_
{
    abstract class Expr
    {
        interface Visitor<T>
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
                this.left = left;
                this.op = op;
                this.right = right;
            }

            internal T Accept(Visitor<T> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }

            internal readonly Expr left;
            internal readonly Token op;
            internal readonly Expr right;
        }
        internal class Grouping : Expr
        {
            internal Grouping(Expr expression)
            {
                this.expression = expression;
            }

            internal T Accept(Visitor<T> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }

            internal readonly Expr expression;
        }
        internal class Literal : Expr
        {
            internal Literal(Object value)
            {
                this.value = value;
            }

            internal T Accept(Visitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }

            internal readonly Object value;
        }
        internal class Unary : Expr
        {
            internal Unary(Token op, Expr right)
            {
                this.op = op;
                this.right = right;
            }

            internal T Accept(Visitor<T> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }

            internal readonly Token op;
            internal readonly Expr right;
        }

        internal abstract T Accept(Visitor<T> visitor);
    }
}
