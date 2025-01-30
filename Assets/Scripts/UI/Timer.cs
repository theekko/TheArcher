using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Rendering.Universal.Internal;

public class Timer : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float remainingTime;
    [SerializeField] private float blinkSpeed = 0.5f;
    [SerializeField] private Damageable damageable;
    [SerializeField] private float damageTimePenalty = 1f;
    [SerializeField] private MainLevelLoader mainLevelLoader;
    [SerializeField] private float startFontSize = 92;
    [SerializeField] private float finalFontSize = 150;

    public event EventHandler timerRunoutEvent;

    private bool isBlinking = false;
    private bool isPaused = false;
    private float blinkTimer = 0f;

    private void Awake() {
        damageable.damageableHit += Damageable_damageableHit;
        mainLevelLoader.gameOverEvent += MainLevelLoader_gameOverEvent;
    }

    private void MainLevelLoader_gameOverEvent(object sender, EventArgs e) {
        isPaused = true;
        StartCoroutine(AnimateTimerToCenter());
    }

    private void Damageable_damageableHit(object sender, Damageable.OnHitEventArgs e) {
        remainingTime = Mathf.Max(remainingTime - damageTimePenalty, 0);
    }

    private void Update() {
        if (isPaused) return;
        if (remainingTime > 0) {
            remainingTime -= Time.deltaTime;
        }

        if (remainingTime <= 0 && !isBlinking) {
            remainingTime = 0;
            timerText.color = Color.red;
            isBlinking = true;
            timerRunoutEvent?.Invoke(this, EventArgs.Empty);
        }

        if (isBlinking) {
            BlinkTimerText();
        }




        int minutes = Mathf.FloorToInt(remainingTime / 60);     
        int seconds = Mathf.FloorToInt(remainingTime % 60);      
        int milliseconds = Mathf.FloorToInt((remainingTime * 1000) % 1000); 

        timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    private void BlinkTimerText() {

        blinkTimer += Time.deltaTime;


        if (blinkTimer >= blinkSpeed) {
            timerText.enabled = !timerText.enabled;
            blinkTimer = 0f;
        }
    }


    IEnumerator AnimateTimerToCenter() {
        timerText.color = Color.red; 
        timerText.fontSize = startFontSize; 


        RectTransform timerRect = timerText.rectTransform;
        Vector3 originalPosition = timerRect.localPosition;
        Vector3 targetPosition = Vector3.zero;

        float animationDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            // Lerp the position and font size
            timerRect.localPosition = Vector3.Lerp(originalPosition, targetPosition, t);
            timerText.fontSize = Mathf.Lerp(startFontSize, finalFontSize, t); 

            yield return null;
        }

        timerRect.localPosition = targetPosition;
        timerText.fontSize = finalFontSize;

        yield break;
    }
}

