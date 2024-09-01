using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
    [Header("Menu Objects")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject settingsCanvas;

    [Header("Player scripts to be deactivated during pause")]
    [SerializeField] private Player player;
    [SerializeField] private BowController bowController;

    [Header("First selected options")]
    [SerializeField] private GameObject mainMenuFirst;

    private bool isPaused = false;

    private void Start() { 
        mainMenuCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
    }

    private void Pause() {
        isPaused = true;
        Time.timeScale = 0f;
        player.enabled = false;
        bowController.enabled = false;
        OpenMainMenu();
    }

    private void Unpause() {
        isPaused = false;
        Time.timeScale = 1f;
        player.enabled = true;
        bowController.enabled = true;
        CloseAllMenus();
    }

    private void OpenMainMenu() { 
        mainMenuCanvas.SetActive(true);
        settingsCanvas.SetActive(false);

        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
    }

    private void CloseAllMenus() {
        mainMenuCanvas.SetActive(false);
        settingsCanvas.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
    }

    #region Main Menu Button Actions

    public void OnResumePress() {
        Unpause();
    }

    public void OnRestartPress() { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Unpause();
    }

    public void OnQuitPress() {
        Application.Quit();
    }

    #endregion




    public void OnMenuOpenClose(InputAction.CallbackContext context) {

        if (context.performed) {
            if (!isPaused) {
                Pause();
            } else {
                Unpause();
            }
        }
    }

    void Update() {
        
    }
}
