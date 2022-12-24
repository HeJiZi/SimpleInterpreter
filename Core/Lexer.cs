namespace SimpleInterpreter.Core;

public class Lexer
{
    private const char EndFlag = '\0';
    private readonly string _text;
    private int _pos;
    private char _currentChar;

    public Lexer(string text)
    {
        _text = text;
        _pos = 0;
        if (_text.Length > 0)
        {
            _currentChar = _text[_pos];
        }
        else
        {
            _currentChar = EndFlag;
        }
    }
    private void Error()
    {
        throw new Exception("Invalid character");
    }

    private void SkipWhitespace()
    {
        while (_currentChar != EndFlag && char.IsWhiteSpace(_currentChar))
        {
            Advance();
        }
    }

    private void SkipComment()
    {
        while (_currentChar != '}')
        {
            Advance();
        }
        Advance();
    }

    private char Peek()
    {
        var peekPos = _pos + 1;
        if (peekPos >= _text.Length)
            return EndFlag;
        return _text[peekPos];
    }
    
    private void Advance()
    {
        _pos++;
        if (_pos >= _text.Length)
            _currentChar = EndFlag;
        else
        {
            _currentChar = _text[_pos];
        }
    }

    private Token Id()
    {
        //Handle identifiers and reserved keywords
        int startIndex = _pos;
        int subLength = 0;
        while (_currentChar != EndFlag && char.IsAsciiLetterOrDigit(_currentChar))
        {
            subLength++;
            Advance();
        }

        Token result = null;
        string identifier = _text.Substring(startIndex, subLength);
        Token.TryGetReservedKeyWord(identifier, out result);
        if (result == null)
        {
            return new Token(TokenType.Id, identifier);
        }

        return result;

    }

    /// <summary>
    /// 根据输入流生成一个数字
    /// </summary>
    /// <returns></returns>
    private Token Number()
    {
        int intVal = 0;
        Token token = null;
        do
        {
            intVal = intVal * 10 + (_currentChar - '0');
            Advance();
        } while (_currentChar!= EndFlag && char.IsDigit(_currentChar));

        if (_currentChar == '.')
        {
            Advance();
            float floatVal = 0;
            float factor = 1;
            while (_currentChar != EndFlag && char.IsDigit(_currentChar))
            {
                floatVal = floatVal * 10 + (_currentChar - '0');
                factor *= 10;
                Advance();
            }

            floatVal /= factor;
            token = new Token(TokenType.RealConst, intVal + floatVal);
        }
        else
        {
            token = new Token(TokenType.IntegerConst, intVal);
        }

        return token;
    }

    /// <summary>
    /// lexical analyzer/parser
    /// 将输入流划分成不同的token
    /// </summary>
    /// <returns></returns>
    public Token GetNextToken()
    {
        while (_currentChar != EndFlag)
        {
            if (char.IsWhiteSpace(_currentChar))
            {
                SkipWhitespace();
            }
            
            if (_currentChar == '{')
            {
                Advance();
                SkipComment();
                continue;
            }
            
            if (char.IsAsciiLetter(_currentChar))
            {
                return Id();
            }

            if (_currentChar == ':' && Peek() == '=')
            {
                Advance();
                Advance();
                return new Token(TokenType.Assign, ":=");
            }
            
            if (char.IsDigit(_currentChar))
            {
                return Number();
            }
            
            switch (_currentChar)
            {
                case ';':
                    Advance();
                    return new Token(TokenType.Semi, ';');
                case '.':
                    Advance();
                    return new Token(TokenType.Dot, '.');
                case '+':
                    Advance();
                    return new Token(TokenType.Plus, '+');
                case '-':
                    Advance();
                    return new Token(TokenType.Minus, '-');
                case '*':
                    Advance();
                    return new Token(TokenType.Mul, '*');
                case '/':
                    Advance();
                    return new Token(TokenType.FloatDiv, '/');
                case '(':
                    Advance();
                    return new Token(TokenType.LParen, '(');
                case ')':
                    Advance();
                    return new Token(TokenType.RParen, ')');
                case ':':
                    Advance();
                    return new Token(TokenType.Colon, ':');
                case ',':
                    Advance();
                    return new Token(TokenType.Comma, ',');
                default:
                    Error();
                    break;
            }
        }

        return new Token(TokenType.Eof, null);
    }

}