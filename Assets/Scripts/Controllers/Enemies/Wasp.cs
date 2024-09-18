using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wasp : MonoBehaviour {
    [Header("Attack")]
    [SerializeField] private float attackPower = 5f;
    [SerializeField] private float chargeTime = 0.5f;
    [SerializeField] private bool _isAttacking = false;

    [Header("Time Between Attacks")]
    [SerializeField] private float attackTime = 3f;  // Time to wait between dashes

    [Header("Target Detection")]
    [SerializeField] private float targetDetectionDistance = 3f;
    [SerializeField] private float returnTolerance = 0.05f;
    [SerializeField] private float returnSpeed = 2f;
    [SerializeField] private bool _targetDetected = false;
    [SerializeField] private bool _isReturningHome = false;

    [Header("Capture TP Arrow")]
    [SerializeField] private TeleportPoint teleportPoint;
    [SerializeField] private bool _arrowCaptuerd = false;
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float waypointReachedDistance = 0.1f;


    private Rigidbody2D rb;
    private TouchingDirections touchingDirections;
    private Collider2D waspCollider;
    private float attackTimer = 0f;
    private Vector3 startingPosition;
    private float distance;
    private Damageable damageable;
    private TeleportPoint existingTeleportPoint;
    private Vector2 direction;
    private Vector2 finalAttackDirection;
    private Transform nextWaypoint;
    private int waypointNum = 0;
    private bool _isfacingRight = false;
    private bool _isEnteringEscape = false;
    private bool _isEscaping = false;
    private Coroutine navigateAwayCoroutine;

    public static event EventHandler CapturedArrowEvent;
    public static event EventHandler ArrowReleaseEvent;
    public event EventHandler ArmoredCapturedArrowEvent;


    public bool TargetDetected {
        get { 
            return _targetDetected; 
        }
        private set { 
            _targetDetected = value;
        }
    }

    public bool IsAttacking {
        get { 
            return _isAttacking; 
        }
        private set { 
            _isAttacking = value; 
        }
    }

    public bool IsReturningHome {
        get { 
            return _isReturningHome;
        }
        private set { 
            _isReturningHome = value;
        }
    }

    public bool ArrowCaptured {
        get {
            return _arrowCaptuerd;
        }
        private set { 
            _arrowCaptuerd = value;
        }
    }

    public bool IsFacingRight {
        get {
            return _isfacingRight;
        }
        private set {
            _isfacingRight = value;
        }
    }

    public bool IsEnteringEscape {
        get {
            return _isEnteringEscape;
        }
        private set { 
            _isEnteringEscape = value;
        }
    }

    public bool IsEscaping {
        get {
            return _isEscaping;
        }
        private set {
            _isEscaping = value;
        }
    }







    private IEnumerator DashAttack(float dashDistance) {
        IsAttacking = true;
        dashDistance = Mathf.Min(dashDistance, targetDetectionDistance);
        float chargeTimer = 0f;
        while (chargeTimer < chargeTime) {
            UpdateAttackDirection(); 
            chargeTimer += Time.deltaTime;
            yield return null;
        }

        // Final direction lock before dash
        finalAttackDirection = direction.normalized;

        rb.velocity = finalAttackDirection * attackPower;  


        float distanceTraveled = 0f;
        Vector2 startPosition = transform.position;

 
        while (distanceTraveled < dashDistance) {
            distanceTraveled = Vector2.Distance(startPosition, transform.position);
            yield return null;  
        }

        rb.velocity = Vector2.zero; 

        IsAttacking = false;

        attackTimer = 0f;  
    }

    private void UpdateAttackDirection() {
        existingTeleportPoint = FindObjectOfType<TeleportPoint>();

        if (existingTeleportPoint != null) {
            distance = Vector2.Distance(transform.position, existingTeleportPoint.transform.position);
            direction = existingTeleportPoint.transform.position - transform.position;
        } else {
            distance = Vector2.Distance(transform.position, Player.Instance.transform.position);
            direction = Player.Instance.transform.position - transform.position;
        }
    }

    private void NavigateBackToStartingPosition() {
        if (TargetDetected || IsAttacking) {
            IsReturningHome = false;
            return;
        }

        IsReturningHome = true;

        Vector2 toStartingPosition = startingPosition - transform.position;
        float distanceToStartingPosition = toStartingPosition.magnitude;
        Vector2 directionToStartingPosition = toStartingPosition.normalized;

        // Perform raycast from current position to starting position
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToStartingPosition, distanceToStartingPosition, LayerMask.GetMask(LayerStrings.Ground));

        Vector2 targetVelocity;

        if (hit.collider != null) {
            // Obstacle detected, choose an alternative path
            Vector2 adjustedDirection = GetAdjustedDirection(directionToStartingPosition);

            // Move using the Rigidbody2D
            targetVelocity = adjustedDirection * returnSpeed;
        } else {
            // No obstacle detected, move directly to the starting position
            targetVelocity = directionToStartingPosition * returnSpeed;

            // If the Wasp reaches the starting position, stop returning home
            if (Vector2.Distance(transform.position, startingPosition) < returnTolerance) {
                IsReturningHome = false;
                targetVelocity = Vector2.zero;
            }
        }

        // Set the Rigidbody velocity to move the Wasp
        rb.velocity = targetVelocity;


    }


    // Function to adjust the movement direction based on obstacles
    private Vector2 GetAdjustedDirection(Vector2 originalDirection) {
        // Try moving up or down when there's an obstacle in the original direction
        Vector2 adjustedDirectionUp = originalDirection + Vector2.up;   // Try moving up
        Vector2 adjustedDirectionDown = originalDirection + Vector2.down; // Try moving down

        // Cast a short ray to see if moving up or down is possible
        bool canMoveUp = !Physics2D.Raycast(transform.position, adjustedDirectionUp, 1f, LayerMask.GetMask(LayerStrings.Ground));
        bool canMoveDown = !Physics2D.Raycast(transform.position, adjustedDirectionDown, 1f, LayerMask.GetMask(LayerStrings.Ground));

        // Choose the best alternative
        if (canMoveUp) {
            return adjustedDirectionUp.normalized;
        } else if (canMoveDown) {
            return adjustedDirectionDown.normalized;
        }

        // If neither up nor down works, continue in the original direction (should try more complex logic)
        return originalDirection;
    }

    public void Escape() {

        if (IsEnteringEscape) {
            IsEnteringEscape = false;
            IsEscaping = true;

            // Find the closest waypoint
            float closestDistance = float.MaxValue;

            for (int i = 0; i < waypoints.Count; i++) {
                float distanceToWaypoint = Vector2.Distance(transform.position, waypoints[i].position);
                if (distanceToWaypoint < closestDistance) {
                    closestDistance = distanceToWaypoint;
                    waypointNum = i;  // Set the closest waypoint as the current target
                }
            }

            nextWaypoint = waypoints[waypointNum];  // Set the next waypoint to the closest one
        }
        // Fly to next waypoint
        Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

        // Check if we have reached the waypoint already
        float distance = Vector2.Distance(nextWaypoint.position, transform.position);

        // do not need to multiply by Time.deltaTime because rb already take this into account
        rb.velocity = directionToWaypoint * returnSpeed;

        if (distance <= waypointReachedDistance) {
            // Switch to next waypoint
            waypointNum++;
            if (waypointNum >= waypoints.Count) {
                waypointNum = 0;
            }

            nextWaypoint = waypoints[waypointNum];
        }
    }

    public void UpdateFacingDirection() {
        if (rb.velocity.x > 0) {
            IsFacingRight = true;

        } else if (rb.velocity.x < 0) {
            IsFacingRight = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        TeleportPoint teleportPoint = collision.gameObject.GetComponent<TeleportPoint>();
        if (teleportPoint != null) {
            // Invoke the event if the TeleportPoint trigger is detected
            CapturedArrowEvent?.Invoke(this, EventArgs.Empty);
            ArmoredCapturedArrowEvent?.Invoke(this, EventArgs.Empty);
            TargetDetected = false;
            ArrowCaptured = true;
        }
    }







    void Update() {
        if (!damageable.IsAlive) {
            rb.velocity = Vector2.zero;
            existingTeleportPoint = FindObjectOfType<TeleportPoint>();

            if (existingTeleportPoint == null && ArrowCaptured) {
                ArrowReleaseEvent?.Invoke(this, EventArgs.Empty);
                Instantiate(teleportPoint, transform.position, Quaternion.identity);
            }
            waspCollider.enabled = false;
            return;
        }
        UpdateAttackDirection();
        if (rb.velocity.x != 0) {
            UpdateFacingDirection();
        }

        attackTimer += Time.deltaTime;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, LayerMask.GetMask(LayerStrings.Ground));

        if (distance < targetDetectionDistance && hit.collider == null && !ArrowCaptured) {
            TargetDetected = true;
            IsReturningHome = false;

            if (direction.x > 0) {
                IsFacingRight = true;
            } else if (direction.x < 0) {
                IsFacingRight = false;
            }

            if (attackTimer >= attackTime && !IsAttacking) {
                StartCoroutine(DashAttack(distance));
            }
        } else if ((distance > targetDetectionDistance || hit.collider != null) && !ArrowCaptured) {
            IsAttacking = false;
            TargetDetected = false;
            NavigateBackToStartingPosition();
        } else {
            if (!IsEscaping) { 
                IsEnteringEscape = true;
            }
            Escape();
        }


    }

    private void Start() {
        startingPosition = transform.position;
        nextWaypoint = waypoints[waypointNum];
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
        touchingDirections = GetComponent<TouchingDirections>();
        waspCollider = GetComponent<Collider2D>();
    }
}