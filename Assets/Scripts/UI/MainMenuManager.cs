using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {
    [Header("Menu Objects")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject acknowledgementsCanvas;

    [Header("First selected options")]
    [SerializeField] private GameObject mainMenuFirst;
    [SerializeField] private GameObject acknowledgmentsMenuFirst;

    public void OnPlayPress() {
        SceneManager.LoadScene(SceneStrings.IntroMovie);
    }

    public void OnAcknowledgementPress() {
        mainMenuCanvas.SetActive(false);
        acknowledgementsCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(acknowledgmentsMenuFirst);
    }

    public void OnQuitPress() {
        Application.Quit();
    }

    public void OnBackPress() {
        mainMenuCanvas.SetActive(true);
        acknowledgementsCanvas.SetActive(false);
        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
    }




    private void Start() {
        mainMenuCanvas.SetActive(true);
        acknowledgementsCanvas.SetActive(false);
        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
    }

    

}
