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
    private static Dictionary<string, Token> _reservedKeyWords;

    public static void BuildReservedKeyWordsDict()
    {
        _reservedKeyWords = new Dictionary<string, Token>();
        var startToken = TokenType.PROGRAM;
        var endToken = TokenType.END;
        for (var type = startToken; type <= endToken; type++)
        {
            if (type == TokenType.INTEGER_DIV)
            {
                _reservedKeyWords.Add("DIV", new Token(type, "DIV"));
            }
            else
            {
                _reservedKeyWords.Add(type.ToString(), new Token(type, type.ToString()));
            }
        }
    }
    
    public static void TryGetReservedKeyWord(string key, out Token token)
    {
        if (_reservedKeyWords == null)
        {
            BuildReservedKeyWordsDict();
        }
        _reservedKeyWords.TryGetValue(key.ToUpper(), out token!);
    }

    public Token(TokenType type, Object value, int lineno = 0, int column = 0)
    {
        Type = type;
        Value = value;
        Lineno = lineno;
        Column = column;
    }

    public override string ToString()
    {
        return $"Token({Type}, {Convert.ToString(Value)}, position={Lineno}:{Column})";
    }

    public TokenType Type { get; }

    public Object Value { get; }

    public int Lineno { get; }

    public int Column { get; }
}