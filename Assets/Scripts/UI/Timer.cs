using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float remainingTime;
    [SerializeField] private float blinkSpeed = 0.5f;

    public event EventHandler timerRunoutEvent;

    private bool isBlinking = false;
    private float blinkTimer = 0f;  
    

    private void Update() {
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


        int minutes = Mathf.FloorToInt(Mathf.Max(remainingTime, 0) / 60);
        int seconds = Mathf.FloorToInt(Mathf.Max(remainingTime, 0) % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void BlinkTimerText() {

        blinkTimer += Time.deltaTime;


        if (blinkTimer >= blinkSpeed) {
            timerText.enabled = !timerText.enabled; 
            blinkTimer = 0f; 
        }
    }
}
