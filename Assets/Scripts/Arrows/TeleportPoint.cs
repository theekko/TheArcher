using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour {
    [SerializeField] private float teleportOverlapOffset = 0.3f;
    private BoxCollider2D teleportPoint;

    private void Awake() {
        teleportPoint = GetComponent<BoxCollider2D>();
        CheckInitialOverlap();
    }

    private void CheckInitialOverlap() {
        if (teleportPoint != null) {
            // Using the collider's size and rotation for the overlap check
            Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, teleportPoint.size, transform.eulerAngles.z, LayerMask.GetMask(LayerStrings.Ground));

            if (hits.Length > 0) {
                HandleOverlaps(hits);
            }
        }
    }

    private void HandleOverlaps(Collider2D[] hits) {
        Collider2D closestHit = null;
        float closestDistance = float.MaxValue;
        Vector2 closestNormal = Vector2.up;

        foreach (var hit in hits) {
            if (hit != teleportPoint) {  // Make sure not to handle the teleport point's own collider
                Vector2 directionToPlayer = (Player.Instance.transform.position - transform.position).normalized;
                Vector2 hitNormal = GetHitNormal(hit);

                if (Vector2.Dot(hitNormal, directionToPlayer) < 0) {
                    hitNormal = -hitNormal;  // Ensure normal faces towards the player
                }

                float distanceToPlayer = Vector2.Distance(hit.transform.position, Player.Instance.transform.position);
                if (distanceToPlayer < closestDistance) {
                    closestDistance = distanceToPlayer;
                    closestNormal = hitNormal;
                    closestHit = hit;
                }
            }
        }

        if (closestHit != null) {
            // Move the teleport point in the direction of the closest normal
            transform.position += (Vector3)closestNormal * teleportOverlapOffset;
        }
    }

    private Vector2 GetHitNormal(Collider2D hit) {
        // This assumes that the normal can be directly derived from the hit. Adjust as necessary.
        // For a more accurate normal, you might need additional logic or collision detection details.
        if (hit is BoxCollider2D) {
            // Placeholder logic for BoxCollider2D
            return -Vector2.up;  // Default normal if you cannot calculate it dynamically
        }

        return Vector2.up;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer(LayerStrings.Ground)) {
            HandleOverlap(collision.collider);
        }
    }

    private void HandleOverlap(Collider2D hit) {
        Vector2 moveDirection = CalculateAdjustmentDirection(hit);
        transform.position += (Vector3)moveDirection * teleportOverlapOffset;
    }

    private Vector2 CalculateAdjustmentDirection(Collider2D hit) {
        return Vector2.up;  // Adjusts upwards by default
    }
}