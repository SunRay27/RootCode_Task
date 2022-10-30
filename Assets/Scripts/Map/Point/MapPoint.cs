using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents path point 
/// </summary>
public class MapPoint : MonoBehaviour
{
    [field: SerializeField]
    public List<MapPoint> ConnectedPoints { get; private set; } = new List<MapPoint>();

    public MapPointView view;

    public void InitView(MapController mapController, MapPointView prefab, int index)
    {
        if (view != null)
            Destroy(view.gameObject);

        view = Instantiate(prefab);
        view.Init(mapController, transform.position, index);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach (var item in ConnectedPoints)
        {
            Gizmos.DrawLine(transform.position, item.transform.position);
        }
    }

}
