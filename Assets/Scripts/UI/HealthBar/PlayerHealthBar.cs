using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour {
     
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    private Damageable damageable;


    private void Start() {
        if (damageable == null) {
            // Assuming Player is a singleton
            if (Player.Instance != null) {
                damageable = Player.Instance.GetComponent<Damageable>();
            }
        }

        if (damageable != null) {
            damageable.healthChanged += Damageable_healthChanged;
        } else {
            Debug.LogWarning("Damageable component not found on Player.");
        }
        MaxHealth(damageable);
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

    private void MaxHealth(Damageable damageable) {
        for (int i = 0; i < hearts.Length; i++) {
            if (i < damageable.MaxHealth) {
                hearts[i].enabled = true;
            } else {
                hearts[i].enabled = false;
            }
        }
    }



    private void Update() {
    }
}
