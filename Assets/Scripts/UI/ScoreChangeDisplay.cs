using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class ScoreChangeDisplay : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI scoreChangeText;
    [SerializeField] private StreakTimer streakTimer;
    [SerializeField] private Damageable damageable;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float moveDistance = 50f;
    [SerializeField] private Vector2 moveDirection = new Vector2(-1, -1); // Move down and to the left

    private Vector2 initialPosition; // To store the initial position

    private void Awake() {
        initialPosition = scoreChangeText.rectTransform.anchoredPosition; // Store the starting position
        scoreChangeText.enabled = false; // Ensure the text is disabled at the start

        Arrow.objectHitEvent += Arrow_objectHitEvent;
        Arrow.groundHitEvent += Arrow_groundHitEvent;
        Arrow.arrowMissEvent += Arrow_arrowMissEvent;
        EmpoweredArrow.groundHitEvent += EmpoweredArrow_groundHitEvent;
        EmpoweredArrow.empoweredArrowMissEvent += EmpoweredArrow_empoweredArrowMissEvent;
        damageable.damageableHit += Damageable_damageableHit;
        streakTimer.OnTimeOfHitChanged += StreakTimer_OnTimeOfHitChanged;
    }

    

    private void OnDestroy() {
        // Unsubscribe from events to avoid memory leaks
        Arrow.objectHitEvent -= Arrow_objectHitEvent;
        Arrow.groundHitEvent -= Arrow_groundHitEvent;
        Arrow.arrowMissEvent -= Arrow_arrowMissEvent;
        EmpoweredArrow.groundHitEvent -= EmpoweredArrow_groundHitEvent;
        EmpoweredArrow.empoweredArrowMissEvent -= EmpoweredArrow_empoweredArrowMissEvent;
        damageable.damageableHit -= Damageable_damageableHit;
        streakTimer.OnTimeOfHitChanged -= StreakTimer_OnTimeOfHitChanged;
    }

    private void ShowScoreChange(string message, Color color) {
        if (scoreChangeText == null) {
            Debug.LogWarning("scoreChangeText is null!");
            return;
        }
        // Reset the position to the initial position
        scoreChangeText.rectTransform.anchoredPosition = initialPosition;

        scoreChangeText.text = message;
        scoreChangeText.color = color;
        scoreChangeText.enabled = true;
        StartCoroutine(FadeAndMoveText());
    }

    private IEnumerator FadeAndMoveText() {
        // Make sure the text still exists before starting the coroutine
        if (scoreChangeText == null) yield break;

        Vector2 startPos = initialPosition; // Start from the initial position each time
        Vector2 targetPos = startPos + moveDirection * moveDistance;
        float elapsedTime = 0f;

        Color startColor = scoreChangeText.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0); // Fade to transparent

        while (elapsedTime < fadeDuration) {
            if (scoreChangeText == null) yield break;  // Ensure the text exists during the coroutine

            // Lerp position
            scoreChangeText.rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsedTime / fadeDuration);
            // Lerp color (fade)
            scoreChangeText.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it's fully faded and moved at the end
        scoreChangeText.rectTransform.anchoredPosition = targetPos;
        scoreChangeText.color = targetColor;
        scoreChangeText.enabled = false; // Hide the text after the effect
    }


    private void StreakTimer_OnTimeOfHitChanged(object sender, StreakTimer.TimeOfHitEventArgs e) {
        string message = $"3 + {e.TimeOfHit:F2}";
        ShowScoreChange(message, Color.green);
    }

    private void Arrow_objectHitEvent(object sender, EventArgs e) {
        ShowScoreChange("+1", Color.green);
    }

    private void Arrow_groundHitEvent(object sender, EventArgs e) {
        ShowScoreChange("-1", Color.red);
    }

    private void Arrow_arrowMissEvent(object sender, EventArgs e) {
        ShowScoreChange("-1", Color.red);
    }


    private void EmpoweredArrow_groundHitEvent(object sender, EventArgs e) {
        ShowScoreChange("-1", Color.red);
    }

    private void EmpoweredArrow_empoweredArrowMissEvent(object sender, EventArgs e) {
        ShowScoreChange("-1", Color.red);
    }

    private void Damageable_damageableHit(object sender, Damageable.OnHitEventArgs e) {
        ShowScoreChange("-1", Color.red);
    }
}
