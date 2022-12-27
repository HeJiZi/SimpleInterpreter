using System.Collections;
using System.Diagnostics;
using System.Reflection;
using SimpleInterpreter.Core;

namespace SimpleInterpreter.Tool;

public static class AstVisualUtil
{
    
    /// <summary>
    /// 简易的可视化抽象语法树，调试用
    /// </summary>
    /// <param name="root"></param>
    public static void PrintTree(AST root)
    {
        Console.WriteLine("\nStart:AST>");

        Queue<AST> currentLevel= new Queue<AST>();
        Queue<AST> nextLevel = new Queue<AST>();
        currentLevel.Enqueue(root);
        int level = 0;
        Console.WriteLine($"==============================Level:{level}==============================");
        while (currentLevel.Count > 0)
        {
            var node = currentLevel.Dequeue();
            var typeInfo = node.GetType();
            var properties = typeInfo.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            string info = $"{node} Children=> ";
            foreach (var propertyInfo in properties)
            {
                object? value = propertyInfo.GetValue(node);
                if( value == null || (value is not AST && value is not ICollection))
                    continue;

                info += PushChildren(nextLevel, propertyInfo, node);
            }
            Console.WriteLine(info);

            if (currentLevel.Count == 0 && nextLevel.Count > 0)
            {
                (currentLevel, nextLevel) = (nextLevel, currentLevel);
                level++;
                Console.WriteLine($"==============================Level:{level}==============================");
            }

        }
    }

    private static string PushChildren(Queue<AST> nextLevel, PropertyInfo propertyInfo, AST node)
    {
        string childInfo = "";
        var value = propertyInfo.GetValue(node);
        if (value == null)
        {
            return childInfo;
        }

        if (value is ICollection collection)
        {
            foreach (var obj in collection)
            {
                if (obj is not AST ast)
                    return childInfo;
                nextLevel.Enqueue(ast);
                childInfo += $"{ast}, ";
            }
            return childInfo;
        }

        var child = (AST)propertyInfo.GetValue(node);
        nextLevel.Enqueue(child);
        childInfo += $"{child}, ";
        return childInfo;

    }
}