using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour {
    [SerializeField] private Damageable damageable;
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;



    private void Awake() {
        damageable.healthChanged += Damageable_healthChanged;
        damageable.maxHealthChanged += Damageable_maxHealthChanged;
    }

    private void Damageable_healthChanged(object sender, Damageable.OnHealthChangedEventArgs e) {
        for (int i = 0; i < hearts.Length; i++) {
            if (i < e.health) {
                hearts[i].sprite = fullHeart;
            } else {
                hearts[i].sprite = emptyHeart;
            }
        }
    }

    private void Damageable_maxHealthChanged(object sender, Damageable.OnMaxHealthChangedEventArgs e) {
        for (int i = 0; i < hearts.Length; i++) {
            if (i < e.maxHealth) {
                hearts[i].enabled = true;
            } else {
                hearts[i].enabled = false;
            }
        }
    }



    private void Update() {
    }
}
