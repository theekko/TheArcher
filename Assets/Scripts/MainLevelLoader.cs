using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class MainLevelLoader : MonoBehaviour {
    [SerializeField] private Animator transition;
    [SerializeField] private float transitionTime = 1f;
    [SerializeField] private float timeForTimer = 3f;
    [SerializeField] private Damageable playerDamageable;
    [SerializeField] private Timer timer;
    private int numEnemiesLeft = 0;
    private bool _timerRunout = false;
    private int totalEnemies = 0;

    public event EventHandler gameOverEvent;

    public bool TimerRunout {
        get {
            return _timerRunout;
        } private set { 
            _timerRunout = value;
        }
    }
    

    private void Start() {
        EnemyCount();
        totalEnemies = numEnemiesLeft;
    }

    private void OnEnable() {
        Damageable.damageableEnemyDeath += Damageable_damageableEnemyDeath;
    }

    private void OnDisable() {
        Damageable.damageableEnemyDeath -= Damageable_damageableEnemyDeath;
    }

    private void Damageable_damageableEnemyDeath(object sender, System.EventArgs e) {
        numEnemiesLeft--;
        if (numEnemiesLeft == 0) {
            StartCoroutine(LoadWinScene());
        } 
    }

    private void EnemyCount() {
        GameObject[] enemies = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in enemies) {
            // Check if the GameObject has a Wasp, Slime, or Shade component
            if (obj.GetComponent<Wasp>() != null ||
                obj.GetComponent<Slime>() != null ||
                obj.GetComponent<Shade>() != null) {
                numEnemiesLeft++;
            }
        }
    }

    private void Update() {
        if (!playerDamageable.IsAlive || (TimerRunout && numEnemiesLeft != totalEnemies)) {
            StartCoroutine(LoadLoseScene());
        } else if (TimerRunout && numEnemiesLeft == totalEnemies) {
            StartCoroutine(LoadSecretWinScene());
        }
    }

    private void Awake() {
        timer.timerRunoutEvent += Timer_timerRunoutEvent;
    }

    private void Timer_timerRunoutEvent(object sender, System.EventArgs e) {
        TimerRunout = true;
    }

    IEnumerator LoadWinScene() {
        gameOverEvent?.Invoke(this, EventArgs.Empty);
        yield return new WaitForSeconds(timeForTimer);
        transition.SetTrigger(AnimatorStrings.levelEnd);
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(SceneStrings.LevelEndWinMovie);
    }

    IEnumerator LoadSecretWinScene() {
        gameOverEvent?.Invoke(this, EventArgs.Empty);
        yield return new WaitForSeconds(timeForTimer);
        transition.SetTrigger(AnimatorStrings.levelEnd);
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(SceneStrings.LevelEndSecretWinMovie);
    }

    IEnumerator LoadLoseScene() {
        yield return new WaitForSeconds(transitionTime);
        transition.SetTrigger(AnimatorStrings.levelEnd);
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(SceneStrings.LevelEndLoseMovie);
    }
}

