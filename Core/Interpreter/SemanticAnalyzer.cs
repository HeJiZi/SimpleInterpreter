
namespace SimpleInterpreter.Core;

public class SemanticAnalyzer:NodeVisitor
{
    private SymbolTable symbolTable;

    public SemanticAnalyzer()
    {
        symbolTable = new SymbolTable();
    }

    public override string ToString()
    {
        return symbolTable.ToString();
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
        Visit(((Program)node).Block);
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
        Symbol typeSymbol = symbolTable.LookUp(typeName);
        if (typeSymbol == null)
            throw new Exception($"Undefined Type {typeName}");
        string varName = ((Var)varDecl.VarNode).Value;
        var varSymbol = new VarSymbol(varName, typeSymbol);
        
        if (symbolTable.LookUp(varName) is not null)
        {
            throw new Exception($"Error: Duplicate identifier '{varName}' found");
        }
        symbolTable.Insert(varSymbol);
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
        var varSymbol = symbolTable.LookUp(varNode.Value);
        if (varSymbol == null)
            throw new Exception($"Error: Symbol(identifier) not found {varNode.Value}");
        return null;
    }
}