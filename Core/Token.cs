namespace SimpleInterpreter.Core;

public enum TokenType
{
    Interger,
    Plus,
    Minus,
    Eof
}

public class Token
{
    public Token(TokenType type, Object value)
    {
        this.Type = type;
        this.Value = value;
    }

    public override string ToString()
    {
        return $"Token({this.Type}, {Convert.ToString(this.Value)})";
    }

    public TokenType Type { get; }

    public Object Value { get; }
}