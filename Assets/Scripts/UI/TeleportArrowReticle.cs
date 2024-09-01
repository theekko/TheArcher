using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TeleportArrowReticle : MonoBehaviour {
    [SerializeField] TeleportArrow teleportArrow;
    [SerializeField] BowController bowController;
    [SerializeField] Transform bowEndpointPosition;
    private RectTransform reticlePosition;
    private Canvas canvas;
    private Image reticle;
    private void Awake() {
        reticlePosition = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        reticle = GetComponent<Image>();
    }
    private void UpdateTeleportArrowUI() {
        // Calculate the distance the image should move based on hold time and moveSpeed
        float distance = bowController.DestroyTimer * teleportArrow.MoveSpeed;

        // Calculate the world position of the teleport arrow
        Vector3 worldPosition = bowEndpointPosition.position + bowController.Direction * distance;

        RaycastHit2D hit = Physics2D.Raycast(bowEndpointPosition.position, bowController.Direction, distance, LayerMask.GetMask(LayerStrings.Ground));

        // If ground is detected, stop the reticle at the collision point
        if (hit.collider != null) {
            worldPosition = hit.point;
        }

        // Convert the world position to screen space
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // Convert the screen position to UI position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPosition,
            canvas.worldCamera,
            out Vector2 uiPosition
        );

        // Update the UI element's position
        reticlePosition.anchoredPosition = uiPosition;
    }
    private void Update() {
        if (bowController.DrawSucceedTeleportArrow) {
            reticle.enabled = true;
            UpdateTeleportArrowUI();
        } else { 
            reticle.enabled = false;
        }
    }

}
