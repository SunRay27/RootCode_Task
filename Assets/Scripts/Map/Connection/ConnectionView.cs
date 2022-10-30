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
    public void Init(Vector3 point1, Vector3 point2, int weight)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, point1);
        lineRenderer.SetPosition(1, point2);

        weightText.text = weight.ToString();
        weightText.transform.position = point1 / 2 + point2 / 2;

        SetColor(Color.grey);
    }

    public void SetColor(Color color)
    {
        lineRenderer.startColor = lineRenderer.endColor = color;
    }
}
