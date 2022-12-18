namespace SimpleInterpreter.Core;

public class NodeVisitor
{
    private  Dictionary<NodeType, Func<AST, int>> _visitMap;
    
    public NodeVisitor()
    {
        _visitMap = new Dictionary<NodeType, Func<AST, int>>()
        {
            { NodeType.Num, VisitNum },
            { NodeType.BinOp, VisitBinOp},
            { NodeType.UnaryOp, VisitUnaryOp},
        };
    }

    protected int Visit(AST node)
    {
        _visitMap.TryGetValue(node.NodeType, out var visitFunc);
        if (visitFunc != null)
        {
            return visitFunc(node);
        }

        throw new Exception($"No visit Method {node.NodeType}");
        return 0;
    }

    protected virtual int VisitNum(AST node)
    {
        Console.WriteLine("No Implement VisitNum");
        return -1;
    }

    protected virtual int VisitBinOp(AST node)
    {
        Console.WriteLine("No Implement VisitBinOp");
        return -1;
    }

    protected virtual int VisitUnaryOp(AST node)
    {
        Console.WriteLine("No Implement VisitUnaryOp");
        return -1;
    }
    
}

public class Interpreter: NodeVisitor
{
    private Parser _parser;
    public Interpreter(Parser parser):base()
    {
        _parser = parser;
    }
    
    protected override int VisitNum(AST node)
    {
        return ((Num)node).Value;
    }

    protected override int VisitBinOp(AST node)
    {
        var binOp = (BinOp)node;
        if (binOp.Op.Type == TokenType.Plus)
            return Visit(binOp.Left) + Visit(binOp.Right);
        if (binOp.Op.Type == TokenType.Minus)
            return Visit(binOp.Left) - Visit(binOp.Right);
        if (binOp.Op.Type == TokenType.Mul)
            return Visit(binOp.Left) * Visit(binOp.Right);
        if (binOp.Op.Type == TokenType.Div)
            return Visit(binOp.Left) / Visit(binOp.Right);

        throw new InvalidOperationException($"InValid binOp{binOp.Op.Type}");
        return -1;
    }

    protected override int VisitUnaryOp(AST node)
    {
        var unaryOp = (UnaryOp)node;
        if (unaryOp.Op.Type == TokenType.Minus)
        {
            return -Visit(unaryOp.Expr);
        }
        else if(unaryOp.Op.Type == TokenType.Plus)
        {
            return +Visit(unaryOp.Expr);
        }

        return -1;
    }

    public int Interprete()
    {
        var tree = _parser.Parse();
        return Visit(tree);
    }
}