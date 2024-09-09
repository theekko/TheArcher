using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Slime : MonoBehaviour {

    [Header("Jump")]
    [SerializeField] private float initialJumpDelayRange = 2f;
    [SerializeField] private float jumpImpulse = 5f;
    [SerializeField] private float jumpTime = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float horizontalSpeed = 5f;

    [Header("Player Detection")]
    [SerializeField] private float playerDetectionDistance = 3f;
    [SerializeField] private float returnTolerance = 0.05f;
    [SerializeField] private float returnSpeed = 2f;
    [SerializeField] private bool _playerDetected = false;
    [SerializeField] private float horizontalMoveTolerance = 0.1f;

    private Rigidbody2D rb;
    private float jumpTimer = 0f;
    private Vector3 startingPosition;
    private float distance;
    private TouchingDirections touchingDirections;
    private Damageable damageable;

    public event EventHandler jumpEvent;
    public event EventHandler fastFallEvent;

    public bool PlayerDetected {
        get { 
            return _playerDetected;
        }
        private set { 
            _playerDetected = value;
        }
    }



    private void Jump() {
        rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        jumpTimer = 0;
        jumpEvent?.Invoke(this, EventArgs.Empty);
    }



    private void Start() {
        startingPosition = transform.position;
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>(); 

        // Use the slime's position and time since level load to generate a unique seed
        float uniqueSeed = transform.position.x + transform.position.y + Time.timeSinceLevelLoad;
        UnityEngine.Random.InitState(uniqueSeed.GetHashCode());

        // Now generate the jump timer based on this unique seed
        jumpTimer = UnityEngine.Random.Range(0, initialJumpDelayRange);
    }



    private void FixedUpdate() {
        //accelerated fall time
        if (rb.velocity.y < 0 && PlayerDetected) {
            fastFallEvent?.Invoke(this, EventArgs.Empty);
            rb.velocity = new Vector2(0, rb.velocity.y * fallMultiplier);
        }
    }

    void Update() {
        if (!damageable.IsAlive) {
            rb.velocity = Vector2.zero;
            return;
        }
        jumpTimer += Time.deltaTime;
        if (jumpTimer >= jumpTime) {
            Jump();
        }
        distance = Vector2.Distance(transform.position, Player.Instance.transform.position);
        Vector2 startingDirection = startingPosition - transform.position;
        Vector2 direction = Player.Instance.transform.position - transform.position;
        
        // Raycast to check if there's ground between Slime and Player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, distance, LayerMask.GetMask(LayerStrings.Ground));

        float horizontalDistanceToPlayer = Mathf.Abs(Player.Instance.transform.position.x - transform.position.x);

        if (distance < playerDetectionDistance && hit.collider == null && !touchingDirections.IsGrounded && rb.velocity.y >= 0) {
            // Only move horizontally towards the player if beyond the tolerance range
            if (horizontalDistanceToPlayer > horizontalMoveTolerance) {
                PlayerDetected = true;
                Vector2 horizontalMovement = new Vector2(direction.x, 0).normalized * horizontalSpeed * Time.deltaTime;
                transform.position = new Vector3(transform.position.x + horizontalMovement.x, transform.position.y, 0);
            }
        } else if ((distance > playerDetectionDistance || hit.collider != null) && !touchingDirections.IsGrounded) {
            PlayerDetected = false;
            if (Mathf.Abs(startingPosition.x - transform.position.x) > returnTolerance) {
                // Move towards the starting position if not within the tolerance range
                float step = returnSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(startingPosition.x, transform.position.y, 0), step);
            }
        }

    }
}
