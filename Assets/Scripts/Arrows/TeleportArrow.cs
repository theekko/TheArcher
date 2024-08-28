using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportArrow : MonoBehaviour {
    [SerializeField] private int damage = 1;
    [SerializeField] private Vector2 knockback = Vector2.zero;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float hitDestroyTimer = 0f;
    [SerializeField] private Transform TeleportPoint;
    [SerializeField] private float arrowCollisionOffset = 0.1f;

    private Vector3 shootDir;
    private Vector3 previousPosition;
    private Rigidbody2D rb;
    private bool hasHit = false;

    public void Setup(Vector3 shootDir, float destroyTimer) {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(shootDir.x, shootDir.y) * moveSpeed, ForceMode2D.Impulse);

        this.shootDir = shootDir;
        this.shootDir.z = 0;
        previousPosition = transform.position;

        // Calculate the angle in degrees for the rotation
        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;

        // Rotate the arrow to face the shoot direction
        transform.eulerAngles = new Vector3(0, 0, angle);

        // Use the passed destroy timer value
        Invoke(nameof(DestroyArrow), destroyTimer);
    }

    private void FixedUpdate() {
        if (hasHit) return; // Stop any further updates if the arrow has hit something

        // Perform a raycast from the previous position to the current position
        Vector3 currentPosition = transform.position;
        Vector3 direction = currentPosition - previousPosition;
        float distance = direction.magnitude;

        // Create a LayerMask that matches the collision matrix
        int layerMask = LayerMask.GetMask(LayerStrings.Enemies, LayerStrings.Ground);

        // Use CircleCast for better surface detection
        RaycastHit2D hit = Physics2D.CircleCast(previousPosition, arrowCollisionOffset, direction, distance, layerMask);

        if (hit.collider != null) {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer(LayerStrings.Enemies)) {
                HandleEnemyCollision(hit);

                // After handling the enemy collision, perform a ground check
                HandleGroundAfterEnemyHit();
            } else if (hit.collider.gameObject.layer == LayerMask.NameToLayer(LayerStrings.Ground)) {
                HandleGroundCollision(hit, previousPosition);
            }
        }

        // Update the previous position for the next frame
        previousPosition = currentPosition;
    }

    private void HandleGroundCollision(RaycastHit2D hit, Vector3 previousPosition) {
        // Adjust the arrow's position to just before the collision point with the ground
        float distanceToImpact = hit.distance - arrowCollisionOffset; // 0.1f offset to avoid placing the point inside the wall
        transform.position = Vector3.MoveTowards(previousPosition, hit.point, distanceToImpact);

        // Trigger ground collision handling
        ArrowHit(hit.collider);
    }

    private void HandleGroundAfterEnemyHit() {
        Vector3 direction = transform.position - previousPosition;
        float remainingDistance = direction.magnitude;

        RaycastHit2D groundHit = Physics2D.CircleCast(transform.position, arrowCollisionOffset, direction, remainingDistance, LayerMask.GetMask(LayerStrings.Ground));
        if (groundHit.collider != null) {
            HandleGroundCollision(groundHit, transform.position);
        }
    }

    private void HandleEnemyCollision(RaycastHit2D hit) {
        // Handle the enemy hit
        ArrowHit(hit.collider);

        // The rest of the ground check logic is moved to HandleGroundAfterEnemyHit
    }

    public void ArrowHit(Collider2D collision) {
        hasHit = true; // Ensure this only happens once
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null) {
            Vector2 deliveredKnockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);
            bool gotHit = damageable.Hit(damage, deliveredKnockback);
        } else {
            DestroyArrow();
        }
    }

    private void DestroyArrow() {
        TeleportPoint existingTeleportPoint = FindObjectOfType<TeleportPoint>();
        if (existingTeleportPoint != null) {
            Destroy(existingTeleportPoint.gameObject);
        }

        // Spawn the teleport point at the arrow's current position
        if (TeleportPoint != null) {
            Instantiate(TeleportPoint, transform.position, Quaternion.identity);
        }

        // Destroy the arrow
        Destroy(gameObject, hitDestroyTimer);
    }
}
