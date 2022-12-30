using SimpleInterpreter.Core;

namespace SimpleInterpreter.Exceptions;

public class Error:Exception
{
    protected string errorCode;

    protected Token token;

    protected string message;

    public Error(string errorCode = null, Token token = null, string message = null)
    {
        this.errorCode = errorCode;
        this.token = token;
        this.message = $"{message}";
    }

    public override string Message => this.message;
}

public class LexerError : Error
{
    public LexerError(string errorCode = null, Token token = null, string message = null) :
        base(errorCode, token, message)
    {
        
    }
}

public class ParserError : Error
{
    public ParserError(string errorCode = null, Token token = null, string message = null) :
        base(errorCode, token, message)
    {
        
    }
}

public class SemanticError : Error
{
    public SemanticError(string errorCode = null, Token token = null, string message = null) :
        base(errorCode, token, message)
    {
        
    }
}