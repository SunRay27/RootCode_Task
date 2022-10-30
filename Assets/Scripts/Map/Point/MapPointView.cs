using UnityEditor;
using UnityEngine;

/// <summary>
/// Display and interaction logic of MapPoint
/// </summary>
public class MapPointView : MonoBehaviour
{
    private Renderer rend;

    [SerializeField]
    private Renderer selection;

    public int MapPointIndex { get; private set; }

    public void Init(MapController map, Vector3 pos, int pointIndex)
    {
        rend = GetComponent<Renderer>();
        MapPointIndex = pointIndex;
        transform.position = pos;

        SetColor(Color.white);
        OnDeselect();
    }

    public void SetColor(Color color)
    {
        rend.material.color = color;
    }

    public void OnSelect()
    {
        selection.enabled = true;
    }
    public void OnDeselect()
    {
        selection.enabled = false;
    }
}