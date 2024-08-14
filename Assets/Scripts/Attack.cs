using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private Vector2 knockback = Vector2.zero;
    [SerializeField] private Damageable attackSource;


    // This happens when something enters the Collider2D while it is enabled
    private void OnTriggerEnter2D(Collider2D collision) {
        // See if it can be hit
        // Could be better to have an intrface instead of a specific script
        // to implement different types of damage
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null & attackSource.IsAlive) {

            Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);
            bool gotHit = damageable.Hit(attackDamage, deliveredKnockback);

        }
    }
}
