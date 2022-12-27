using System.Diagnostics;

namespace SimpleInterpreter.Core;

public enum TokenType
{
    //保留关键字
    PROGRAM,
    PROCEDURE,
    VAR,
    INTEGER_DIV,
    INTEGER,
    REAL,
    BEGIN,
    END,
    
    //操作符
    IntegerConst,
    RealConst,
    Plus,
    Minus,
    Mul,
    FloatDiv,
    LParen,
    RParen,
    Id,
    Assign,
    Semi,
    Colon,
    Comma,
    Dot,
    Eof
}

public class Token
{
    private static Dictionary<string, Token> _reservedKeyWords = new()
    {
        {"PROGRAM", new Token(TokenType.PROGRAM, "PROGRAM")},
        {"PROCEDURE", new Token(TokenType.PROCEDURE, "PROCEDURE")},
        {"VAR", new Token(TokenType.VAR, "VAR")},
        {"DIV", new Token(TokenType.INTEGER_DIV, "DIV")},
        {"INTEGER", new Token(TokenType.INTEGER, "INTEGER")},
        {"REAL", new Token(TokenType.REAL, "REAL")},
        {"BEGIN", new Token(TokenType.BEGIN, "BEGIN")},
        {"END", new Token(TokenType.END, "End")},
    };

    public static void TryGetReservedKeyWord(string key, out Token token)
    {
        _reservedKeyWords.TryGetValue(key, out token!);
    }

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