using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {
    [SerializeField] private int damage = 1;
    [SerializeField] private Vector2 knockback = Vector2.zero;
    [SerializeField] private float destroyTimer = 2f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float hitDestroyTimer = 0f;

    private Vector3 shootDir;
    private Vector3 previousPosition;
    private Rigidbody2D rb;
    private bool hasHit = false;

    public void Setup(Vector3 shootDir) {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(shootDir.x, shootDir.y).normalized * moveSpeed, ForceMode2D.Impulse);

        this.shootDir = shootDir;
        this.shootDir.z = 0;
        previousPosition = transform.position;

        // Calculate the angle in degrees for the rotation
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        angle += 180f;

        // Rotate the fireball to face the shoot direction
        transform.eulerAngles = new Vector3(0, 0, angle);
        Destroy(gameObject, destroyTimer);
    }

    private void FixedUpdate() {
        if (hasHit) return; 

        // Perform a raycast from the previous position to the current position
        Vector3 currentPosition = transform.position;
        Vector3 direction = currentPosition - previousPosition;
        float distance = direction.magnitude;

        // Create a LayerMask that matches the collision matrix
        int layerMask = LayerMask.GetMask(LayerStrings.Player, LayerStrings.Ground);

        RaycastHit2D hit = Physics2D.Raycast(previousPosition, direction, distance, layerMask);
        if (hit.collider != null) {
            transform.position = hit.point;
            rb.velocity = Vector2.zero;
            rb.isKinematic = true; 
            FireballHit(hit.collider);
        }

        previousPosition = currentPosition;
    }

    public void FireballHit(Collider2D collision) {
        hasHit = true; 
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null) {

            Vector2 deliveredKnockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            bool gotHit = damageable.Hit(damage, deliveredKnockback);

            Destroy(gameObject, hitDestroyTimer);

        } else {
            Destroy(gameObject, hitDestroyTimer);
        }
    }
}
