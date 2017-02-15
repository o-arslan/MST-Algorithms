using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmManager : MonoBehaviour
{
    public AlgorithmTypes ConnectionAlgorithm;
    public bool LoopAlgorithms;

    [Range(10, 1000)]
    public int ConnectionCount;

    [Range(10, 100)]
    public int DrawRadius;

    [Range(0, 1)]
    public float DrawDelay;

    [Range(1, 100)]
    public int DrawIterations;
    public GameObject ConnectionPoints;
    public GameObject ConnectionPoint;
    public GameObject ConnectionObjects;
    public GameObject ConnectionObject;
    public List<GameObject> Connections { get; set; }
    public Stopwatch Stopwatch { get; set; }

    // Use this for initialization
    void Start()
    {
        CreateConnectionPoints();
        SetCameraPoisition();
        Camera.main.gameObject.AddComponent<PrimAlgorithm>();
        Camera.main.gameObject.AddComponent<KruskalAlgorithm>();
        Camera.main.gameObject.AddComponent<DefaultAlgorithm>();
        Stopwatch = new Stopwatch();

        IAlgorithm algorithm;
        switch (ConnectionAlgorithm)
        {
            case AlgorithmTypes.PrimsAlgorithm:
                algorithm = gameObject.GetComponent<PrimAlgorithm>();
                break;
            case AlgorithmTypes.KruskalsAlgorithm:
                algorithm = gameObject.GetComponent<KruskalAlgorithm>();
                break;
            default:
                algorithm = gameObject.GetComponent<DefaultAlgorithm>();
                break;
        }

        StartCoroutine(LoopDrawIterations(algorithm));
    }

    public void SetCameraPoisition()
    {
        var axisLoc = DrawRadius / 2;
        var cameraPos = new Vector3(axisLoc, axisLoc, axisLoc - (axisLoc * 3));
        Camera.main.transform.position = cameraPos;
    }

    public void CreateConnectionPoints()
    {
        Connections = new List<GameObject>();
        var r = new System.Random();
        for (var i = 0; i < ConnectionCount; i++)
        {
            var randomPosition = new Vector3(r.Next(DrawRadius), r.Next(DrawRadius), r.Next(DrawRadius));
            var go = (GameObject)Instantiate(ConnectionPoint, randomPosition, Quaternion.identity);
            go.transform.parent = ConnectionPoints.transform;
            // go.GetComponent<Renderer>().material.color = Color.black;
            Connections.Add(go);
        }
    }

    public void ConnectPoints(Vector3 startPos, Vector3 endPos)
    {
        // Calculate distance between two points in 3 Dimensions
        var distance = Mathf.Sqrt(Mathf.Pow(endPos.x - startPos.x, 2) + Mathf.Pow(endPos.y - startPos.y, 2) + Mathf.Pow(endPos.z - startPos.z, 2)) / 2f;

        // Calculate midpoint between two points in 3 Dimensions
        var midpoint = (endPos + startPos) * 0.5f;

        // Create new ConnectionObject
        var connection = (GameObject)Instantiate(ConnectionObject);

        // Set size to distance between two points
        connection.transform.localScale = new Vector3(0.5f, distance, 0.5f);

        // Set position to midpoint
        connection.transform.position = midpoint;

        // Set connection Color
        connection.GetComponent<Renderer>().material.color = Color.yellow;

        // Set rotation to the endpoint
        connection.transform.LookAt(endPos);

        // The LookAt method rotates on the X-axis, so we have to add -90 degrees to the X rotation to make the object really connection
        // This could be fixed by having the prefab default be in the laying position, but you cannot Freeze transitions in Unity.
        connection.transform.Rotate(-90, 0, 0);
        connection.transform.parent = ConnectionObjects.transform;
    }

    public bool iterationDone { get; set; }
    IEnumerator LoopDrawIterations(IAlgorithm algorithm)
    {
        var curIteration = 0;
        var initialAlgorithm = algorithm;
        while (curIteration < DrawIterations)
        {
            iterationDone = false;
            GenerateConnections(algorithm);

            if (LoopAlgorithms)
            {
                switch (algorithm.GetAlgorithmType())
                {
                    case AlgorithmTypes.DefaultAlgorithm:
                        algorithm = gameObject.GetComponent<PrimAlgorithm>();
                        break;
                    case AlgorithmTypes.PrimsAlgorithm:
                        algorithm = gameObject.GetComponent<KruskalAlgorithm>();
                        break;
                    case AlgorithmTypes.KruskalsAlgorithm:
                        algorithm = gameObject.GetComponent<DefaultAlgorithm>();
                        break;
                }

                if (algorithm == initialAlgorithm)
                    curIteration++;
            }
            else
            {
                curIteration++;
            }

            yield return new WaitUntil(() => iterationDone);
        }

        LogAverage(LoopAlgorithms ? null : algorithm);
    }

    public void GenerateConnections(IAlgorithm algorithm)
    {
        ClearConnections();
        Stopwatch.Reset();
        Stopwatch.Start();

        var cons = algorithm.CalculateRoute();
        StartCoroutine(algorithm.ShowConnectionsDelayed(cons));
    }

    public void ClearConnections()
    {
        foreach (Transform child in ConnectionObjects.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void LogAverage(IAlgorithm algorithm)
    {
        UnityEngine.Debug.Log("--------------------");

        if (algorithm != null)
        {
            algorithm.LogAverage();
        }
        else
        {
            gameObject.GetComponent<DefaultAlgorithm>().LogAverage();
            UnityEngine.Debug.Log("--------------------");
            gameObject.GetComponent<PrimAlgorithm>().LogAverage();
            UnityEngine.Debug.Log("--------------------");
            gameObject.GetComponent<KruskalAlgorithm>().LogAverage();
        }
    }
}
