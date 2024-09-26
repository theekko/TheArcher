using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleportStartAnimator : MonoBehaviour {
    [SerializeField] private Image m_Image;  // Image for first animation
    [SerializeField] private Sprite[] m_SpriteArray;
    [SerializeField] private float m_Speed = .02f;
    [SerializeField] private Canvas canvas; // Reference to your canvas
    private Vector2 startPosition;
    Coroutine m_CorotineAnim;

    private void Awake() {
        m_Image.enabled = false;
    }

    public void PlayStartAnimation(Vector2 position) {
        startPosition = position;
        m_Image.enabled = true;  // Turn on the Image

        // Immediately update the UI position before starting the animation
        UpdateUIPosition(startPosition);

        // Start the coroutine to play the animation
        m_CorotineAnim = StartCoroutine(PlayAnimation());
    }

    // Update the UI position method to set the RectTransform position
    private void UpdateUIPosition(Vector2 position) {
        // Convert the world position to UI position
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPosition,
            canvas.worldCamera,
            out Vector2 uiPosition
        );

        // Update the UI element's position immediately
        m_Image.rectTransform.anchoredPosition = uiPosition;
    }

    IEnumerator PlayAnimation() {
        // Play the animation
        for (int i = 0; i < m_SpriteArray.Length; i++) {
            yield return new WaitForSeconds(m_Speed);
            m_Image.sprite = m_SpriteArray[i];
        }

        // Turn off the Image after the animation ends
        m_Image.enabled = false;
    }


    private void StopAnimation() {
        if (m_CorotineAnim != null) {
            StopCoroutine(m_CorotineAnim);
        }
        m_Image.enabled = false; // Ensure the image is turned off if stopped
    }
}
