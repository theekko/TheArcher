using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    [SerializeField] private int damage = 1;
    [SerializeField] private Vector2 knockback = Vector2.zero;
    [SerializeField] private float destroyTimer = 2f;
    [SerializeField] float moveSpeed = 1f;

    private Vector3 shootDir;

    public void Setup(Vector3 shootDir) {
        this.shootDir = shootDir;
        // Calculate the angle in degrees for the rotation
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;

        // Rotate the arrow to face the shoot direction
        transform.eulerAngles = new Vector3(0, 0, angle);
        Destroy(gameObject, destroyTimer);
    }

    private void Update() {
        transform.position += shootDir * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null) {

            Vector2 deliveredKnockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            bool gotHit = damageable.Hit(damage, deliveredKnockback);

            if (gotHit) {
                Debug.Log("Hit");
                Destroy(gameObject);
            }
        } else {
            Destroy(gameObject);
        }
    }
}
