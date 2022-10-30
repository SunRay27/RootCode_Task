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
    //day one - 3hr
    //day two - from 13:00 to ???
    //STAGES
    //1. Map and view creation (weights, connections, view) 1hr
    //2. Map points selection and path construction 1hr
    //3. Map generation ~
    //4. Cube movement ~1hr started at 13:00
    //5. Menuing, stage cycle - finished at 16:40
    //6. main menu - finished at 17:00
    //7. clean-up - after 17:00

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
    private List<MapPoint> currentPath = new List<MapPoint>();
    private int currentPathWeight = 0;

    //selected point info
    private MapPointView selectedPointView;
    private MapPoint selectedPoint;

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
                    selectedPoint = allPoints[point.MapPointIndex];
                    point.OnSelect();
                }
            }

        //add/remove point from current path
        if (Input.GetKeyDown(KeyCode.Space) && selectedPoint != null)
        {
            if (currentPath.Contains(selectedPoint))
                TryRemovePointFromPath(selectedPoint);
            else
                TryAddPointToPath(selectedPoint);
        }
    }

    public void Clean()
    {
        foreach (var item in connections)
            Destroy(item.view.gameObject);

        foreach (var item in allPoints)
            Destroy(item.view.gameObject);

        currentPath.Clear();
        connections.Clear();
        allPoints.Clear();

        selectedPoint = null;
        selectedPointView = null;

        currentPathWeight = 0;
    }
    public void Init()
    {
        destinationReached = false;


        GetComponentsInChildren(allPoints);

        LoadConnections();
        InitView();

        AddPointToPath(startPoint);
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
    private void TryRemovePointFromPath(MapPoint thisPoint)
    {
        if (currentPath.IndexOf(thisPoint) == currentPath.Count - 1 && currentPath.Count != 1)
            RemovePointFromPath(thisPoint);
    }
    private void TryAddPointToPath(MapPoint thisPoint)
    {
        if ((currentPath[currentPath.Count - 1].ConnectedPoints.Contains(thisPoint) || thisPoint.ConnectedPoints.Contains(currentPath[currentPath.Count - 1])) && !currentPath.Contains(thisPoint))
        {
            AddPointToPath(thisPoint);

            if (thisPoint == endPoint)
                destinationReached = true;
        }
    }
    private void AddPointToPath(MapPoint point)
    {
        if (currentPath.Count == 0)
        {
            currentPath.Add(point);
            point.view.SetColor(Color.green);
            return;
        }

        MapPoint lastPathPoint = currentPath[currentPath.Count - 1];
        Connection connectionToNewPoint = connections.First(i => (i.point1 == lastPathPoint && i.point2 == point) || (i.point2 == lastPathPoint && i.point1 == point));

        //update point/connection view
        connectionToNewPoint.view.SetColor(Color.green);
        point.view.SetColor(Color.green);

        currentPathWeight += connectionToNewPoint.weight;
        currentPath.Add(point);

        //move player to new point
        playerView.QueueMoveToPosition(point.transform.position);

    }
    private void RemovePointFromPath(MapPoint point)
    {
        MapPoint lastPathPoint = currentPath[currentPath.Count - 2];
        Connection connectionToOldPoint = connections.First(i => (i.point1 == lastPathPoint && i.point2 == point) || (i.point2 == lastPathPoint && i.point1 == point));

        //update point/connection view
        connectionToOldPoint.view.SetColor(Color.grey);
        point.view.SetColor(Color.white);

        currentPathWeight -= connectionToOldPoint.weight;
        currentPath.Remove(point);

        //move player to previous point
        playerView.QueueMoveToPosition(lastPathPoint.transform.position);
    }
}
