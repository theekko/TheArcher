using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BowReadinessBar : MonoBehaviour {
    [SerializeField] private BowController bowcontroller;
    [SerializeField] private Transform bowReadinessBarPostion;
    [SerializeField] private Image image;
    [SerializeField] private GameObject sliderGameObject;
    [SerializeField] private Slider slider;
    private Camera mainCamera;
    private Canvas canvas;
    private float drawTime;
    private Color barColor;
    

    private void Awake() {
        canvas = GetComponent<Canvas>();
        drawTime = 0f;
        mainCamera = Camera.main;
    }

    private void Update() {
        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x); 
        transform.localScale = localScale;
        canvas.transform.position = bowReadinessBarPostion.position;
        if (bowcontroller.IsDrawingArrow) {
            sliderGameObject.SetActive(true);
            if (ColorUtility.TryParseHtmlString("#6EABF6", out barColor)) {
                image.color = barColor;
            }
            drawTime += Time.deltaTime;
            slider.value = drawTime / bowcontroller.MinDrawTime;
        } else if (bowcontroller.DrawSucceedTeleportArrow) {
            sliderGameObject.SetActive(true);
            if (ColorUtility.TryParseHtmlString("#F57C6E", out barColor)) {
                image.color = barColor;
            }
            // Update draw time
            drawTime += Time.deltaTime;

            // Calculate linear time based on the full range of draw time
            float linearTime = Mathf.Clamp01((drawTime - bowcontroller.MinDrawTimeTeleportArrow) / (bowcontroller.MaxDrawTime - bowcontroller.MinDrawTimeTeleportArrow)); ; 
            float destroyTimer = Mathf.Lerp(bowcontroller.MinDestroyTimer, bowcontroller.MaxDestroyTimer, linearTime);
            // Set the slider value based on linearTime
            slider.value = destroyTimer/bowcontroller.MaxDestroyTimer;

        } else {
            sliderGameObject.SetActive(false);
            drawTime = 0f;
        }
    }
}
