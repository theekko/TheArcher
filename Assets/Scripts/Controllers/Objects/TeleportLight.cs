using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class TeleportLight : MonoBehaviour {
    private Light2D teleportLight;
    [SerializeField] private float maxLightIntensity = 2f;
    [SerializeField] private float maxOuterRadius = 5f;

    private void Awake() {
        teleportLight = GetComponentInChildren<Light2D>();

        // Ensure the light is initially off
        if (teleportLight != null) {
            teleportLight.intensity = 0f;
            teleportLight.pointLightOuterRadius = 0f;
        }
    }

    public void TriggerLightEffect(float teleportTime) {
        if (teleportLight != null) {
            StartCoroutine(AnimateLight(teleportTime));
        }
    }

    private IEnumerator AnimateLight(float duration) {
        float elapsedTime = 0f;

        // Gradually increase light intensity and radius
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            teleportLight.intensity = Mathf.Lerp(0f, maxLightIntensity, t);
            teleportLight.pointLightOuterRadius = Mathf.Lerp(0f, maxOuterRadius, t);

            yield return null;
        }

        // After teleportation is complete, turn off the light
        teleportLight.intensity = 0f;
        teleportLight.pointLightOuterRadius = 0f;
    }
}
