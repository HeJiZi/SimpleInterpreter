namespace SimpleInterpreter.Core;

public enum NodeType
{
    Ast = 0,
    BinOp,
    Num,
    UnaryOp,
    Compound,
    Assign,
    Var,
    NoOp,
    Program,
    Block,
    VarDecl,
    Type,
}
public abstract class AST
{
    public Token Token { get; protected init; }
    public abstract NodeType NodeType { get; }
};

public class BinOp : AST
{
    public AST Left { get; }
    public AST Right { get; }
    public Token Op { get; }

    public override NodeType NodeType => NodeType.BinOp;

    public BinOp(AST left, Token op, AST right)
    {
        Left = left;
        Op = Token = op;
        Right = right;
    }
}

public class Num : AST
{
    public dynamic Value { get; }
    public override NodeType NodeType => NodeType.Num;

    public Num(Token token)
    {
        Token = token;
        Value = Token.Value;
    }
}

public class UnaryOp : AST
{
    public Token Op { get; }
    public AST Expr { get; }
    public override NodeType NodeType => NodeType.UnaryOp;

    public UnaryOp(Token op, AST expr)
    {
        Op = Token = op;
        Expr = expr;
    }
}

public class Compound : AST
{
    public List<AST> Children { get; }
    public override NodeType NodeType => NodeType.Compound;

    public Compound()
    {
        Children = new List<AST>();
    }
}

public class Assign : AST
{
    public AST Left { get; }
    
    public Token Op { get; }

    public AST Right { get; }
    
    public override NodeType NodeType => NodeType.Assign;

    public Assign(AST left, Token op, AST right)
    {
        Left = left;
        Op = Token = op;
        Right = right;
    }
}

public class Var : AST
{
    public string Value { get; }
    public override NodeType NodeType => NodeType.Var;

    public Var(Token token)
    {
        Token = token;
        Value = (string?)Token.Value;
    }
}

public class NoOp : AST
{
    public override NodeType NodeType => NodeType.NoOp;
}

public class Program : AST
{
    public override NodeType NodeType => NodeType.Program;

    public string Name { get; }

    public Block Block { get; }

    public Program(string name, Block block)
    {
        Name = name;
        Block = block;
    }
}

public class Block : AST
{
    public override NodeType NodeType => NodeType.Block;

    public List<VarDecl> Declarations { get; }

    public AST CompoundStatement { get; }

    public Block(List<VarDecl> declarations, AST compoundStatement)
    {
        Declarations = declarations;
        CompoundStatement = compoundStatement;
    }
}


public class VarDecl : AST
{
    public override NodeType NodeType => NodeType.VarDecl;

    public AST VarNode { get; }
    
    public Type TypeNode { get; }

    public VarDecl(AST varNode, Type typeNode)
    {
        VarNode = varNode;
        TypeNode = typeNode;
    }

}

public class Type : AST
{
    public override NodeType NodeType => NodeType.Type;

    public string Value { get; }

    public Type(Token token)
    {
        Token = token;
        Value = (string?)token.Value;
    }
}