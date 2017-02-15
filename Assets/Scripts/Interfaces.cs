using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public interface IAlgorithm
{
    List<float> Distances { get; set; }
    List<TimeSpan> Times { get; set; }
    AlgorithmTypes GetAlgorithmType();
    List<Connect> CalculateRoute();
    IEnumerator ShowConnectionsDelayed(List<Connect> Connections);

    void LogAverage();
}

public enum AlgorithmTypes
{
    DefaultAlgorithm = 0,
    PrimsAlgorithm = 1,
    KruskalsAlgorithm = 2
}

public class Connect
{
    public GameObject Source { get; set; }
    public GameObject Target { get; set; }
    public float Distance { get; set; }

    public Connect(GameObject source, GameObject target, float distance)
    {
        Source = source;
        Target = target;
        Distance = distance;
    }
}