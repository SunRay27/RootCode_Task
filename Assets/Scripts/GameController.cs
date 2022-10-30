using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contols game cycle logic
/// </summary>
public class GameController : MonoBehaviour
{
    [SerializeField]
    private MapGenerator mapGenerator;

    [SerializeField]
    private MapController mapController;

    [SerializeField]
    private GameInfoView infoView;


    private int score = 0;
    private int currentStage = 0;


    private void Start()
    {
        mapController.onStageClear += EndStage;
        infoView.onNextButtonClick += StartStage;
        StartStage();
    }

    private void StartStage()
    {
        currentStage++;

        mapController.Clean();
        mapGenerator.Clean();

        mapGenerator.GenerateMap();
        mapController.Init();


        infoView.DisplayStage(currentStage);
        infoView.DisplayScore(score);
    }

    private void EndStage(int pathWeight)
    {
        int scoreDelta = Mathf.FloorToInt(1f / pathWeight * 1000);

        infoView.DisplayStageResults(pathWeight, scoreDelta);
        infoView.DisplayScore(score);

        score += scoreDelta;
    }

}
