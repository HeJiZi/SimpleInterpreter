using SimpleInterpreter.Exceptions;

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
    
    private void Error(string errorCode, Token token)
    {
        throw new ParserError(errorCode:errorCode, token:token, message: $"{errorCode} -> {token}");
    }
    
    public AST Parse()
    {
        var node = Program();
        if(_currentToken.Type != TokenType.Eof)
            Error(ErrorCode.UnexpectedToken, _currentToken);
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
            Error(ErrorCode.UnexpectedToken, _currentToken);
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
        Error(ErrorCode.UnexpectedToken, _currentToken);
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
            Error(ErrorCode.UnexpectedToken, _currentToken);

    }

    public AST Statement()
    {
        /*
        statement : compound_statement
              | proccall_statement
              | assignment_statement
              | empty
        */
        switch (_currentToken.Type)
        {
            case TokenType.BEGIN:
                return CompoundStatement();
            case TokenType.Id:
                return _lexer.CurrentChar == '(' ? ProcCallStatement() : AssignmentStatement();
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

    public List<AST> Declarations()
    {
        /*
         * declarations : (VAR (variable_declaration SEMI)+)*
                    | (PROCEDURE ID (LPAREN formal_parameter_list RPAREN)? SEMI block SEMI)*
                    | empty
         */
        var result = new List<AST>();
        while (_currentToken.Type == TokenType.VAR)
        {
            Eat(TokenType.VAR);
            while (_currentToken.Type == TokenType.Id)
            {
                VariableDeclaration(result);
                Eat(TokenType.Semi);
            }
        }

        while (_currentToken.Type == TokenType.PROCEDURE)
        {
            result.Add(ProcedureDeclaration());
        }

        return result;
    }

    public ProcedureDecl ProcedureDeclaration()
    {
        Eat(TokenType.PROCEDURE);
        string procName = (string)_currentToken.Value;
        Eat(TokenType.Id);
        var @params = new List<Param>();
        if (_currentToken.Type == TokenType.LParen)
        {
            Eat(TokenType.LParen);
            FormalParameterList(@params);
            Eat(TokenType.RParen);
        }
        Eat(TokenType.Semi);
        Block blockNode = Block();
        var procedureDecl = new ProcedureDecl(procName, @params, blockNode);
        Eat(TokenType.Semi);
        return procedureDecl;
    }

    public void FormalParameterList(List<Param> result)
    {
        /*formal_parameter_list : formal_parameters
                              | formal_parameters SEMI formal_parameter_list
         */
        if (_currentToken.Type == TokenType.Id)
        {
            FormalParameters(result);
            while (_currentToken.Type == TokenType.Semi)
            {
                Eat(TokenType.Semi);
                FormalParameters(result);
            }
        }
    }

    public void FormalParameters(List<Param> parameters)
    {
        // formal_parameters : ID (COMMA ID)* COLON type_spec
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
            parameters.Add(new Param(varNode, typeNode));
        }

        tempVarList.Clear();
    }

    public void VariableDeclaration(List<AST> varDeclaraions)
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

    public ProcedureCall ProcCallStatement()
    {
        //proccall_statement : ID LPAREN (expr (COMMA expr)*)? RPAREN
        var token = _currentToken;
        string procName = (string)_currentToken.Value;
        
        Eat(TokenType.Id);
        Eat(TokenType.LParen);
        List<AST> actualParams = new List<AST>();
        if (_currentToken.Type != TokenType.RParen)
        {
            actualParams.Add(Expr());
        }

        while (_currentToken.Type == TokenType.Comma)
        {
            Eat(TokenType.Comma);
            actualParams.Add(Expr());
        }
        Eat(TokenType.RParen);

        return new ProcedureCall(procName, actualParams, token);
    }
}