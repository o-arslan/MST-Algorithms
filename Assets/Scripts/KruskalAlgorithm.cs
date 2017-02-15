using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class KruskalAlgorithm : MonoBehaviour, IAlgorithm
{
    public List<float> Distances { get; set; }
    public List<TimeSpan> Times { get; set; }

    public KruskalAlgorithm()
    {
        Distances = new List<float>();
        Times = new List<TimeSpan>();
    }

    public AlgorithmTypes GetAlgorithmType()
    {
        return AlgorithmTypes.KruskalsAlgorithm;
    }
    public List<Connect> CalculateRoute()
    {
        Graph graph = CreateGraph();
        int verticesCount = graph.VerticesCount;
        Edge[] result = new Edge[verticesCount];
        int i = 0;
        int e = 0;

        Array.Sort(graph.Edges, delegate (Edge a, Edge b)
        {
            return a.Weight.CompareTo(b.Weight);
        });

        Subset[] subsets = new Subset[verticesCount];

        for (int v = 0; v < verticesCount; ++v)
        {
            subsets[v].Parent = v;
            subsets[v].Rank = 0;
        }

        while (e < verticesCount - 1)
        {
            Edge nextEdge = graph.Edges[i++];
            int x = Find(subsets, nextEdge.Source);
            int y = Find(subsets, nextEdge.Destination);

            if (x != y)
            {
                result[e++] = nextEdge;
                Union(subsets, x, y);
            }
        }

        Times.Add(gameObject.GetComponent<AlgorithmManager>().Stopwatch.Elapsed);
        Debug.Log("Kruskal calculation: " + Times[Times.Count - 1]);
        var Connections = gameObject.GetComponent<AlgorithmManager>().Connections;
        var cons = new List<Connect>();
        foreach (var edge in result)
        {
            cons.Add(new Connect(Connections[edge.Source], Connections[edge.Destination], edge.Weight));
        }

        return cons;
    }

    public IEnumerator ShowConnectionsDelayed(List<Connect> Connections)
    {
        float distance = 0;
        for (var i = 0; i < Connections.Count; i++)
        {
            var startPos = Connections[i].Source.transform.position;
            var endPos = Connections[i].Target.transform.position;
            distance += Connections[i].Distance;

            gameObject.GetComponent<AlgorithmManager>().ConnectPoints(startPos, endPos);

            yield return new WaitForSeconds(gameObject.GetComponent<AlgorithmManager>().DrawDelay);
        }

        Distances.Add(distance);
        gameObject.GetComponent<AlgorithmManager>().Stopwatch.Stop();
        Debug.Log("Kruskal distance: " + distance + ", time taken: " + Times[Times.Count - 1]);

        gameObject.GetComponent<AlgorithmManager>().iterationDone = true;
    }

    public void LogAverage()
    {
        var KruskalAvgDis = Distances.Average();
        UnityEngine.Debug.Log("Kruskal average Distance: " + KruskalAvgDis);
        var KruskalAvgTime = Times.Average(avg => avg.Ticks);
        UnityEngine.Debug.Log("Kruskal average Time: " + KruskalAvgTime + " ms");
    }

    public Graph CreateGraph()
    {
        var Connections = gameObject.GetComponent<AlgorithmManager>().Connections;
        var verticesCount = Connections.Count;
        var edgesCount = verticesCount + 1;
        Graph graph = new Graph();
        graph.VerticesCount = verticesCount;
        graph.EdgesCount = edgesCount;
        graph.Edges = new Edge[verticesCount * verticesCount];


        var cpCount = Connections.Count;
        var cur = 0;
        for (var y = 0; y < cpCount; y++)
        {
            var startPos = Connections[y].transform.position;
            for (var x = 0; x < cpCount; x++)
            {
                var endPos = Connections[x].transform.position;
                var dist = Mathf.Sqrt(Mathf.Pow(endPos.x - startPos.x, 2) + Mathf.Pow(endPos.y - startPos.y, 2) + Mathf.Pow(endPos.z - startPos.z, 2));
                graph.Edges[cur].Source = y;
                graph.Edges[cur].Destination = x;
                graph.Edges[cur].Weight = dist;
                cur++;
            }
        }

        return graph;
    }

    private int Find(Subset[] subsets, int i)
    {
        if (subsets[i].Parent != i)
            subsets[i].Parent = Find(subsets, subsets[i].Parent);

        return subsets[i].Parent;
    }

    private void Union(Subset[] subsets, int x, int y)
    {
        int xroot = Find(subsets, x);
        int yroot = Find(subsets, y);

        if (subsets[xroot].Rank < subsets[yroot].Rank)
            subsets[xroot].Parent = yroot;
        else if (subsets[xroot].Rank > subsets[yroot].Rank)
            subsets[yroot].Parent = xroot;
        else
        {
            subsets[yroot].Parent = xroot;
            ++subsets[xroot].Rank;
        }
    }
}

public struct Edge
{
    public int Source;
    public int Destination;
    public float Weight;
}

public struct Graph
{
    public int VerticesCount;
    public int EdgesCount;
    public Edge[] Edges;
}

public struct Subset
{
    public int Parent;
    public int Rank;
}
