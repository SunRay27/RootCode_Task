using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;


/// <summary>
/// Loads info from MapGenerator and manages all map interactions
/// </summary>
public partial class MapController : MonoBehaviour
{
    [SerializeField]
    private ConnectionView connectionPrefab;

    [SerializeField]
    private MapPointView pointPrefab, startPointPrefab, endPointPrefab;

    [SerializeField]
    private MapPoint startPoint, endPoint;

    [SerializeField]
    private MapPlayerView playerView;

    //called when player reaches end point
    public Action<int> onStageClear;



    //map info
    private List<MapPoint> allPoints = new List<MapPoint>();
    private List<Connection> connections = new List<Connection>();

    //current path 
    private PathStack pathStack = new PathStack();
    private int currentPathWeight = 0;

    //selected point info
    private MapPointView selectedPointView;
    private int selectedPointIndex = -1;

    private bool destinationReached = false;



    private void Start()
    {
        playerView.onMoveEnd += OnPlayerMovementEnded;
    }
    private void Update()
    {
        if (destinationReached)
            return;
        if (UIUtils.IsPointerOverUIObject())
            return;


        //raycast selection
        if (Input.GetKeyDown(KeyCode.Mouse0))
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                MapPointView point = hit.transform.GetComponent<MapPointView>();
                if (point)
                {
                    if (selectedPointView)
                        selectedPointView.OnDeselect();

                    selectedPointView = point;
                    selectedPointIndex = point.MapPointIndex;
                    point.OnSelect();
                }
            }

        //add/remove point from current path
        if (selectedPointIndex == -1)
            return;

        // Add
        if (Input.GetKeyDown(KeyCode.Q))
        {

            TryAddPointToPath(selectedPointIndex);
        }
        // Remove
        if (Input.GetKeyDown(KeyCode.W))
        {
            TryRemovePointFromPath(selectedPointIndex);

        }
    }

    public void Clean()
    {
        foreach (var item in connections)
            Destroy(item.view.gameObject);

        foreach (var item in allPoints)
            Destroy(item.view.gameObject);

        pathStack.Clear();
        //currentPath.Clear();
        connections.Clear();
        allPoints.Clear();

        selectedPointIndex = -1;
        selectedPointView = null;

        currentPathWeight = 0;
    }
    public void Init()
    {
        destinationReached = false;


        GetComponentsInChildren(allPoints);

        LoadConnections();
        InitView();

        AddPointToPath(allPoints.IndexOf(startPoint));
        SnapPlayerToStart();

    }


    private void LoadConnections()
    {
        int minWeight = 1;
        int maxWeight = 10;

        for (int i = 0; i < allPoints.Count; i++)
        {
            foreach (var connectedPoint in allPoints[i].ConnectedPoints)
            {
                //randomize connection weights
                Connection newConnection = new Connection(allPoints[i], connectedPoint, Random.Range(minWeight, maxWeight + 1));
                if (!connections.Contains(newConnection))
                    connections.Add(newConnection);
            }
        }
    }
    private void InitView()
    {
        for (int i = 0; i < allPoints.Count; i++)
        {
            if (allPoints[i] == startPoint)
                allPoints[i].InitView(this, startPointPrefab, i);
            else if (allPoints[i] == endPoint)
                allPoints[i].InitView(this, endPointPrefab, i);
            else
                allPoints[i].InitView(this, pointPrefab, i);
        }

        for (int i = 0; i < connections.Count; i++)
            connections[i].InitView(connectionPrefab);
    }



    private void OnPlayerMovementEnded()
    {
        if (destinationReached)
            onStageClear?.Invoke(currentPathWeight);
    }
    private void SnapPlayerToStart()
    {
        playerView.ResetAtPosition(startPoint.transform.position);
    }



    //path manipulations
    private void TryRemovePointFromPath(int thisPointIndex)
    {
        if (pathStack.PeekPoint() == thisPointIndex && pathStack.Count != 1)
            RemovePointFromPath(thisPointIndex);
    }
    private void TryAddPointToPath(int thisPointIndex)
    {
        MapPoint newPoint = allPoints[thisPointIndex];
        MapPoint lastPoint = allPoints[pathStack.PeekPoint()];

        if (lastPoint.ConnectedPoints.Contains(newPoint) || newPoint.ConnectedPoints.Contains(lastPoint))
        {
            AddPointToPath(thisPointIndex);

            if (newPoint == endPoint)
                destinationReached = true;
        }
    }
    private void AddPointToPath(int index)
    {
        if (pathStack.Count == 0)
        {
            pathStack.PushPoint(index);
            allPoints[index].view.SetColor(1);
            return;
        }

        MapPoint newPoint = allPoints[index];
        MapPoint lastPathPoint = allPoints[pathStack.PeekPoint()];
        Connection connectionToNewPoint = connections.First(i => (i.point1 == lastPathPoint && i.point2 == newPoint) || (i.point2 == lastPathPoint && i.point1 == newPoint));
        int connectionIndex = connections.IndexOf(connectionToNewPoint);

        currentPathWeight += connectionToNewPoint.weight;
        pathStack.PushPoint(index);
        pathStack.PushConnection(connectionIndex);

        //update point/connection view
        connectionToNewPoint.view.SetColor(pathStack.GetConnectionCount(connectionIndex));
        newPoint.view.SetColor(pathStack.GetPointCount(index));



        //move player to new point
        playerView.QueueMoveToPosition(newPoint.transform.position);

    }
    private void RemovePointFromPath(int index)
    {
        MapPoint pointToDelete = allPoints[pathStack.PopPoint()];
        MapPoint lastPathPoint = allPoints[pathStack.PeekPoint()];
        Connection connectionToOldPoint = connections.First(i => (i.point1 == lastPathPoint && i.point2 == pointToDelete) || (i.point2 == lastPathPoint && i.point1 == pointToDelete));
        int connectionIndex = connections.IndexOf(connectionToOldPoint);

        currentPathWeight -= connectionToOldPoint.weight;
        pathStack.PopConnection();


        //update point/connection view
        connectionToOldPoint.view.SetColor(pathStack.GetConnectionCount(connectionIndex));
        pointToDelete.view.SetColor(pathStack.GetPointCount(index));



        //move player to previous point
        playerView.QueueMoveToPosition(lastPathPoint.transform.position);
    }
}
