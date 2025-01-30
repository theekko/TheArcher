using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class TeleportEndAnimator : MonoBehaviour {
    [SerializeField] private Image m_Image;  
    [SerializeField] private Sprite[] m_SpriteArray;
    [SerializeField] private float m_Speed = .02f;
    [SerializeField] private Canvas canvas; 
    [SerializeField] private float delayBeforeStart = 1f; 
    private Vector2 endPosition;
    Coroutine m_CorotineAnim;

    private void Awake() {
        m_Image.enabled = false;
    }

    public void PlayEndAnimation(Vector2 position) {
        endPosition = position;

        // Immediately update the UI position before starting the animation
        UpdateUIPosition(endPosition);

        // Start the coroutine to play the animation after a delay
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
        // Wait before starting the second animation
        yield return new WaitForSeconds(delayBeforeStart);

        m_Image.enabled = true;

        // Play the animation
        for (int i = 0; i < m_SpriteArray.Length; i++) {
            yield return new WaitForSeconds(m_Speed);
            m_Image.sprite = m_SpriteArray[i];
        }
        m_Image.enabled = false;
    }


    private void StopAnimation() {
        if (m_CorotineAnim != null) {
            StopCoroutine(m_CorotineAnim);
        }
        m_Image.enabled = false;
    }
}
