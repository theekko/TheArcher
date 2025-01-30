using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private Vector2 knockback = Vector2.zero;
    [SerializeField] private Damageable attackSource;


    private void OnTriggerEnter2D(Collider2D collision) {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null & attackSource.IsAlive) {

            
            float direction = Mathf.Sign(collision.transform.position.x - transform.position.x);
            Vector2 deliveredKnockback = new Vector2(direction * knockback.x, knockback.y);

            bool gotHit = damageable.Hit(attackDamage, deliveredKnockback);

        }
    }
}
