using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private UIManager _uiManager;
    private bool _isGameOver = false;
    public bool isPaused = false;

    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UIManager is NULL");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
        {
            SceneManager.LoadScene(1); // load the Game scene
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused == false)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }


    public void PauseGame()
    {
        Time.timeScale = 0f;
        _uiManager.PausePanelShow();
        isPaused = true;

    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        _uiManager.PausePanelHide();
        isPaused = false;
    }

    public void RestartGame()
    {
        if (isPaused == true)
        {
            ResumeGame();
        }

        SceneManager.LoadScene(1); // load the Game scene
    }

    public void ReturnToMainMenu()
    {
        if (isPaused == true)
        {
            ResumeGame();
        }
        SceneManager.LoadScene(0); // load the Main Menu scene
    }

    public void GameOver()
    {
        _isGameOver = true;
    }
}
