using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour {

    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float heartIncreaseInterval = 5f; // Interval in seconds
    private Damageable damageable;
    private float heartTimer = 0f;
    private bool playerMadeInput = false;
    private int maxHearts = 10;
    private int startingMaxHealth;

    private void Start() {
        if (damageable == null) {
            if (Player.Instance != null) {
                damageable = Player.Instance.GetComponent<Damageable>();
            }
        }

        if (damageable != null) {
            damageable.healthChanged += Damageable_healthChanged;

        } else {
            Debug.LogWarning("Damageable component not found on Player.");
        }
        startingMaxHealth = damageable.MaxHealth;
        MaxHealth(damageable);

        AddHeart();
        heartTimer = 0f;
    }

    private void Update() {
        // Detect player input or movement
        if (Input.anyKeyDown || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
            Debug.Log("player input");
            damageable.MaxHealth = startingMaxHealth;
            playerMadeInput = true;
            ResetToMaxHealth();
        }

        if (!playerMadeInput) {
            heartTimer += Time.deltaTime;

            if (heartTimer >= heartIncreaseInterval && damageable.MaxHealth < maxHearts) {
                AddHeart();
                heartTimer = 0f; // Reset timer after adding a heart
            }
        }
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
        float duration = animationDuration; 
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

    private void AddHeart() {
        damageable.MaxHealth++;
        MaxHealth(damageable);

        if (damageable.MaxHealth <= hearts.Length) {
            Image newHeart = hearts[damageable.MaxHealth - 1]; 
            StartCoroutine(FadeInHeart(newHeart));
        }
    }

    private IEnumerator FadeInHeart(Image heart) {
        float elapsedTime = 0f;
        Color heartColor = heart.color;


        heartColor.a = 0;
        heart.color = heartColor;

        while (elapsedTime < heartIncreaseInterval) {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / heartIncreaseInterval);
            heartColor.a = alpha;
            heart.color = heartColor;
            yield return null;
        }
    }

    private void ResetToMaxHealth() {
        Debug.Log("reset");
        for (int i = 0; i < hearts.Length; i++) {
            if (i < startingMaxHealth) {
                hearts[i].enabled = true;
            } else {
                Debug.Log("i: " + i);
                hearts[i].enabled = false;
            }
        }
    }
}
