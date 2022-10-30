using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls pause menu logic
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    [SerializeField]
    private Button resumeButton, mainMenuButton, exitButton;

    [SerializeField]
    private GameObject content;
    private bool isOpened = false;

    private void Start()
    {
        UpdatePauseMenu();

        resumeButton.onClick.AddListener(() => { isOpened = false; UpdatePauseMenu(); });
        mainMenuButton.onClick.AddListener(() => { SceneManager.LoadScene(0); });
        exitButton.onClick.AddListener(() => { Application.Quit(); });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isOpened = !isOpened;
            UpdatePauseMenu();
        }
    }

    private void UpdatePauseMenu()
    {
        content.SetActive(isOpened);
    }
}
