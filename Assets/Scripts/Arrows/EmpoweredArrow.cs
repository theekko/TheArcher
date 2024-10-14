using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmpoweredArrow : MonoBehaviour {
    [SerializeField] private int damage = 3;
    [SerializeField] private Vector2 knockback = Vector2.zero;
    [SerializeField] private float destroyTimer = 2f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float hitDestroyTimer = 0f;

    private Vector3 shootDir;
    private Vector3 previousPosition;
    private Rigidbody2D rb;
    private bool hasHit = false;

    static public event EventHandler objectHitEvent;
    static public event EventHandler groundHitEvent;
    static public event EventHandler empoweredArrowMissEvent;

    public void Setup(Vector3 shootDir) {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(shootDir.x, shootDir.y) * moveSpeed, ForceMode2D.Impulse);

        this.shootDir = shootDir;
        this.shootDir.z = 0;
        previousPosition = transform.position;

        // Calculate the angle in degrees for the rotation
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;

        // Rotate the arrow to face the shoot direction
        transform.eulerAngles = new Vector3(0, 0, angle);
        StartCoroutine(DestroyAfterTime());
    }

    private IEnumerator DestroyAfterTime() {
        // Wait for the destroy timer duration
        yield return new WaitForSeconds(destroyTimer);

        // If the arrow hasn't hit anything, trigger the miss event
        if (!hasHit) {
            empoweredArrowMissEvent?.Invoke(this, EventArgs.Empty);
        }

        // Destroy the arrow
        Destroy(gameObject);
    }

    private void FixedUpdate() {
        if (hasHit) return; // Stop any further updates if arrow has hit something

        // Perform a raycast from the previous position to the current position
        Vector3 currentPosition = transform.position;
        Vector3 direction = currentPosition - previousPosition;
        float distance = direction.magnitude;

        // Create a LayerMask that matches the collision matrix
        int layerMask = LayerMask.GetMask(LayerStrings.Enemies, LayerStrings.EnemySlime, LayerStrings.Ground);

        RaycastHit2D hit = Physics2D.Raycast(previousPosition, direction, distance, layerMask);
        if (hit.collider != null) {
            // Set arrow position to the exact point of impact
            transform.position = hit.point;
            // Stop arrow's movement
            rb.velocity = Vector2.zero;
            rb.isKinematic = true; // Optional: make the Rigidbody kinematic to stop further physics interactions

            // Handle the collision if something was hit
            ArrowHit(hit.collider);
        }

        // Update the previous position for the next frame
        previousPosition = currentPosition;
    }

    public void ArrowHit(Collider2D collision) {
        hasHit = true; // Ensure this only happens once
        Damageable damageable = collision.GetComponent<Damageable>();
        if (collision.gameObject.layer != LayerMask.NameToLayer(LayerStrings.Ground)) {
            objectHitEvent?.Invoke(this, EventArgs.Empty);
        } else {
            groundHitEvent?.Invoke(this, EventArgs.Empty);
        }
        if (damageable != null) {

            Vector2 deliveredKnockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            bool gotHit = damageable.Hit(damage, deliveredKnockback);
            
            Destroy(gameObject, hitDestroyTimer);
            
        } else {
            Destroy(gameObject, hitDestroyTimer);
        }
    }
}
