using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DefaultAlgorithm : MonoBehaviour, IAlgorithm
{
    public List<float> Distances { get; set; }
    public List<TimeSpan> Times { get; set; }

    public DefaultAlgorithm()
    {
        Distances = new List<float>();
        Times = new List<TimeSpan>();
    }

    public AlgorithmTypes GetAlgorithmType()
    {
        return AlgorithmTypes.DefaultAlgorithm;
    }

    public List<Connect> CalculateRoute()
    {
        var Connections = gameObject.GetComponent<AlgorithmManager>().Connections;
        var cons = new List<Connect>();
        for (var i = 1; i < Connections.Count; ++i)
        {
            var startPos = Connections[i - 1].transform.position;
            var endPos = Connections[i].transform.position;
            var distance = Mathf.Sqrt(Mathf.Pow(endPos.x - startPos.x, 2) + Mathf.Pow(endPos.y - startPos.y, 2) + Mathf.Pow(endPos.z - startPos.z, 2));
            cons.Add(new Connect(Connections[i - 1], Connections[i], distance));
        }

        Times.Add(gameObject.GetComponent<AlgorithmManager>().Stopwatch.Elapsed);
        Debug.Log("Default calculation: " + Times[Times.Count - 1]);
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
        Debug.Log("Default distance: " + distance + ", time taken: " + gameObject.GetComponent<AlgorithmManager>().Stopwatch.Elapsed);

        gameObject.GetComponent<AlgorithmManager>().iterationDone = true;
    }

    public void LogAverage()
    {
        var defaultAvgDis = Distances.Average();
        UnityEngine.Debug.Log("Default average Distance: " + defaultAvgDis);
        var defaultAvgTime = Times.Average(avg => avg.Ticks);
        UnityEngine.Debug.Log("Default average Time: " + defaultAvgTime + " ms");
    }
}
