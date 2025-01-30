using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class DamageTimePenalty : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI timeChangeText;
    [SerializeField] private Damageable damageable;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float moveDistance = 50f;
    [SerializeField] private Vector2 moveDirection = new Vector2(-1, -1); 
    [SerializeField] private string subtractionTime = "-1 sec";

    private Vector2 initialPosition;

    private void Awake() {
        initialPosition = timeChangeText.rectTransform.anchoredPosition; 
        timeChangeText.enabled = false; 
        damageable.damageableHit += Damageable_damageableHit;
    }



    private void OnDestroy() {
        damageable.damageableHit -= Damageable_damageableHit;
    }

    private void ShowScoreChange(string message, Color color) {
        if (timeChangeText == null) {
            Debug.LogWarning("timeChangeText is null!");
            return;
        }
        timeChangeText.rectTransform.anchoredPosition = initialPosition;

        timeChangeText.text = message;
        timeChangeText.color = color;
        timeChangeText.enabled = true;
        StartCoroutine(FadeAndMoveText());
    }

    private IEnumerator FadeAndMoveText() {
        if (timeChangeText == null) yield break;

        Vector2 startPos = initialPosition; 
        Vector2 targetPos = startPos + moveDirection * moveDistance;
        float elapsedTime = 0f;

        Color startColor = timeChangeText.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0); 

        while (elapsedTime < fadeDuration) {
            if (timeChangeText == null) yield break; 

            // Lerp position
            timeChangeText.rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsedTime / fadeDuration);
            // Lerp color (fade)
            timeChangeText.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it's fully faded and moved at the end
        timeChangeText.rectTransform.anchoredPosition = targetPos;
        timeChangeText.color = targetColor;
        timeChangeText.enabled = false; 
    }

    private void Damageable_damageableHit(object sender, Damageable.OnHitEventArgs e) {
        ShowScoreChange(subtractionTime, Color.red);
    }
}