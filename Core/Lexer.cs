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

    /// <summary>
    /// 根据输入流生成一个多位的整形
    /// </summary>
    /// <returns></returns>
    private int Interger()
    {
        int intVal = 0;
        do
        {
            intVal = intVal * 10 + (_currentChar - '0');
            Advance();
        } while (_currentChar!= EndFlag && char.IsDigit(_currentChar));

        return intVal;
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
                continue;
            }
            
            if (char.IsDigit(_currentChar))
            {
                return new Token(TokenType.Interger, Interger());
            }

            if (_currentChar == '+')
            {
                Advance();
                return new Token(TokenType.Plus, _currentChar);
            }

            if (_currentChar == '-')
            {
                Advance();
                return new Token(TokenType.Minus, _currentChar);
            }

            if (_currentChar == '*')
            {
                Advance();
                return new Token(TokenType.Mul, _currentChar);
            }

            if (_currentChar == '/')
            {
                Advance();
                return new Token(TokenType.Div, _currentChar);
            }
            
            Error();
        }

        return new Token(TokenType.Eof, null);
    }

}