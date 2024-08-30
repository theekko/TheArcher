using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Slime : MonoBehaviour {

    [SerializeField] private float jumpImpulse = 5f;
    [SerializeField] private float jumpMaxTime = 5f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float approachDistance = 3f;
    [SerializeField] private float returnTolerance = 0.05f;
    [SerializeField] private float returnSpeed = 2f;
    [SerializeField] private float initialJumpDelayRange = 2f;

    private Rigidbody2D rb;
    private float jumpTimer = 0f;
    private Vector3 startingPosition;
    private float distance;
    private TouchingDirections touchingDirections;



    private void Jump() {
        rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        jumpTimer = 0;
    }

    private void Start() {
        startingPosition = transform.position;
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();

        // Use the slime's position and time since level load to generate a unique seed
        float uniqueSeed = transform.position.x + transform.position.y + Time.timeSinceLevelLoad;
        Random.InitState(uniqueSeed.GetHashCode());

        // Now generate the jump timer based on this unique seed
        jumpTimer = Random.Range(0, initialJumpDelayRange);
    }


    void Update()
    {
        jumpTimer += Time.deltaTime;
        if (jumpTimer >= jumpMaxTime) {
            Jump();
        }
        distance = Vector2.Distance(transform.position, Player.Instance.transform.position);
        Vector2 startingDirection = startingPosition - transform.position;
        Vector2 direction = Player.Instance.transform.position - transform.position;
        
        // Raycast to check if there's ground between Slime and Player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, distance, LayerMask.GetMask(LayerStrings.Ground));
        if (distance < approachDistance && hit.collider == null && !touchingDirections.IsGrounded) {
            // Only move horizontally towards the player, not affecting vertical velocity
            Vector2 horizontalMovement = new Vector2(direction.x, 0).normalized * speed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x + horizontalMovement.x, transform.position.y, 0);
        } else if ((distance > approachDistance || hit.collider != null) && !touchingDirections.IsGrounded) {
            if (Mathf.Abs(startingPosition.x - transform.position.x) > returnTolerance) {
                // Move towards the starting position if not within the tolerance range
                float step = returnSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(startingPosition.x, transform.position.y, 0), step);
            }
        }
    }
}
