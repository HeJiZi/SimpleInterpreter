namespace SimpleInterpreter.Core;

public class Interpreter: NodeVisitor
{
    public static bool ShouldLogStack = false;

    private void Log(object message)
    {
        if (ShouldLogStack)
        {
            Console.WriteLine(message.ToString());
        }
    }
    
    private Parser _parser;
    private CallStack _callStack;

    public Interpreter(Parser parser):base()
    {
        _parser = parser;
        _callStack = new CallStack();
    }

    public void PrintVars()
    {
        Console.WriteLine(_callStack.ToString());
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
        var ar = _callStack.Peek();

        var varName = ((Var)assign.Left).Value;
        ar[varName] = value;
        return value;
    }

    protected override dynamic VisitVar(AST node)
    {
        var varNode = (Var)node;
        var varName = varNode.Value;
        var ar = _callStack.Peek();
        
        return ar[varName];
    }

    protected override dynamic VisitProgram(AST node)
    {
        var program = (Program)node;
        Log($"ENTER: PROGRAM {program.Name}");
        var ar = new ActivationRecord(program.Name, ARType.RPOGRAM, 1);
        _callStack.Push(ar);
        Log(_callStack);
        
        Visit(program.Block);
        
        Log($"LEAVE: PROGRAM {program.Name}");
        Log(_callStack);

        _callStack.Pop();
        return null;
    }

    protected override dynamic VisitProcedureCall(AST node)
    {
        var procedureCall = (ProcedureCall)node;
        var procName = procedureCall.ProcName;
        var procSymbol = procedureCall.ProcedureSymbol;

        var ar = new ActivationRecord(procName, ARType.PROCEDURE, procSymbol.ScopeLevel + 1);

        var formalParams = procSymbol.Params;
        var actualParams = procedureCall.ActualParams;
        for (int i = 0; i < formalParams.Count; i++)
        {
            ar[formalParams[i].Name] = Visit(actualParams[i]);
        }
        
        _callStack.Push(ar);
        Log($"ENTER: PROCEDURE {procName}");
        Log(_callStack);
        

        Visit(procSymbol.BlockAst);

        Log($"LEAVE: PROCEDURE {procName}");
        Log(_callStack);

        _callStack.Pop();

        return null;
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