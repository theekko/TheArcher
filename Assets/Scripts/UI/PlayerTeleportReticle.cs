using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTeleportReticle : MonoBehaviour {
    [SerializeField] private Player player; // Reference to the Player script
    [SerializeField] private RectTransform reticlePosition; // The UI element (reticle)
    [SerializeField] private Canvas canvas;
    private Image reticle;

    private void Awake() {
        // Get references
        reticlePosition = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        reticle = GetComponent<Image>();
    }

    private void UpdateTeleportReticleUI() {
        // Get the teleport direction from the player
        Vector3 teleportDirection = new Vector3(player.LeftStickInput.x, player.LeftStickInput.y, 0).normalized;
        float teleportDistance = player.TeleportDistance;

        // Set the raycast starting position slightly above the ground to avoid teleporting inside colliders
        Vector3 raycastStartPosition = player.transform.position + Vector3.up * 0.1f;

        // Perform a raycast in the teleport direction
        RaycastHit2D hit = Physics2D.Raycast(raycastStartPosition, teleportDirection, teleportDistance, LayerMask.GetMask(LayerStrings.Ground));

        // Calculate the final teleport position based on whether the raycast hits an object
        Vector3 teleportPosition;
        if (hit.collider != null) {
            // Stop at the collision point and adjust for teleport overlap
            Vector3 hitNormalOffset = -(Vector3)hit.normal * player.TeleportOverlapOffset;
            teleportPosition = (Vector3)hit.point + hitNormalOffset;
        } else {
            // Teleport the full distance if no collision is detected
            teleportPosition = player.transform.position + teleportDirection * teleportDistance;
        }

        // Convert world position to screen space
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(teleportPosition);

        // Convert screen space to UI position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPosition,
            canvas.worldCamera,
            out Vector2 uiPosition
        );

        // Update the reticle's UI position
        reticlePosition.anchoredPosition = uiPosition;
    }

    private void Update() {
        // If the player is about to teleport, show the reticle
        if (player.IsTeleporting) {
            reticle.enabled = true;
            UpdateTeleportReticleUI();
        } else {
            reticle.enabled = false;
        }
    }
}