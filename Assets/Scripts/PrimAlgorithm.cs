using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PrimAlgorithm : MonoBehaviour, IAlgorithm
{
    public List<float> Distances { get; set; }
    public List<TimeSpan> Times { get; set; }

    public PrimAlgorithm()
    {
        Distances = new List<float>();
        Times = new List<TimeSpan>();
    }

    public AlgorithmTypes GetAlgorithmType()
    {
        return AlgorithmTypes.PrimsAlgorithm;
    }

    public List<Connect> CalculateRoute()
    {
        List<GameObject> Connections = gameObject.GetComponent<AlgorithmManager>().Connections;
        float[,] graph = GetDistances();
        int verticesCount = gameObject.GetComponent<AlgorithmManager>().Connections.Count;
        int[] parent = new int[verticesCount];
        float[] key = new float[verticesCount];
        bool[] mstSet = new bool[verticesCount];

        for (int i = 0; i < verticesCount; ++i)
        {
            key[i] = int.MaxValue;
            mstSet[i] = false;
        }

        key[0] = 0;
        parent[0] = -1;

        for (int count = 0; count < verticesCount - 1; ++count)
        {
            int u = MinKey(key, mstSet, verticesCount);
            mstSet[u] = true;

            for (int v = 0; v < verticesCount; ++v)
            {
                if (Convert.ToBoolean(graph[u, v]) && mstSet[v] == false && graph[u, v] < key[v])
                {
                    parent[v] = u;
                    key[v] = graph[u, v];
                }
            }
        }

        Times.Add(gameObject.GetComponent<AlgorithmManager>().Stopwatch.Elapsed);
        Debug.Log("Prim calculation: " + Times[Times.Count - 1]);
        var cons = new List<Connect>();
        for (int i = 1; i < Connections.Count; ++i)
        {
            cons.Add(new Connect(Connections[parent[i]], Connections[i], graph[i, parent[i]]));
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
        Debug.Log("Prim distance: " + distance + ", time taken: " + gameObject.GetComponent<AlgorithmManager>().Stopwatch.Elapsed);

        gameObject.GetComponent<AlgorithmManager>().iterationDone = true;
    }

    public void LogAverage()
    {
        var PrimAvgDis = Distances.Average();
        UnityEngine.Debug.Log("Prim average Distance: " + PrimAvgDis);
        var PrimAvgTime = Times.Average(avg => avg.Ticks);
        UnityEngine.Debug.Log("Prim average Time: " + PrimAvgTime + " ms");
    }

    public float[,] GetDistances()
    {
        var Connections = gameObject.GetComponent<AlgorithmManager>().Connections;
        var cpCount = Connections.Count;
        float[,] distances = new float[cpCount, cpCount];
        for (var y = 0; y < cpCount; y++)
        {
            var startPos = Connections[y].transform.position;
            for (var x = 0; x < cpCount; x++)
            {
                var endPos = Connections[x].transform.position;
                var dist = Mathf.Sqrt(Mathf.Pow(endPos.x - startPos.x, 2) + Mathf.Pow(endPos.y - startPos.y, 2) + Mathf.Pow(endPos.z - startPos.z, 2));
                distances[x, y] = dist;
            }
        }
        return distances;
    }

    private static int MinKey(float[] key, bool[] set, int verticesCount)
    {
        float min = float.MaxValue;
        int minIndex = 0;

        for (int v = 0; v < verticesCount; ++v)
        {
            if (set[v] == false && key[v] < min)
            {
                min = key[v];
                minIndex = v;
            }
        }

        return minIndex;
    }
}
