using SimpleInterpreter.Exceptions;

namespace SimpleInterpreter.Core;

public class Lexer
{
    private const char EndFlag = '\0';
    private readonly string _text;
    private int _pos;
    private char _currentChar;

    private int _lineno;
    private int _column;

    public char CurrentChar => _currentChar;

    public Lexer(string text)
    {
        _text = text;
        _pos = 0;
        _currentChar = _text.Length > 0 ? _text[_pos] : EndFlag;

        _lineno = 1;
        _column = 1;
    }
    private void Error()
    {
        throw new LexerError(message:$"Lexer error on '{_currentChar}' line: {_lineno} column: {_column}");
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
        if (_currentChar == '\n')
        {
            _lineno++;
            _column = 0;
        }
        
        _pos++;
        if (_pos >= _text.Length)
        {
            _currentChar = EndFlag;
        }
        else
        {
            _column++;
            _currentChar = _text[_pos];
        }
    }

    private Token Id()
    {
        //Handle identifiers and reserved keywords
        int startIndex = _pos;
        int subLength = 0;
        while (_currentChar != EndFlag && char.IsLetterOrDigit(_currentChar))
        {
            subLength++;
            Advance();
        }

        Token result = null;
        string identifier = _text.Substring(startIndex, subLength);
        Token.TryGetReservedKeyWord(identifier, out result);
        if (result == null)
        {
            return new Token(TokenType.Id, identifier, _lineno, _column);
        }
        else
        {
            result = new Token(result.Type, result.Value, _lineno, _column);
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
            token = new Token(TokenType.RealConst, intVal + floatVal, _lineno, _column);
        }
        else
        {
            token = new Token(TokenType.IntegerConst, intVal, _lineno, _column);
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
            
            if (char.IsLetter(_currentChar))
            {
                return Id();
            }

            if (_currentChar == ':' && Peek() == '=')
            {
                Advance();
                Advance();
                return new Token(TokenType.Assign, ":=", _lineno, _column);
            }
            
            if (char.IsDigit(_currentChar))
            {
                return Number();
            }
            
            switch (_currentChar)
            {
                case ';':
                    Advance();
                    return new Token(TokenType.Semi, ';', _lineno, _column);
                case '.':
                    Advance();
                    return new Token(TokenType.Dot, '.', _lineno, _column);
                case '+':
                    Advance();
                    return new Token(TokenType.Plus, '+', _lineno, _column);
                case '-':
                    Advance();
                    return new Token(TokenType.Minus, '-', _lineno, _column);
                case '*':
                    Advance();
                    return new Token(TokenType.Mul, '*', _lineno, _column);
                case '/':
                    Advance();
                    return new Token(TokenType.FloatDiv, '/', _lineno, _column);
                case '(':
                    Advance();
                    return new Token(TokenType.LParen, '(', _lineno, _column);
                case ')':
                    Advance();
                    return new Token(TokenType.RParen, ')', _lineno, _column);
                case ':':
                    Advance();
                    return new Token(TokenType.Colon, ':', _lineno, _column);
                case ',':
                    Advance();
                    return new Token(TokenType.Comma, ',', _lineno, _column);
                default:
                    Error();
                    break;
            }
        }

        return new Token(TokenType.Eof, null, _lineno, _column);
    }

}