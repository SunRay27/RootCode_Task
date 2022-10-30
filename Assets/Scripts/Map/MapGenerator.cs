using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Simple graph generator
/// </summary>
public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    private MapPoint pointPrefab;

    [SerializeField]
    private MapPoint startPoint;

    [SerializeField]
    private MapPoint endPoint;

    [Header("Total size")]
    [SerializeField]
    private Vector3 mapOffset;
    [SerializeField]
    private Vector2 mapWorldSize;

    [Header("Visual")]
    [SerializeField]
    private Vector2 maxPointRandomOffset;

    [Header("Min sizes")]
    [SerializeField]
    private int minHorizontalNodes = 1;
    [SerializeField]
    private int minVerticalNodes = 1;
    [SerializeField]
    private int minConnectionsPerNode = 1;

    [Header("Max sizes")]
    [SerializeField]
    private int maxHorizontalNodes = 10;
    [SerializeField]
    private int maxVerticalNodes = 10;
    [SerializeField]
    private int maxConnectionsPerNode = 10;


    private List<MapPoint> previousLayer = new List<MapPoint>();

    public void Clean()
    {
        startPoint.ConnectedPoints.Clear();
        endPoint.ConnectedPoints.Clear();
        previousLayer.Clear();

        List<GameObject> toDelete = new List<GameObject>();
        foreach (var item in GetComponentsInChildren<MapPoint>())
            if (item != startPoint && item != endPoint)
                toDelete.Add(item.gameObject);

        foreach (var item in toDelete)
            DestroyImmediate(item);
    }
    public void GenerateMap()
    {
        int totalHorizontalNodes = Random.Range(minHorizontalNodes, maxHorizontalNodes + 1);
        float defaultHorizontalStep = mapWorldSize.x / (totalHorizontalNodes + 2);

        startPoint.transform.position = mapOffset + transform.position + new Vector3(0, 0, mapWorldSize.y / 2) + GetRandomOffset();
        endPoint.transform.position = mapOffset + transform.position + new Vector3(mapWorldSize.x, 0, mapWorldSize.y / 2) + GetRandomOffset();

        previousLayer.Add(startPoint);


        for (int i = 0; i < totalHorizontalNodes; i++)
        {
            float currentHorizontalStep = (i + 1) * defaultHorizontalStep + defaultHorizontalStep / 2;
            int localVerticalNodes = Random.Range(minVerticalNodes, maxVerticalNodes + 1);
            float defaultVerticalStep = mapWorldSize.y / localVerticalNodes;

            List<MapPoint> currentLayer = new List<MapPoint>();
            for (int j = 0; j < localVerticalNodes; j++)
            {
                float currentVerticalStep = (j) * defaultVerticalStep + defaultVerticalStep / 2;

                MapPoint newPoint = Instantiate(pointPrefab, transform);
                newPoint.transform.position = mapOffset + transform.position + new Vector3(currentHorizontalStep, 0, currentVerticalStep) + GetRandomOffset();
                currentLayer.Add(newPoint);
            }

            List<MapPoint> combinedLayer = new List<MapPoint>(previousLayer);
            combinedLayer.AddRange(currentLayer);

            foreach (var item in currentLayer)
            {
                int connections = Random.Range(minConnectionsPerNode, maxConnectionsPerNode + 1);

                //ensure we have connection with previous layer
                item.ConnectedPoints.Add(previousLayer[Random.Range(0, previousLayer.Count)]);

                for (int l = 0; l < connections - 1; l++)
                    item.ConnectedPoints.Add(combinedLayer[Random.Range(0, combinedLayer.Count)]);
            }
            previousLayer = currentLayer;
        }

        int endConnection = Random.Range(minConnectionsPerNode, maxConnectionsPerNode + 1);

        //ensure we have connection with previous layer
        for (int l = 0; l < endConnection; l++)
            endPoint.ConnectedPoints.Add(previousLayer[Random.Range(0, previousLayer.Count)]);



    }

    private Vector3 GetRandomOffset()
    {
        //x - right
        //z - up
        return new Vector3(Random.Range(-maxPointRandomOffset.x, maxPointRandomOffset.x), 0, Random.Range(-maxPointRandomOffset.y, maxPointRandomOffset.y));
    }


    private void OnDrawGizmos()
    {
        Vector3 leftBottom = mapOffset + transform.position;
        Vector3 leftTop = leftBottom + new Vector3(0, 0, mapWorldSize.y);
        Vector3 rightTop = leftTop + new Vector3(mapWorldSize.x, 0, 0);
        Vector3 rightBottom = leftBottom + new Vector3(mapWorldSize.x, 0, 0);
        Gizmos.DrawLine(leftBottom, leftTop);
        Gizmos.DrawLine(leftTop, rightTop);
        Gizmos.DrawLine(rightTop, rightBottom);
        Gizmos.DrawLine(rightBottom, leftBottom);
    }
}
