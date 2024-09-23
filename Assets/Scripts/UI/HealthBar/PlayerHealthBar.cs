using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour {

    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private float animationDuration = 0.5f;
    private Damageable damageable;

    private void Start() {
        if (damageable == null) {
            // Assuming Player is a singleton
            if (Player.Instance != null) {
                damageable = Player.Instance.GetComponent<Damageable>();
            }
        }

        if (damageable != null) {
            damageable.healthChanged += Damageable_healthChanged;
        } else {
            Debug.LogWarning("Damageable component not found on Player.");
        }
        MaxHealth(damageable);
    }

    private void Damageable_healthChanged(object sender, Damageable.OnHealthChangedEventArgs e) {
        for (int i = 0; i < hearts.Length; i++) {
            if (i < e.health) {
                hearts[i].sprite = fullHeart;
            } else {
                if (i == e.health) {
                    // If this is the heart transitioning from full to empty, animate it
                    StartCoroutine(AnimateHeart(hearts[i]));
                } else {
                    hearts[i].sprite = emptyHeart;
                }
            }
        }
    }

    private IEnumerator AnimateHeart(Image heart) {
        float duration = animationDuration; // Duration of the animation
        float elapsedTime = 0f;
        Vector3 originalScale = heart.rectTransform.localScale;
        Quaternion originalRotation = heart.rectTransform.localRotation;

        while (elapsedTime < duration) {
            float progress = elapsedTime / duration;

            // Scale the heart up and down
            float scale = Mathf.Lerp(1f, 1.5f, Mathf.PingPong(progress * 2f, 1f));
            heart.rectTransform.localScale = originalScale * scale;

            // Rotate the heart back and forth
            float rotationAngle = Mathf.Sin(progress * Mathf.PI * 4f) * 20f; // Oscillating rotation
            heart.rectTransform.localRotation = originalRotation * Quaternion.Euler(0f, 0f, rotationAngle);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset the heart to its original scale and rotation
        heart.rectTransform.localScale = originalScale;
        heart.rectTransform.localRotation = originalRotation;

        // Set the heart to empty after the animation
        heart.sprite = emptyHeart;
    }

    private void MaxHealth(Damageable damageable) {
        for (int i = 0; i < hearts.Length; i++) {
            if (i < damageable.MaxHealth) {
                hearts[i].enabled = true;
            } else {
                hearts[i].enabled = false;
            }
        }
    }

    private void Update() {
    }
}
