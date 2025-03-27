using System;
using System.Collections.Generic;
using System.Globalization;

public interface IOperation
{
    string Apply(string operand2);
}
public class Addition : IOperation
{
    public string Apply( string operand2)
    {
        return $"+ {operand2}";
    }
}
public class Differentiation : IOperation
{
    public string Apply(string operand2)
    {
        // Пример простого дифференцирования
        return $"d({operand2})/dx"; // Возвращаем строку с дифференцированием
    }
}
public class Subtraction : IOperation
{
    public string Apply( string operand2)
    {
        return $"- {operand2}";
    }
}
public class Multiplication : IOperation
{
    public string Apply(string operand2)
    {
        return $"* {operand2}";
    }
}
public class Division : IOperation
{
    public string Apply( string operand2)
    {
        return $"/ {operand2}";
    }
}
public class Graph
{
    public Dictionary<int, Node> Nodes { get; set; } // Словарь для хранения узлов по ID
    private int nextId; // Переменная для отслеживания следующего ID
    public Graph()
    {
        Nodes = new Dictionary<int, Node>();
        nextId = 0; // Начальный ID равен 0
    }
    public void AddNode(string name)
    {
        Nodes[nextId] = new Node(nextId, name); // Добавляем новый узел с уникальным ID
        nextId++; // Увеличиваем следующий ID для следующего узла
    }
    public void AddEdge(int fromId, int toId, IOperation operation)
    {
        if (!Nodes.ContainsKey(fromId) || !Nodes.ContainsKey(toId))
        {
            throw new Exception("Both nodes must exist in the graph.");
        }
        var edge = new Edge(Nodes[fromId], Nodes[toId], operation);
        Nodes[fromId].OutgoingEdges.Add(edge);
        Nodes[toId].IncomingEdges.Add(edge);
    }

    public string ToEquation()
    {
        var equationParts = new List<string>();
        var processedNodes = new HashSet<int>();

        foreach (var node in Nodes.Values)
        {
            if (!processedNodes.Contains(node.Id))
            {
                if (node.IncomingEdges.Count == 0)
                {
                    equationParts.Add(node.Name);
                }
                else
                {
                    var currentEquation = new List<string>();

                    foreach (var edge in node.IncomingEdges)
                    {
                        string relation = edge.Operation.Apply( node.Name);
                        currentEquation.Add(relation);
                        processedNodes.Add(edge.From.Id);
                    }

                    equationParts.Add(string.Join(" + ", currentEquation));
                }

                processedNodes.Add(node.Id);
            }
        }

        var uniqueParts = new HashSet<string>(equationParts);
        return string.Join(" ", uniqueParts);
    }

}
public class Node
{
    public int Id { get; set; } // Уникальный идентификатор узла
    public string Name { get; set; } // Имя узла
    public List<Edge> OutgoingEdges { get; set; } // Список рёбер, исходящих из узла
    public List<Edge> IncomingEdges { get; set; } // Список рёбер, входящих в узел
    public Node(int id, string name)
    {
        Id = id; // Присваиваем уникальный идентификатор
        Name = name;
        OutgoingEdges = new List<Edge>();
        IncomingEdges = new List<Edge>(); // Инициализируем список входящих рёбер
    }
}
public class Edge
{
    public Node From { get; set; } // Исходный узел
    public Node To { get; set; } // Целевой узел
    public IOperation Operation { get; set; } // Объект операции

    public Edge(Node from, Node to, IOperation operation)
    {
        From = from;
        To = to;
        Operation = operation;
    }
}
public class Program
{
    public static void Main(string[] args)
    {
        Graph graph = new Graph();
        // Добавление узлов
        graph.AddNode( "1,2");
        graph.AddNode( "x");
        graph.AddNode( "3");
        graph.AddNode( "y");
        graph.AddNode( "1");
        graph.AddNode("t");

        // Добавление рёбер
        graph.AddEdge(0, 1, new Multiplication()); // 1.2 * x
        graph.AddEdge(1, 2, new Addition()); // x + 3
        graph.AddEdge(2, 3, new Multiplication()); // 3 * y
        graph.AddEdge(3, 4, new Addition()); // y + 1
        graph.AddEdge(4, 5, new Differentiation());

        // Преобразование графа в уравнение
        string equation = graph.ToEquation();
        Console.WriteLine("Уравнение: " + equation);
    }
}