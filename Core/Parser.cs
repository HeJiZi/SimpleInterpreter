namespace SimpleInterpreter.Core;

public class Parser
{
    private static List<Var> tempVarList = new(128);

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
    
    public AST Parse()
    {
        var node = Program();
        if(_currentToken.Type != TokenType.Eof)
            Error();
        return node;
    }

    public AST Program()
    {
        //program : PROGRAM variable SEMI block DOT
        Eat(TokenType.PROGRAM);
        string programName = (string)_currentToken.Value;
        Eat(TokenType.Id);
        Eat(TokenType.Semi);
        var block = Block();
        Eat(TokenType.Dot);
        return new Program(programName, block);
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
        /*
         factor : PLUS factor
              | MINUS factor
              | INTEGER_CONST
              | REAL_CONST
              | LPAREN expr RPAREN
              | variable
         */

        var lastToken = _currentToken;
        switch (lastToken.Type)
        {
            case TokenType.Plus:
                Eat(TokenType.Plus);
                return new UnaryOp(lastToken, Factor());
            case TokenType.Minus:
                Eat(TokenType.Minus);
                return new UnaryOp(lastToken, Factor());
            case TokenType.IntegerConst:
                Eat(TokenType.IntegerConst);
                return new Num(lastToken);
            case TokenType.RealConst:
                Eat(TokenType.RealConst);
                return new Num(lastToken);
            case TokenType.LParen:
                Eat(TokenType.LParen);
                var node = Expr();
                Eat(TokenType.RParen);
                return node;
            case TokenType.Id:
                return Variable();
        }
        Error();
        return null;
    }

    private AST Term()
    {
        //term : factor ((MUL | INTEGER_DIV | FLOAT_DIV) factor)*
        var node = Factor();
        while (_currentToken.Type is TokenType.Mul or TokenType.FloatDiv or TokenType.INTEGER_DIV)
        {
            var lastToken = _currentToken;
            Eat(lastToken.Type);
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

    public AST CompoundStatement()
    {
        //compound_statement: BEGIN statement_list END
        var node = new Compound();
        Eat(TokenType.BEGIN);
        StatementList(node.Children);
        Eat(TokenType.END);
        return node;
    }

    public void StatementList(List<AST> children)
    {
        /*
        statement_list : statement
            | statement SEMI statement_list
        */
        var node = Statement();
        children.Add(node);

        while (_currentToken.Type == TokenType.Semi)
        {
            Eat(TokenType.Semi);
            children.Add(Statement());
        }
        if(_currentToken.Type == TokenType.Id)
            Error();

    }

    public AST Statement()
    {
        /*
        statement : compound_statement
              | assignment_statement
              | empty
        */
        switch (_currentToken.Type)
        {
            case TokenType.BEGIN:
                return CompoundStatement();
            case TokenType.Id:
                return AssignmentStatement();
            default:
                return Empty();
        }
    }

    public AST AssignmentStatement()
    {
        //assignment_statement : variable ASSIGN expr
        var left = Variable();
        var token = _currentToken;
        Eat(TokenType.Assign);
        var right = Expr();
        return new Assign(left, token, right);
    }

    public AST Variable()
    {
        var node = new Var(_currentToken);
        Eat(TokenType.Id);
        return node;
    }

    public AST Empty()
    {
        return new NoOp();
    }

    public Block Block()
    {
        //block : declarations compound_statement
        var declarations = Declarations();
        var compoundStatement = CompoundStatement();
        return new Block(declarations, compoundStatement);
    }

    public List<VarDecl> Declarations()
    {
        /*
         * declarations : VAR (variable_declaration SEMI)+
                    | empty
         */
        var result = new List<VarDecl>();
        if (_currentToken.Type != TokenType.VAR)
        {
            return result;
        }
        Eat(TokenType.VAR);
        
        VariableDeclaration(result);

        while (_currentToken.Type == TokenType.Semi)
        {
            Eat(TokenType.Semi);
            VariableDeclaration(result);
        }

        return result;
    }

    public void VariableDeclaration(List<VarDecl> varDeclaraions)
    {
        //variable_declaration : ID (COMMA ID)* COLON type_spec
        if(_currentToken.Type != TokenType.Id)
            return;

        tempVarList.Clear();
        tempVarList.Add((Var)Variable());
        while (_currentToken.Type == TokenType.Comma)
        {
            Eat(TokenType.Comma);
            tempVarList.Add((Var)Variable());
        }
        Eat(TokenType.Colon);

        var typeNode = TypeNode();
        foreach (var varNode in tempVarList)
        {
            varDeclaraions.Add(new VarDecl(varNode, typeNode));
        }

        tempVarList.Clear();
    }

    public Type TypeNode()
    {
        /*
         type_spec : INTEGER
            | REAL
        */
        var token = _currentToken;
        Eat(_currentToken.Type == TokenType.INTEGER ? TokenType.INTEGER : TokenType.REAL);
        return new Type(token);
    }
}