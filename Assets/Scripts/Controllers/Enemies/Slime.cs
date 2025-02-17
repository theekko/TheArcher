using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
    [SerializeField] private float platformDetectionRange = 0.05f; // Adjust this value as needed

    private Rigidbody2D rb;
    private float jumpTimer = 0f;
    private Vector3 startingPosition;
    private float distance;
    private TouchingDirections touchingDirections;
    private Damageable damageable;
    private bool isNearWall = false; // Track if the slime is near a wall
    private Vector2 cachedHorizontalMovement; // Cache the movement direction

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
        cachedHorizontalMovement = Vector2.zero; // Reset cached movement after a jump
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
        // Accelerated fall time
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
        Vector2 direction = Player.Instance.transform.position - transform.position;

        float horizontalDistanceToPlayer = Mathf.Abs(Player.Instance.transform.position.x - transform.position.x);

        // Raycast in the direction of movement to detect platform limits, limiting the range
        RaycastHit2D platformLimitHit = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(direction.x), platformDetectionRange, LayerMask.GetMask(LayerStrings.PlatformLimits));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, distance, LayerMask.GetMask(LayerStrings.Ground));

        // Check if near a platform limit
        if (platformLimitHit.collider != null) {
            isNearWall = true; 
        } else {
            isNearWall = false;
        }

        // If near a platform limit, only allow movement away from the wall
        if (isNearWall) {
            if (Mathf.Sign(direction.x) != Mathf.Sign(platformLimitHit.point.x - transform.position.x)) {
                Vector2 horizontalMovement = new Vector2(direction.x, 0).normalized * horizontalSpeed * Time.deltaTime;
                cachedHorizontalMovement = horizontalMovement; // Cache the movement before jumping
                transform.position = new Vector3(transform.position.x + horizontalMovement.x, transform.position.y, 0);
            }
        } else if (distance < playerDetectionDistance && !touchingDirections.IsGrounded && rb.velocity.y >= 0 && hit.collider == null) {
            if (horizontalDistanceToPlayer > horizontalMoveTolerance) {
                PlayerDetected = true;

                if (cachedHorizontalMovement == Vector2.zero) {
                    // Cache the movement direction the first time the slime jumps towards the player
                    cachedHorizontalMovement = new Vector2(direction.x, 0).normalized * horizontalSpeed * Time.deltaTime;
                }

                // Apply the cached movement direction mid-air, so it doesn't change
                transform.position = new Vector3(transform.position.x + cachedHorizontalMovement.x, transform.position.y, 0);
            }
        }

        if ((distance > playerDetectionDistance || platformLimitHit.collider != null || hit.collider != null) && !touchingDirections.IsGrounded) {
            PlayerDetected = false;
            if (Mathf.Abs(startingPosition.x - transform.position.x) > returnTolerance) {
                float step = returnSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(startingPosition.x, transform.position.y, 0), step);
            }
        }
    }
}


