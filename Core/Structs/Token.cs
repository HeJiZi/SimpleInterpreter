namespace SimpleInterpreter.Core;

public enum TokenType
{
    Interger,
    Plus,
    Minus,
    Mul,
    Div,
    LParen,
    RParen,
    Eof
}

public class Token
{
    public Token(TokenType type, Object value)
    {
        Type = type;
        Value = value;
    }

    public override string ToString()
    {
        return $"Token({Type}, {Convert.ToString(Value)})";
    }

    public TokenType Type { get; }

    public Object Value { get; }
}