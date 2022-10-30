using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays in-game info about stage and total score
/// Displays stage clear panel
/// </summary>
public class GameInfoView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText, stageText;

    [Header("Stage clear panel")]
    [SerializeField]
    private GameObject stageClearPanel;

    [SerializeField]
    private TextMeshProUGUI stageEndScoreText, stageEndPathWeightText, stageEndText;

    [SerializeField]
    private Button stageEndNextButton;

    public Action onNextButtonClick;

    private void Start()
    {
        stageEndNextButton.onClick.AddListener(() => { onNextButtonClick?.Invoke(); HideStageResults(); });
        HideStageResults();
    }

    public void DisplayStage(int value)
    {
        stageText.text = value.ToString();
    }
    public void DisplayScore(int value)
    {
        scoreText.text = value.ToString();
    }

    public void DisplayStageResults(int pathWeight, int scoreDelta)
    {
        stageClearPanel.SetActive(true);

        stageEndPathWeightText.text = pathWeight.ToString();
        stageEndScoreText.text = scoreDelta.ToString();
        stageEndText.text = stageText.text;


    }
    public void HideStageResults()
    {
        stageClearPanel.SetActive(false);
    }
}
