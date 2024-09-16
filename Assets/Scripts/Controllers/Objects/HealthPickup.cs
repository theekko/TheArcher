using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour {
    [SerializeField] private int healthRestore = 2;

    private void OnTriggerEnter2D(Collider2D collision) {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable) {
            bool wasHealed = damageable.Heal(healthRestore);

            if (wasHealed) {

                Destroy(gameObject);
            }
        }
    }

}
