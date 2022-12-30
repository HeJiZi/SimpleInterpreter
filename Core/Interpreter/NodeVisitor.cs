namespace SimpleInterpreter.Core;

public class NodeVisitor
{
    private Dictionary<NodeType, Func<AST, dynamic>> _visitMap;
    
    public NodeVisitor()
    {
        _visitMap = new Dictionary<NodeType, Func<AST, dynamic>>()
        {
            { NodeType.Num, VisitNum },
            { NodeType.BinOp, VisitBinOp},
            { NodeType.UnaryOp, VisitUnaryOp},
            { NodeType.Compound, VisitCompound},
            { NodeType.NoOp, VisitNoOp},
            { NodeType.Assign, VisitAssign},
            { NodeType.Var, VisitVar},
            { NodeType.Program, VisitProgram},
            { NodeType.Block, VisitBlock},
            { NodeType.VarDecl, VisitVarDecl},
            { NodeType.Type, VisitType},
            { NodeType.ProcedureDecl, VisitProcedureDecl},
            { NodeType.ProcedureCall, VisitProcedureCall}
        };
    }

    public dynamic Visit(AST node)
    {
        _visitMap.TryGetValue(node.NodeType, out var visitFunc);
        if (visitFunc != null)
        {
            return visitFunc(node);
        }

        throw new Exception($"No visit Method {node.NodeType}");
        return null;
    }

    protected virtual dynamic VisitNum(AST node) { return null; }
    protected virtual dynamic VisitBinOp(AST node) { return null; }
    protected virtual dynamic VisitUnaryOp(AST node) { return null; }
    protected virtual dynamic VisitCompound(AST node) { return null; }
    protected virtual dynamic VisitNoOp(AST node) { return null; }
    protected virtual dynamic VisitAssign(AST node) { return null; }
    protected virtual dynamic VisitVar(AST node) { return null; }
    protected virtual dynamic VisitProgram(AST node) { return null; }
    protected virtual dynamic VisitBlock(AST node) { return null; }
    protected virtual dynamic VisitVarDecl(AST node) { return null; }
    protected virtual dynamic VisitType(AST node) { return null; }
    protected virtual dynamic VisitProcedureDecl(AST node) { return null; }
    protected virtual dynamic VisitProcedureCall(AST node){ return null; }

}