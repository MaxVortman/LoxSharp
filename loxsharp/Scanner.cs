using System.Collections.Generic;

namespace Lox_
{
    class Scanner
    {

        private static IDictionary<string, TokenType> _keywords = new Dictionary<string, TokenType>{
            {"and", TokenType.And},
            {"class", TokenType.Class},
            {"else", TokenType.Else},
            {"false", TokenType.False},
            {"for", TokenType.For},
            {"fun", TokenType.Fun},
            {"if", TokenType.If},
            {"nil", TokenType.Nil},
            {"or", TokenType.Or},
            {"print", TokenType.Print},
            {"return", TokenType.Return},
            {"super", TokenType.Super},
            {"this", TokenType.This},
            {"true", TokenType.True},
            {"var", TokenType.Var},
            {"while", TokenType.While}
        };

        private readonly string _source;
        private readonly List<Token> _tokens = new List<Token>();

        private int _start = 0;
        private int _current = 0;
        private int _line = 1;

        internal Scanner(string source)
        {
            this._source = source;
        }

        internal IEnumerable<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                // We are at the beginning of the next lexeme.
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.Eof, "", null, _line));
            return _tokens;
        }

        private bool IsAtEnd()
        {
            return _current >= _source.Length;
        }

        private void ScanToken()
        {
            var c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LeftParen); break;
                case ')': AddToken(TokenType.RightParen); break;
                case '{': AddToken(TokenType.LeftBrace); break;
                case '}': AddToken(TokenType.RightBrace); break;
                case ',': AddToken(TokenType.Comma); break;
                case '.': AddToken(TokenType.Dot); break;
                case '-': AddToken(TokenType.Minus); break;
                case '+': AddToken(TokenType.Plus); break;
                case ';': AddToken(TokenType.Semicolon); break;
                case '*': AddToken(TokenType.Star); break;
                case '!': AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang); break;
                case '=': AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal); break;
                case '<': AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less); break;
                case '>': AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater); break;
                case '/':
                    if (Match('/'))
                    {
                        // A comment goes until the end of the line.                
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    }
                    else if (Match('*'))
                    {
                        // A comment /* goes until the */.                
                        while ((Peek() != '*' || PeekNext() != '/') && !IsAtEnd())
                        {
                            if (Peek() == '\n')
                                _line++;
                            Advance();
                        }

                        if (IsAtEnd())
                        {
                            Program.Error(_line, "Unexpected end of file");
                        }
                        else
                        {
                            Advance();
                            Advance();
                        }
                    }
                    else
                    {
                        AddToken(TokenType.Slash);
                    }
                    break;

                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace.                      
                    break;

                case '\n':
                    _line++;
                    break;

                case '"': String(); break;

                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Program.Error(_line, "Unexpected character.");
                    }
                    break;
            }
        }

        private char Advance()
        {
            _current++;
            return _source[_current - 1];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object literal)
        {
            var text = _source.Substring(_start, _current - _start);
            _tokens.Add(new Token(type, text, literal, _line));
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (_source[_current] != expected) return false;

            _current++;
            return true;
        }

        private bool MatchNext(char expected)
        {
            if (IsAtEnd()) return false;
            if (_source[_current + 1] != expected) return false;

            _current++;
            return true;
        }

        private char Peek()
        {
            return IsAtEnd() ? '\0' : _source[_current];
        }

        private void String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') _line++;
                Advance();
            }

            // Unterminated string.                                 
            if (IsAtEnd())
            {
                Program.Error(_line, "Unterminated string.");
                return;
            }

            // The closing ".                                       
            Advance();

            // Trim the surrounding quotes.                         
            var value = _source.Substring(_start + 1, _current - 2 - _start);
            AddToken(TokenType.String, value);
        }

        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private void Number()
        {
            while (IsDigit(Peek())) Advance();

            // Look for a fractional part.                            
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // Consume the "."                                      
                Advance();

                while (IsDigit(Peek())) Advance();
            }

            AddToken(TokenType.Number,
                double.Parse(_source.Substring(_start, _current - _start)));
        }

        private char PeekNext()
        {
            return _current + 1 >= _source.Length ? '\0' : _source[_current + 1];
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            // See if the identifier is a reserved word.   
            var text = _source.Substring(_start, _current - _start);
            if (!_keywords.TryGetValue(text, out var type)) type = TokenType.Identifier;
            AddToken(type);
        }

        private static bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                    c == '_';
        }

        private static bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }
    }
}