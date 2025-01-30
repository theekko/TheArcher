using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageOverlay : MonoBehaviour {
    [SerializeField] private Damageable damageable;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private float maxAlpha = 0.8f;
    private Image image;  
    private Coroutine fadeCoroutine;  


    private void Awake() {
        image = GetComponent<Image>();
        SetImageAlpha(0);  
        damageable.damageableHit += Damageable_damageableHit;  
    }

    private void Damageable_damageableHit(object sender, Damageable.OnHitEventArgs e) {
        if (fadeCoroutine != null) {
            StopCoroutine(fadeCoroutine);
        }

        
        fadeCoroutine = StartCoroutine(FadeToZeroAlpha());
    }


    private IEnumerator FadeToZeroAlpha() {

        SetImageAlpha(maxAlpha);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration) {
            float currentAlpha = Mathf.Lerp(maxAlpha, 0, elapsedTime / fadeDuration);
            SetImageAlpha(currentAlpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetImageAlpha(0);
    }


    private void SetImageAlpha(float alpha) {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
