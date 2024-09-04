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
    [SerializeField] private float normalOffset = 0.3f;

    private Vector3 shootDir;
    private Vector3 previousPosition;
    private Rigidbody2D rb;
    private bool hasHit = false;
    private bool hitWall = false;
    private Vector3 collisionNormal3D;

    public float MoveSpeed {
        get {
            return moveSpeed;
        }
    }

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
        int layerMask = LayerMask.GetMask(LayerStrings.Ground);

        // Use CircleCast for better surface detection
        RaycastHit2D hit = Physics2D.CircleCast(previousPosition, arrowCollisionOffset, direction, distance, layerMask);
        if (hit.collider != null) {
             if (hit.collider.gameObject.layer == LayerMask.NameToLayer(LayerStrings.Ground)) {
                HandleGroundCollision(hit, previousPosition);
            }
        }

        // Update the previous position for the next frame
        previousPosition = currentPosition;
    }

    private void HandleGroundCollision(RaycastHit2D hit, Vector3 previousPosition) {
        // Set the hitWall flag to true since this method handles ground collisions
        hitWall = true;

        // Calculate and store the normal to the surface at the point of contact
        Vector2 collisionNormal = hit.normal;

        // Ensure the normal is facing towards the player (opposite to the arrow's direction)
        if (Vector2.Dot(collisionNormal, shootDir) > 0) {
            // If the dot product is positive, the normal is facing the same direction as the arrow's movement
            // Invert the normal to face the opposite direction
            collisionNormal = -collisionNormal;
        }

        // Convert to Vector3 to match types
        collisionNormal3D = new Vector3(collisionNormal.x, collisionNormal.y, 0f);

        // Move the arrow to the point of impact, adjusted slightly along the normal to prevent overlapping
        Vector3 adjustedPosition = (Vector3)hit.point + collisionNormal3D * arrowCollisionOffset;
        transform.position = adjustedPosition;

        // Calculate the angle needed to rotate the arrow to align with the surface normal
        float angle = Mathf.Atan2(collisionNormal.y, collisionNormal.x) * Mathf.Rad2Deg;

        // Rotate the arrow to align with the surface normal
        transform.eulerAngles = new Vector3(0, 0, angle - 90f); // Subtract 90 to make the arrow point into the surface

        // Trigger ground collision handling
        ArrowHit(hit.collider);
    }

    //private void HandleEnemyCollision(RaycastHit2D hit) {
    //    // Ensure hitWall is false when hitting an enemy
    //    hitWall = false;

    //    // Handle the enemy hit
    //    ArrowHit(hit.collider);
    //}

    //private void HandleGroundAfterEnemyHit() {
    //    Vector3 direction = transform.position - previousPosition;
    //    float remainingDistance = direction.magnitude;

    //    // Check for ground collision along the arrow's path after the enemy hit
    //    RaycastHit2D groundHit = Physics2D.CircleCast(transform.position, arrowCollisionOffset, direction, Mathf.Infinity, LayerMask.GetMask(LayerStrings.Ground));

    //    if (groundHit.collider != null) {
    //        // Move the arrow directly to the future ground impact point
    //        transform.position = groundHit.point;

    //        // Call HandleGroundCollision to handle the normal alignment and further processing
    //        HandleGroundCollision(groundHit, previousPosition);
    //    }
    //}


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
        // Find any existing teleport point and destroy it to ensure only one teleport point exists
        TeleportPoint existingTeleportPoint = FindObjectOfType<TeleportPoint>();
        if (existingTeleportPoint != null) {
            Destroy(existingTeleportPoint.gameObject);
        }

        // Default position for the teleport point is at the arrow's current position
        Vector3 teleportPointPosition = transform.position;

        // If the arrow hit a wall, adjust the teleport point's position
        if (hasHit && hitWall) {
            // Move the teleport point position slightly along the normal to avoid spawning inside the wall
            teleportPointPosition += collisionNormal3D * normalOffset;
        }

        // Spawn the teleport point at the adjusted position
        if (TeleportPoint != null) {
            Instantiate(TeleportPoint, teleportPointPosition, Quaternion.identity);
        }

        // Destroy the arrow
        Destroy(gameObject, hitDestroyTimer);
    }
}
