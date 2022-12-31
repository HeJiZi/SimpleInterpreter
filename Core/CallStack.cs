namespace SimpleInterpreter.Core;

public class CallStack
{
    private Stack<ActivationRecord> _records;

    public CallStack()
    {
        _records = new Stack<ActivationRecord>();
    }

    public void Push(ActivationRecord activationRecord)
    {
        _records.Push(activationRecord);
    }

    public ActivationRecord Pop()
    {
        return _records.Pop();
    }

    public ActivationRecord Peek()
    {
        return _records.Peek();
    }

    public override string ToString()
    {
        return $"CALL STACK\n{string.Join('\n', _records)}\n";
    }
}