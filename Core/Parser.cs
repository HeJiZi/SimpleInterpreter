namespace SimpleInterpreter.Core;

public class Parser
{
    private Lexer _lexer;
    private Token _currentToken;

    public Parser(Lexer lexer)
    {
        _lexer = lexer;
        _currentToken = lexer.GetNextToken();
    }
    
    private void Error()
    {
        throw new Exception("Invalid syntax");
    }
    
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

    private AST Factor()
    {
        // factor: INTEGER | LPAREN expr RPAREN

        var lastToken = _currentToken;
        switch (lastToken.Type)
        {
            case TokenType.Plus:
                Eat(TokenType.Plus);
                return new UnaryOp(lastToken, Factor());
            case TokenType.Minus:
                Eat(TokenType.Minus);
                return new UnaryOp(lastToken, Factor());
                break;
            case TokenType.Interger:
                Eat(TokenType.Interger);
                return new Num(lastToken);
            case TokenType.LParen:
            {
                Eat(TokenType.LParen);
                var node = Expr();
                Eat(TokenType.RParen);
                return node;
            }
        }
        Error();
        return null;
    }

    private AST Term()
    {
        //term: factor((MUL|DIV)factor)*
        var node = Factor();
        while (_currentToken.Type is TokenType.Mul or TokenType.Div)
        {
            var lastToken = _currentToken;
            Eat(_currentToken.Type == TokenType.Mul ? TokenType.Mul : TokenType.Div);
            node = new BinOp(node, lastToken, Factor());
        }

        return node;
    }
    
    private AST Expr()
    {
        /***
            expr   : term ((PLUS | MINUS) term)*
            term   : factor ((MUL | DIV) factor)*
            factor : INTEGER | LPAREN expr RPAREN
        ***/
        var node = Term();

        while (_currentToken.Type is TokenType.Plus or TokenType.Minus)
        {
            var lastToken = _currentToken;
            Eat(_currentToken.Type == TokenType.Minus ? TokenType.Minus : TokenType.Plus);
            node = new BinOp(node, lastToken, Term());

        }

        return node;
    }

    public AST Parse()
    {
        return Expr();
    }
}