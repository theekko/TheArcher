using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FloatingHealthBar : MonoBehaviour {
    [SerializeField] private Damageable damageable;
    [SerializeField] private Transform healthBarPostion;
    [SerializeField] private float healthBarTimer = 5f;
    private Camera mainCamera;
    private Slider slider;
    private Canvas canvas;
    private float healthBarTimerMax;
    


    private void Awake() {
        slider = GetComponent<Slider>();
        canvas = GetComponentInParent<Canvas>();
        canvas.enabled = false;
        slider.value = 1;
        damageable.healthChanged += Damageable_healthChanged;
        mainCamera = Camera.main;
        healthBarTimerMax = healthBarTimer;
    }

    private void Damageable_healthChanged(object sender, Damageable.OnHealthChangedEventArgs e) {
        slider.value = (float)e.health / (float)e.maxHealth;
        canvas.enabled = true;
        healthBarTimer = healthBarTimerMax;
    }

    private void Update() {
        transform.rotation = mainCamera.transform.rotation;
        canvas.transform.position = healthBarPostion.position;
        if (canvas.enabled) { 
            healthBarTimer -= Time.deltaTime;
        }
        if (healthBarTimer <= 0) { 
            canvas.enabled = false;
        }
    }
}
