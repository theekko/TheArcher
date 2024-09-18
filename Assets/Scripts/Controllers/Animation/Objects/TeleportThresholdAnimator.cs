using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportThresholdAnimator : MonoBehaviour {
    [Header("Rotation Settings")]
    [SerializeField] private float rotateSpeed = 10f;  // Speed of rotation

    [Header("Alpha Settings")]
    [SerializeField] private float minAlpha = 0.2f;    // Minimum alpha value
    [SerializeField] private float maxAlpha = 1f;      // Maximum alpha value
    [SerializeField] private float alphaSpeed = 2f;    // Speed at which alpha transitions

    private Transform teleportThresholdTransform;
    private SpriteRenderer teleportThresholdSprite;
    private float alphaLerpTime = 0f;
    private bool increasingAlpha = true;  // Track if alpha is increasing or decreasing

    private void Awake() {
        teleportThresholdTransform = GetComponent<Transform>();
        teleportThresholdSprite = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        // Rotate the sprite
        RotateSprite();

        // Animate the alpha
        AnimateAlpha();
    }

    private void RotateSprite() {
        // Rotate the object at a constant speed
        teleportThresholdTransform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
    }

    private void AnimateAlpha() {
        // Get the current color of the sprite
        Color color = teleportThresholdSprite.color;

        // Calculate alpha transition speed
        float alphaDelta = alphaSpeed * Time.deltaTime;

        if (increasingAlpha) {
            // Increase alpha
            alphaLerpTime += alphaDelta;

            if (alphaLerpTime >= 1f) {
                alphaLerpTime = 1f;
                increasingAlpha = false;  // Switch to decreasing alpha
            }
        } else {
            // Decrease alpha
            alphaLerpTime -= alphaDelta;

            if (alphaLerpTime <= 0f) {
                alphaLerpTime = 0f;
                increasingAlpha = true;  // Switch to increasing alpha
            }
        }

        // Lerp between minAlpha and maxAlpha based on alphaLerpTime
        color.a = Mathf.Lerp(minAlpha, maxAlpha, alphaLerpTime);

        // Apply the updated color with the new alpha back to the sprite
        teleportThresholdSprite.color = color;
    }
}
