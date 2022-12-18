namespace SimpleInterpreter.Core;

public enum NodeType
{
    Ast = 0,
    BinOp,
    Num,
    UnaryOp,
}
public class AST
{
    protected Token _token;
    public NodeType NodeType { get; protected init; } = NodeType.Ast;
};

public class BinOp : AST
{
    public AST Left { get; }
    public AST Right { get; }
    public Token Op { get; }

    public BinOp(AST left, Token op, AST right)
    {
        NodeType = NodeType.BinOp;
        Left = left;
        Op = _token = op;
        Right = right;
    }
}

public class Num : AST
{
    public int Value { get; }

    public Num(Token token)
    {
        NodeType = NodeType.Num;
        _token = token;
        Value = (int)_token.Value;
    }
}

public class UnaryOp : AST
{
    public Token Op { get; }
    public AST Expr { get; }

    public UnaryOp(Token op, AST expr)
    {
        NodeType = NodeType.UnaryOp;
        Op = op;
        Expr = expr;
    }
}