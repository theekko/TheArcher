using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StreakTimer : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float maxTime;
    [SerializeField] private float blinkSpeed = 0.5f;
    [SerializeField] private float timeToStartBlink = 3;
    private float remainingTime;
    private float timeOfHit;
    private bool isBlinking = false;
    private float blinkTimer = 0f;

    public event EventHandler<TimeOfHitEventArgs> OnTimeOfHitChanged;
    public class TimeOfHitEventArgs : EventArgs {
        public float TimeOfHit;
    }

    private void Awake() {
        remainingTime = maxTime;
        EmpoweredArrow.objectHitEvent += EmpoweredArrow_objectHitEvent;
        EnemyArmor.empoweredArrowHitEvent += EnemyArmor_empoweredArrowHitEvent;
    }

    public float RemainingTime { 
        get {
            return remainingTime;
        }
    }

    public float TimeOfHit {
        get {
            return timeOfHit;
        }
        private set { 
            timeOfHit = value;
        }
    }

    private void EmpoweredArrow_objectHitEvent(object sender, EventArgs e) {
        TimeOfHit = remainingTime;
        remainingTime = maxTime;
        OnTimeOfHitChanged?.Invoke(this, new TimeOfHitEventArgs { TimeOfHit = TimeOfHit });
    }
    private void EnemyArmor_empoweredArrowHitEvent(object sender, EventArgs e) {
        TimeOfHit = remainingTime;
        remainingTime = maxTime;
        OnTimeOfHitChanged?.Invoke(this, new TimeOfHitEventArgs { TimeOfHit = TimeOfHit });
    }


    private void Update() {
        if (remainingTime > 0) {
            remainingTime -= Time.deltaTime;
        }

        if (remainingTime <= timeToStartBlink && !isBlinking) {
            if (remainingTime <= 0) {
                remainingTime = 0;
            }
            timerText.color = Color.red;
            isBlinking = true;
        } else if (remainingTime > 3 && isBlinking) {
            timerText.color = Color.white; 
            timerText.enabled = true;
            isBlinking = false;
        }

        if (isBlinking) {
            BlinkTimerText();
        }

        // Convert the remaining time into seconds and milliseconds
        int seconds = Mathf.FloorToInt(Mathf.Max(remainingTime, 0)); // Get whole seconds
        int milliseconds = Mathf.FloorToInt((Mathf.Max(remainingTime, 0) * 1000) % 1000); // Get milliseconds

        // Display the time in seconds and milliseconds format (e.g., 12.345)
        timerText.text = string.Format("{0:00}.{1:000}", seconds, milliseconds);
    }

    private void BlinkTimerText() {

        blinkTimer += Time.deltaTime;


        if (blinkTimer >= blinkSpeed) {
            timerText.enabled = !timerText.enabled;
            blinkTimer = 0f;
        }
    }
}
