using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/// <summary>
/// Controls main menu button logic
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private Button exitGameButton;
    [SerializeField]
    private Button startGameButton;

    private void Start()
    {
        exitGameButton.onClick.AddListener(() => { Application.Quit(); });
        startGameButton.onClick.AddListener(() => { SceneManager.LoadScene(1); });
    }

}
