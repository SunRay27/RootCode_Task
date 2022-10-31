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

    [SerializeField]
    private Gradient overlapColorGradient;

    public int MapPointIndex { get; private set; }

    public void Init(MapController map, Vector3 pos, int pointIndex)
    {
        rend = GetComponent<Renderer>();
        MapPointIndex = pointIndex;
        transform.position = pos;

        SetColor(0);
        OnDeselect();
    }

    public void SetColor(int overlapCount)
    {
        // We should restrict selection ability if overlap count is too big
        const int maxReasonableOverlapCount = 5;
        float t = overlapCount / (maxReasonableOverlapCount - 1f);
        rend.material.color = overlapColorGradient.Evaluate(t);
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