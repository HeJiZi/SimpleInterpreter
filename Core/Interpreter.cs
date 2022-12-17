namespace SimpleInterpreter.Core;

public class Interpreter
{
    private const int MaxDigitLength = 2;
    private const char EndFlag = '\0';
    private readonly string _text;
    private int _pos;
    private char _currentChar;
    private Token _currentToken;


    public Interpreter(string text)
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
        _currentToken = null;
    }

    // ##########################################################
    // # Lexer code                                             #
    // ##########################################################
    private void Error()
    {
        throw new Exception("Error parsing input");
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
    private Token GetNextToken()
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
            Error();
        }

        return new Token(TokenType.Eof, null);
    }

    // ##########################################################
    // # Interpreter code                                       #
    // ##########################################################
    
    /// <summary>
    /// 比较当前的token是否是一个符合语法规则的token，是的话就继续读，否的话则抛出异常
    /// </summary>
    /// <param name="tokenType"></param>
    private void Eat(TokenType tokenType)
    {
        TokenType lastTokenType = _currentToken.Type;
        if (lastTokenType == tokenType)
        {
            _currentToken = GetNextToken();
        }
        else
        {
            Error();
        }
    }

    private int Term()
    {
        var lastToken = _currentToken;
        Eat(TokenType.Interger);
        return (int)lastToken.Value;
    }

    public int Expr()
    {
        _currentToken = GetNextToken();
        int result = Term();

        while (_currentToken.Type == TokenType.Plus || _currentToken.Type == TokenType.Minus)
        {
            if (_currentToken.Type == TokenType.Minus)
            {
                Eat(TokenType.Minus);
                result -= Term();
            }
            else if (_currentToken.Type == TokenType.Plus)
            {
                Eat(TokenType.Plus);
                result += Term();
            }
        }

        return result;
    }
}