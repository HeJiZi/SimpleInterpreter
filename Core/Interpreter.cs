namespace SimpleInterpreter.Core;

public class Interpreter
{
    private const int MaxDigitLength = 2;

    private Lexer _lexer;
    
    private Token _currentToken;
    

    public Interpreter(Lexer lexer)
    {
        _lexer = lexer;
        _currentToken = lexer.GetNextToken();
    }
    
    private void Error()
    {
        throw new Exception("Invalid syntax");
    }
    
    // ##########################################################
    // # Interpreter code                                       #
    // ##########################################################
    
    /// <summary>
    /// 比较当前的token是否是一个符合语法规则的token，是的话就继续读取下个token，否的话则抛出异常
    /// </summary>
    /// <param name="tokenType"></param>
    private void Eat(TokenType tokenType)
    {
        TokenType lastTokenType = _currentToken.Type;
        if (lastTokenType == tokenType)
        {
            _currentToken = _lexer.GetNextToken();
        }
        else
        {
            Error();
        }
    }

    /// <summary>
    /// factor:Interger
    /// </summary>
    /// <returns></returns>
    private int Factor()
    {
        var lastToken = _currentToken;
        Eat(TokenType.Interger);
        return (int)lastToken.Value;
    }

    /// <summary>
    /// term: factor((MUL|DIV)factor)*
    /// </summary>
    /// <returns></returns>
    private int Term()
    {
        int result = Factor();
        while (_currentToken.Type is TokenType.Mul or TokenType.Div)
        {
            if (_currentToken.Type == TokenType.Mul)
            {
                Eat(TokenType.Mul);
                result *= Factor();
            }
            else
            {
                Eat(TokenType.Div);
                result /= Factor();
            }
        }

        return result;
    }

    public int Expr()
    {
        int result = Term();

        while (_currentToken.Type is TokenType.Plus or TokenType.Minus)
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