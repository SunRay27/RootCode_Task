using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Display logic of Connection
/// </summary>
public class ConnectionView : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro weightText;

    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private Gradient overlapColorGradient;
    public void Init(Vector3 point1, Vector3 point2, int weight)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, point1);
        lineRenderer.SetPosition(1, point2);

        weightText.text = weight.ToString();
        weightText.transform.position = point1 / 2 + point2 / 2;

        SetColor(0);
    }

    public void SetColor(int overlapCount)
    {
        // We should restrict selection ability if overlap count is too big
        const int maxReasonableOverlapCount = 5;
        float t = overlapCount / (maxReasonableOverlapCount - 1f);
        lineRenderer.startColor = lineRenderer.endColor = overlapColorGradient.Evaluate(t);
    }
}
