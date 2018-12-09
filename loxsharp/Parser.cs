using System;
using System.Collections.Generic;
using System.Linq;
using static Lox_.TokenType;

namespace Lox_
{
    class Parser
    {
        private class ParseError : Exception
        {
        }

        private readonly IReadOnlyList<Token> _tokens;
        private int _current = 0;

        internal Parser(IReadOnlyList<Token> tokens)
        {
            this._tokens = tokens;
        }

        internal Expr Parse()
        {
            try
            {
                return Expression();
            }
            catch (ParseError error)
            {
                return null;
            }
        }

        private Expr Expression()
        {
            return Equality();
        }

        private Expr Equality()
        {
            var expr = Comparison();

            while (Match(BangEqual, EqualEqual))
            {
                var op = Previous();
                var right = Comparison();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Comparison()
        {
            var expr = Addition();

            while (Match(Greater, GreaterEqual, Less, LessEqual))
            {
                var op = Previous();
                var right = Addition();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Addition()
        {
            var expr = Multiplication();

            while (Match(Minus, Plus))
            {
                var op = Previous();
                var right = Multiplication();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Multiplication()
        {
            var expr = Unary();

            while (Match(Slash, Star))
            {
                var op = Previous();
                var right = Unary();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Unary()
        {
            if (Match(Bang, Minus))
            {
                var op = Previous();
                var right = Unary();
                return new Expr.Unary(op, right);
            }

            return Primary();
        }

        private Expr Primary()
        {
            if (Match(False)) return new Expr.Literal(false);
            if (Match(True)) return new Expr.Literal(true);
            if (Match(Nil)) return new Expr.Literal(null);

            if (Match(Number, TokenType.String))
            {
                return new Expr.Literal(Previous().Literal);
            }

            if (Match(LeftParen))
            {
                var expr = Expression();
                Consume(RightParen, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }

            throw Error(Peek(), "Expect expression.");
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();

            throw Error(Peek(), message);
        }

        private ParseError Error(Token token, string message)
        {
            Program.Error(token, message);
            return new ParseError();
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == Semicolon) return;

                switch (Peek().Type)
                {
                    case Class:
                    case Fun:
                    case Var:
                    case For:
                    case If:
                    case While:
                    case Print:
                    case Return:
                        return;
                }

                Advance();
            }
        }

        private bool Match(params TokenType[] types)
        {
            if (!types.Any(type => Check(type))) return false;
            Advance();
            return true;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Type == Eof;
        }

        private Token Peek()
        {
            return _tokens[_current];
        }

        private Token Previous()
        {
            return _tokens[_current - 1];
        }
    }
}