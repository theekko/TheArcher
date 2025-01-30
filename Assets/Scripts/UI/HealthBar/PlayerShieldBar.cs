using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShieldBar : MonoBehaviour {

    [SerializeField] private Image[] shieldImage;
    [SerializeField] private Sprite fullShield;
    [SerializeField] private Sprite emptyShield;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Shield shield;
    private void Awake() {
        shield.OnShieldEvent += Shield_OnShieldEvent;
        MaxShield(shield);
    }

    private void Shield_OnShieldEvent(object sender, Shield.OnShieldEventArgs e) {
        for (int i = 0; i < shieldImage.Length; i++) {
            if (i < e.numShields) {
                shieldImage[i].sprite = fullShield;
            } else {
                if (i == e.numShields) {
                    // If this is the shield transitioning from full to empty, animate it
                    StartCoroutine(AnimateShield(shieldImage[i]));
                } else {
                    shieldImage[i].sprite = emptyShield;
                }
            }
        }
    }


    private IEnumerator AnimateShield(Image shield) {
        float duration = animationDuration; 
        float elapsedTime = 0f;
        Vector3 originalScale = shield.rectTransform.localScale;
        Quaternion originalRotation = shield.rectTransform.localRotation;

        while (elapsedTime < duration) {
            float progress = elapsedTime / duration;

            // Scale the shield up and down
            float scale = Mathf.Lerp(1f, 1.5f, Mathf.PingPong(progress * 2f, 1f));
            shield.rectTransform.localScale = originalScale * scale;

            // Rotate the shield back and forth
            float rotationAngle = Mathf.Sin(progress * Mathf.PI * 4f) * 20f; // Oscillating rotation
            shield.rectTransform.localRotation = originalRotation * Quaternion.Euler(0f, 0f, rotationAngle);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset the shield to its original scale and rotation
        shield.rectTransform.localScale = originalScale;
        shield.rectTransform.localRotation = originalRotation;

        // Set the shield to empty after the animation
        shield.sprite = emptyShield;
    }

    private void MaxShield(Shield shield) {
        for (int i = 0; i < shieldImage.Length; i++) {
            if (i < shield.MaxNumShields) {
                shieldImage[i].enabled = true;
            } else {
                shieldImage[i].enabled = false;
            }
        }
    }

    private void Update() {
    }
}
