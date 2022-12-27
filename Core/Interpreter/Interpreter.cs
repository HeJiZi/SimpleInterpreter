namespace SimpleInterpreter.Core;

public class Interpreter: NodeVisitor
{
    private Parser _parser;
    private Dictionary<string, dynamic> GLOBAL_SCOPE;
    
    public Interpreter(Parser parser):base()
    {
        _parser = parser;
        GLOBAL_SCOPE = new(10000);
    }

    public void PrintVars()
    {
        Console.WriteLine("\nStart:Run-time GLOBAL_MEMORY contents>");
        
        foreach (var (key,value) in GLOBAL_SCOPE)
        {
            Console.WriteLine("{0} = {1}", key, value);
        }
    }
    
    protected override dynamic VisitNum(AST node)
    {
        return ((Num)node).Value;
    }

    protected override dynamic VisitBinOp(AST node)
    {
        var binOp = (BinOp)node;
        switch (binOp.Op.Type)
        {
            case TokenType.Plus:
                return Visit(binOp.Left) + Visit(binOp.Right);
            case TokenType.Minus:
                return Visit(binOp.Left) - Visit(binOp.Right);
            case TokenType.Mul:
                return Visit(binOp.Left) * Visit(binOp.Right);
            case TokenType.INTEGER_DIV:
                return (int)Visit(binOp.Left) / (int)Visit(binOp.Right);
            case TokenType.FloatDiv:
                return (float)Visit(binOp.Left) / (float)Visit(binOp.Right);
            default:
                throw new InvalidOperationException($"InValid binOp{binOp.Op.Type}");
                return null;
        }
    }

    protected override dynamic VisitUnaryOp(AST node)
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

    protected override dynamic VisitCompound(AST node)
    {
        var compound = (Compound)node;
        foreach (var child in compound.Children)
        {
            Visit(child);
        }

        return -1;
    }

    protected override dynamic VisitAssign(AST node)
    {
        var assign = (Assign)node;
        var value = Visit(assign.Right);
        GLOBAL_SCOPE[((Var)assign.Left).Value] = value;
        return value;
    }

    protected override dynamic VisitVar(AST node)
    {
        var varNode = (Var)node;
        var varName = varNode.Value;
        if (!GLOBAL_SCOPE.ContainsKey(varName))
        {
            throw new Exception($"No Var {varName}");
        }

        return GLOBAL_SCOPE[varName];
    }

    protected override dynamic VisitProgram(AST node)
    {
        var program = (Program)node;
        return Visit(program.Block);
    }

    protected override dynamic VisitBlock(AST node)
    {
        var block = (Block)node;
        foreach (var declaration in block.Declarations)
        {
            Visit(declaration);
        }

        Visit(block.CompoundStatement);

        return null;
    }

    protected override dynamic VisitVarDecl(AST node)
    {
        return base.VisitVarDecl(node);
    }

    protected override dynamic VisitType(AST node)
    {
        return base.VisitType(node);
    }

    public dynamic Interprete()
    {
        var tree = _parser.Parse();
        return Visit(tree);
    }
}