using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class RestartGame : MonoBehaviour {
    private void OnEnable() {
        SceneManager.LoadScene(SceneStrings.Level1);
    }
}

