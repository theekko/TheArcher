using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Streak : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI streakText;
    [SerializeField] private Damageable damageable;
    [SerializeField] private StreakTimer streakTimer;
    private float streakScore = 0f;

    private void Awake() {
        Arrow.objectHitEvent += Arrow_objectHitEvent;
        Arrow.groundHitEvent += Arrow_groundHitEvent;
        Arrow.arrowMissEvent += Arrow_arrowMissEvent;
        EmpoweredArrow.groundHitEvent += EmpoweredArrow_groundHitEvent;
        EmpoweredArrow.empoweredArrowMissEvent += EmpoweredArrow_empoweredArrowMissEvent;
        damageable.damageableHit += Damageable_damageableHit;
        streakTimer.OnTimeOfHitChanged += StreakTimer_OnTimeOfHitChanged;

    }

    private void StreakTimer_OnTimeOfHitChanged(object sender, StreakTimer.TimeOfHitEventArgs e) {
        streakScore += (3f + e.TimeOfHit);
    }

    private void Arrow_objectHitEvent(object sender, EventArgs e) {
        streakScore += 1f;
    }


    private void Arrow_groundHitEvent(object sender, EventArgs e) {
        if (streakScore == 0f) {
            streakScore = 0f;
        } else {
            streakScore -= 1f;
        }
    }

    private void Arrow_arrowMissEvent(object sender, EventArgs e) {
        if (streakScore == 0f) {
            streakScore = 0f;
        } else {
            streakScore -= 1f;
        }
    }

 
    private void EmpoweredArrow_groundHitEvent(object sender, EventArgs e) {
        if (streakScore == 0f) {
            streakScore = 0f;
        } else {
            streakScore -= 1f;
        }
    }

    private void EmpoweredArrow_empoweredArrowMissEvent(object sender, EventArgs e) {
        if (streakScore == 0f) {
            streakScore = 0f;
        } else {
            streakScore -= 1f;
        }
    }


    private void Damageable_damageableHit(object sender, Damageable.OnHitEventArgs e) {
        if (streakScore == 0f) {
            streakScore = 0f;
        } else {
            streakScore -= 1f;
        } 
    }


    private void Update() {
        streakText.text = streakScore.ToString("F2");
    }


}
