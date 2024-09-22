using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {
    [Header("Menu Objects")]
    [SerializeField] private GameObject mainMenuCanvas;

    [Header("First selected options")]
    [SerializeField] private GameObject mainMenuFirst;


    public void OnPlayPress() {
        SceneManager.LoadScene(SceneStrings.IntroMovie);
    }

    public void OnQuitPress() {
        Application.Quit();
    }


    private void Start() {
        mainMenuCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
    }

    

}
