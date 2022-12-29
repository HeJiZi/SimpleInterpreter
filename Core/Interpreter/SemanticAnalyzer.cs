
namespace SimpleInterpreter.Core;

public class SemanticAnalyzer:NodeVisitor
{
    private ScopedSymbolTable currentScope;

    public SemanticAnalyzer()
    {
        // symbolTable = new ScopedSymbolTable("global", 1);
        currentScope = null;
    }

    public override string ToString()
    {
        return currentScope.ToString();
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

    protected override dynamic VisitProgram(AST node)
    {
        Console.WriteLine("ENTER scope: global");
        var globalScope = new ScopedSymbolTable("global", 1, currentScope);
        currentScope = globalScope;
        
        Visit(((Program)node).Block);
        
        Console.WriteLine(globalScope);
        currentScope = currentScope.EnclosingScope;
        Console.WriteLine("LEAVE scope: global");
        return null;
    }

    protected override dynamic VisitBinOp(AST node)
    {
        var binOp = (BinOp)node;
        Visit(binOp.Left);
        Visit(binOp.Right);
        return base.VisitBinOp(node);
    }

    protected override dynamic VisitUnaryOp(AST node)
    {
        Visit(((UnaryOp)node).Expr);
        return base.VisitUnaryOp(node);
    }

    protected override dynamic VisitCompound(AST node)
    {
        var compound = (Compound)node;
        foreach (var child in compound.Children)
        {
            Visit(child);
        }
        return base.VisitCompound(node);
    }

    protected override dynamic VisitVarDecl(AST node)
    {
        var varDecl = (VarDecl)node;
        string typeName = varDecl.TypeNode.Value;
        Symbol typeSymbol = currentScope.LookUp(typeName);
        if (typeSymbol == null)
            throw new Exception($"Undefined Type {typeName}");
        string varName = ((Var)varDecl.VarNode).Value;
        var varSymbol = new VarSymbol(varName, typeSymbol);
        
        if (currentScope.LookUp(varName, true) is not null)
        {
            throw new Exception($"Error: Duplicate identifier '{varName}' found");
        }
        currentScope.Insert(varSymbol);
        return null;
    }

    protected override dynamic VisitAssign(AST node)
    {
        var assign = (Assign)node;

        Visit(assign.Right);
        Visit(assign.Left);
        return null;
    }

    protected override dynamic VisitVar(AST node)
    {
        var varNode = (Var)node;
        var varSymbol = currentScope.LookUp(varNode.Value);
        if (varSymbol == null)
            throw new Exception($"Error: Symbol(identifier) not found {varNode.Value}");
        return null;
    }

    protected override dynamic VisitProcedureDecl(AST node)
    {
        var procedureDecl = (ProcedureDecl)node;
        var procName = procedureDecl.ProcName;
        var procSymbol = new ProcedureSymbol(procName);
        currentScope.Insert(procSymbol);
        Console.WriteLine($"ENTER scope: {procName}");
        var procedureScope = new ScopedSymbolTable(procName, currentScope.ScopeLeve + 1, currentScope);
        currentScope = procedureScope;

        foreach (var parameter in procedureDecl.Params)
        {
            var paramType = currentScope.LookUp(parameter.TypeNode.Value);
            var paramName = parameter.VarNode.Value;
            var varSymbol = new VarSymbol(paramName, paramType);
            currentScope.Insert(varSymbol);
            procSymbol.Params.Add(varSymbol);
        }

        Visit(procedureDecl.BlockNode);
        Console.WriteLine(procedureScope);
        currentScope = currentScope.EnclosingScope;
        Console.WriteLine($"LEAVE scope: {procName}");
        
        return null;
    }
}