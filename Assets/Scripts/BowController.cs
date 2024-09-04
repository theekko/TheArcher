using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class BowController : MonoBehaviour {
    [SerializeField] private Transform bow;
    [SerializeField] private float bowDistance = 1.5f;
    [SerializeField] private Transform bowEndpointPostion;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private bool _isDrawing;
    [SerializeField] private float minDrawTime = 0.5f;
    [SerializeField] private float maxDrawTime = 3f;
    [SerializeField] private float minDestroyTimer = 0.1f; 
    [SerializeField] private float maxDestroyTimer = 0.3f; 
    [SerializeField] private float raycastDistance = 0.05f;
    [SerializeField] private float checkRadius = 0.05f;
    [SerializeField] private float gamepadConeAngle = 30f;
    [SerializeField] private float mouseConeAngle = 15f; 
    [SerializeField] private float maxDistance = 10f;

    [SerializeField] private bool _isEmpoweredShot = false;

    private Vector3 lastDirection;
    private Player player;
    private Rigidbody2D rb;
    private Vector3 direction;
    private float drawTime = 0f;
    private float currentConeAngle; // Variable to store the current cone angle
    private bool _isDrawingArrow;
    private bool _isDrawingTeleportArrow;
    private bool _drawSucceedArrow;
    private bool _drawSucceedTeleportArrow;
    private float linearTime;
    private float destroyTimer;
    private Collider2D closestEnemy;



    public event EventHandler<OnFireSuccessEventArgs> OnFireSuccessEvent;
    public event EventHandler<OnFireSuccessEventArgs> OnFireEmpoweredSuccessEvent;
    public class OnFireSuccessEventArgs : EventArgs {
        public Vector3 bowEndpointPosition;
        public Vector3 shootPosition;
    }


    public event EventHandler<OnFireTeleportSuccessEventArgs> OnFireTeleportSuccessEvent;
    public class OnFireTeleportSuccessEventArgs : EventArgs {
        public Vector3 bowEndpointPosition;
        public Vector3 shootPosition;
        public float destroyTimer; // Pass the destroy timer
    }

    public event EventHandler OnFireFailEvent;
    
    public bool IsDrawing {
        get {
            return _isDrawing;
        }
        private set {
            _isDrawing = value;
        }
    }
    public bool IsDrawingTeleportArrow {
        get {
            return _isDrawingTeleportArrow;
        }
        private set {
            _isDrawingTeleportArrow = value;
        }
    }
    public bool IsDrawingArrow {
        get {
            return _isDrawingArrow;
        }
        private set {
            _isDrawingArrow = value;
        }
    }

    public bool IsEmpoweredShot {
        get {
            return _isEmpoweredShot;
        }
        private set { 
            _isEmpoweredShot = value;
        }
    }

    public bool DrawSucceedArrow {
        get {
            return _drawSucceedArrow;
        }
        private set {
            _drawSucceedArrow = value;
        }
    }

    public bool DrawSucceedTeleportArrow {
        get {
            return _drawSucceedTeleportArrow;
        }
        private set { 
            _drawSucceedTeleportArrow = value;
        }
    }

    public float MinDrawTime {
        get {
            return minDrawTime;
        }
    }

    public float MaxDrawTime {
        get {
            return maxDrawTime;
        }
    }

    public float MinDestroyTimer {
        get {
            return minDestroyTimer;
        }
    }

    public float MaxDestroyTimer {
        get {
            return maxDestroyTimer;
        }
    }

    public float DestroyTimer {
        get { 
            return destroyTimer;
        }
    }

    public Vector3 Direction {
        get {
            return direction;
        }
    }

    public Collider2D ClosestEnemy {
        get {
            return closestEnemy;
        }
    }

    public void OnFireArrow(InputAction.CallbackContext context) {
        if (context.performed) {
            IsDrawing = true;
            IsDrawingArrow = true;
        } else if (context.canceled) {
            IsDrawing = false;
            IsDrawingArrow = false;
            DrawSucceedArrow = false;
            OnFireFailEvent?.Invoke(this, EventArgs.Empty);

            // Perform raycast and overlap checks before triggering the event
            if (drawTime >= minDrawTime && !IsObstacleInTheWay() && !IsBowInsideObstacle()) {
                if (IsEmpoweredShot) {
                    OnFireEmpoweredSuccessEvent?.Invoke(this, new OnFireSuccessEventArgs {
                        bowEndpointPosition = bowEndpointPostion.position,
                        shootPosition = direction
                    });
                    IsEmpoweredShot = false;
                } else {
                    OnFireSuccessEvent?.Invoke(this, new OnFireSuccessEventArgs {
                        bowEndpointPosition = bowEndpointPostion.position,
                        shootPosition = direction
                    });
                }

            }
            drawTime = 0f;
        }
    }

    public void OnFireTeleportArrow(InputAction.CallbackContext context) {
        if (context.performed) {
            IsDrawing = true;
            IsDrawingTeleportArrow = true;
            drawTime = 0f; // Reset draw time when starting to draw

        } else if (context.canceled) {
            IsDrawing = false;
            IsDrawingTeleportArrow = false;
            DrawSucceedTeleportArrow = false;
            OnFireFailEvent?.Invoke(this, EventArgs.Empty);

            // Perform raycast and overlap checks before triggering the event
            if ( !IsObstacleInTheWay() && !IsBowInsideObstacle()) {

                OnFireTeleportSuccessEvent?.Invoke(this, new OnFireTeleportSuccessEventArgs {
                    bowEndpointPosition = bowEndpointPostion.position,
                    shootPosition = direction,
                    destroyTimer = destroyTimer
                });
            }
            drawTime = 0f;
        }
    }

    private bool IsObstacleInTheWay() {
        // Cast from the player's position or slightly in front of it
        Vector3 startPoint = playerTransform.position;
        RaycastHit2D hit = Physics2D.Raycast(startPoint, direction, raycastDistance, LayerMask.GetMask(LayerStrings.Ground));
        return hit.collider != null;
    }

    private bool IsBowInsideObstacle() {
        // Check if the bow is inside any collider using an overlap check
        Collider2D overlapHit = Physics2D.OverlapCircle(bow.position, checkRadius, LayerMask.GetMask(LayerStrings.Ground));
        return overlapHit != null;
    }

    private Collider2D FindClosestEnemyInCone(Vector3 aimDirection, float coneAngle) {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, maxDistance, LayerMask.GetMask(LayerStrings.Enemies));

        Collider2D closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D enemy in enemies) {
            Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
            float angleToEnemy = Vector3.Angle(aimDirection, directionToEnemy);

            // Check if the enemy is within the cone and line of sight
            if (angleToEnemy < coneAngle / 2f && !IsObstacleBetweenPlayerAndEnemy(enemy)) {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < closestDistance) {
                    closestDistance = distanceToEnemy;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }

    private void Player_teleportEvent(object sender, EventArgs e) {
        IsEmpoweredShot = true;
    }


    private void Update() {
        if (IsDrawing) {
            drawTime += Time.deltaTime;
            if (drawTime >= minDrawTime && IsDrawingTeleportArrow) {
                DrawSucceedTeleportArrow = true;
            } else if (drawTime >= minDrawTime && IsDrawingArrow) {
                DrawSucceedArrow = true;
            }
        }

        if (IsDrawingTeleportArrow) {
            // Calculate linear time for scaling destroyTimer
            linearTime = Mathf.Clamp01((drawTime - minDrawTime) / (maxDrawTime - minDrawTime));
            destroyTimer = Mathf.Lerp(minDestroyTimer, maxDestroyTimer, linearTime);
        }

        // Check if a gamepad is connected and the right stick is being used
        if (Gamepad.current != null && player.LeftStickInput.sqrMagnitude > 0f) {
            direction = new Vector3(player.LeftStickInput.x, player.LeftStickInput.y, 0f).normalized;
            lastDirection = direction; // Update the last valid direction
            currentConeAngle = gamepadConeAngle; // Use the gamepad cone angle
        } else if (Gamepad.current != null && lastDirection != Vector3.zero) {
            direction = lastDirection; // Use the last valid direction
            currentConeAngle = gamepadConeAngle; // Use the gamepad cone angle
        } else {
            // Fallback to mouse input
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 directiontoMouse = mousePos - transform.position;
            directiontoMouse.z = 0;
            direction = directiontoMouse.normalized;
            lastDirection = direction; // Update the last valid direction
            currentConeAngle = mouseConeAngle; // Use the mouse cone angle
        }

        // Check for aim assist
        if (IsDrawingArrow) {
            closestEnemy = FindClosestEnemyInCone(direction, currentConeAngle);
            if (closestEnemy != null) {
                // Adjust the direction to point at the closest enemy
                direction = (closestEnemy.transform.position - transform.position).normalized;
            }
        }


        // Calculate the angle from the player to the input direction for the bow's rotation
        float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

       

        // Apply the adjusted rotation angle to the bow with an offset of -133 degrees
        
        bow.rotation = Quaternion.Euler(new Vector3(0, 0, rotationAngle - 133f));
        

        // Calculate the bow's position separately without modifying the angle
        float positionAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bow.position = transform.position + Quaternion.Euler(0, 0, positionAngle) * new Vector3(bowDistance, 0, 0);
    }


    private bool IsObstacleBetweenPlayerAndEnemy(Collider2D enemy) {
        Vector3 directionToEnemy = enemy.transform.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToEnemy, directionToEnemy.magnitude, LayerMask.GetMask(LayerStrings.Ground));
        return hit.collider != null; // Returns true if there's an obstacle in the way
    }

    private void Awake() {
        player = GetComponent<Player>();
        player.teleportEvent += Player_teleportEvent;
        rb = GetComponent<Rigidbody2D>();
    }


}
