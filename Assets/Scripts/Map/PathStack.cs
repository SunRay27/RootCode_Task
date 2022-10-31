using System.Collections.Generic;

public class PathStack
{
    //queue of path indexes
    private Stack<int> pointStack = new Stack<int>();
    private Stack<int> connectionStack = new Stack<int>();
    // index - count
    private Dictionary<int, int> pointCounts = new Dictionary<int, int>();
    private Dictionary<int, int> connectionCounts = new Dictionary<int, int>();

    public int Count => pointStack.Count;

    public int PeekPoint()
    {
        return pointStack.Peek();
    }
    public int PeekConnection()
    {
        return connectionStack.Peek();
    }
    public int PopPoint()
    {
        int lastIndex = pointStack.Pop();

        pointCounts[lastIndex] -= 1;

        return lastIndex;
    }
    public int PopConnection()
    {
        int lastIndex = connectionStack.Pop();

        connectionCounts[lastIndex] -= 1;

        return lastIndex;
    }
    public void PushPoint(int index)
    {
        pointStack.Push(index);

        if (pointCounts.ContainsKey(index))
            pointCounts[index]++;
        else
            pointCounts.Add(index, 1);
    }

    public void PushConnection(int index)
    {
        connectionStack.Push(index);

        if (connectionCounts.ContainsKey(index))
            connectionCounts[index]++;
        else
            connectionCounts.Add(index, 1);
    }
    public int GetPointCount(int index)
    {
        return pointCounts.ContainsKey(index) ? pointCounts[index] : 0;
    }
    public int GetConnectionCount(int index)
    {
        return connectionCounts.ContainsKey(index) ? connectionCounts[index] : 0;
    }
    public void Clear()
    {
        pointStack.Clear();
        pointCounts.Clear();

        connectionCounts.Clear();
        connectionStack.Clear();
    }
}
