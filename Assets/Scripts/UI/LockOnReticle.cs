using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LockOnReticle : MonoBehaviour {
    [SerializeField] private BowController bowController;
    private Image lockOnReticle;
    private Canvas canvas;


    private void Awake() {
        lockOnReticle = GetComponent<Image>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void LockOnEnemy() {
        Vector3 worldPosition = bowController.ClosestEnemy.transform.position;

        // Convert the world position to screen space
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // Convert the screen position to UI position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPosition,
            canvas.worldCamera,
            out Vector2 uiPosition
        );
        lockOnReticle.rectTransform.anchoredPosition = uiPosition;
    }
    private void Update() {
        if (bowController.ClosestEnemy != null && bowController.IsDrawingArrow) {
            lockOnReticle.enabled = true;
            LockOnEnemy();
        } else {
            lockOnReticle.enabled = false;
        }
    }
}
