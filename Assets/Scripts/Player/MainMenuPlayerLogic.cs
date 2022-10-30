using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Controls cube movement on main menu scene
/// </summary>
public class MainMenuPlayerLogic : MonoBehaviour
{
    private MapPlayerView mapPlayerView;
    private float randomSize;
    private Vector3 velocity;


    private void Start()
    {
        mapPlayerView = GetComponent<MapPlayerView>();
        mapPlayerView.onMoveEnd += SetNewPath;
        SetNewPath();
    }

    private void Update()
    {
        mapPlayerView.transform.localScale = Vector3.SmoothDamp(mapPlayerView.transform.localScale, Vector3.one * randomSize, ref velocity, 0.2f);
    }

    private void SetNewPath()
    {
        randomSize = Random.Range(1f, 4f);

        mapPlayerView.cubeSideSize = randomSize;
        mapPlayerView.stepDuration = Random.Range(0.3f, 1f);

        float horMaxRange = Screen.width * 0.8f;
        float verMaxRange = Screen.height * 0.8f;

        float horMinRange = Screen.width * 0.2f;
        float verMinRange = Screen.height * 0.2f;

        Vector3 randomScreemPos = new Vector3(Random.Range(horMinRange, horMaxRange), Random.Range(verMinRange, verMaxRange));

        if (Physics.Raycast(Camera.main.ScreenPointToRay(randomScreemPos), out RaycastHit info))
            mapPlayerView.QueueMoveToPosition(info.point);
        else
            SetNewPath();
    }
}
