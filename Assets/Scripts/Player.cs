using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    [SerializeField] private float moveSpeed = 200f;

    [SerializeField] private bool _isFacingRight = true;

    [SerializeField] private bool _isWallSliding;
    [SerializeField] private float wallSlideSpeed = 2f;

    [SerializeField] private bool _isWallJumping;
    [SerializeField] private float wallJumpingDirection;
    [SerializeField] private float wallJumpingTime = 0.2f;
    [SerializeField] private float wallJumpingCounter;
    [SerializeField] private float wallJumpingDuration = 0.4f;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(1f, 3f);


    [SerializeField] private int maxJumps = 2;
    [SerializeField] private int jumpsRemaining = 2;
    [SerializeField] private float jumpImpulse = 5f;
    [SerializeField] private float doubleJumpeImpulse = 2f;


    private TouchingDirections touchingDirections;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float jumpAction;
    private float horizontal;
    
    


    public float CurrentMoveSpeed {
        get {
            if (!touchingDirections.IsOnWall) {
                return moveSpeed;
            } else {
                return 0;
            }
        }
    }

    public bool IsFacingRight {
        get {
            return _isFacingRight;
        }
        private set {
            if (_isFacingRight != value) {
                // flip the local scale to make the player face the opposite direction
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }

    public bool IsWallSliding {
        get {
            return _isWallSliding;
        }
        private set {
            _isWallSliding = value;
        }
    }

    public bool IsWallJumping {
        get {
            return _isWallJumping;
        }
        private set { 
            _isWallJumping = value;
        }
    }

    public void OnMove(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>();

        SetFacingDirection(moveInput);
    }

    public void OnJump(InputAction.CallbackContext context) {
        jumpAction = context.ReadValue<float>();
        if (context.started && (touchingDirections.IsGrounded || jumpsRemaining != 0)) {
            rb.velocity = new Vector2(rb.velocity.x, jumpsRemaining != maxJumps ? doubleJumpeImpulse : jumpImpulse);

            jumpsRemaining--;
        }
        if (context.canceled && rb.velocity.y > 0 ) {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if (context.started && wallJumpingCounter > 0f && !touchingDirections.IsGrounded) {
            Debug.Log("wall jump");
            jumpsRemaining++;
            IsWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void SetFacingDirection(Vector2 moveInput) {
        if (moveInput.x > 0 && !IsFacingRight) {
            // face right
            IsFacingRight = true;
        } else if (moveInput.x < 0 && IsFacingRight) {
            // face left
            IsFacingRight = false;
        }
    }

    private void WallSlide() {
        if (touchingDirections.IsOnWall && horizontal != 0f && !touchingDirections.IsGrounded) {
            IsWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        } else {
            IsWallSliding = false;
        }
    }

    private void WallJump() {
        if (IsWallSliding) {
            IsWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        } else {
            wallJumpingCounter -= Time.deltaTime;
        }

    }

    private void StopWallJumping() { 
        IsWallJumping = false;
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    private void FixedUpdate() {
        rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed * Time.deltaTime, rb.velocity.y);
    }

    private void Update() {
        horizontal = moveInput.x;

        WallSlide();
        WallJump();

        if (touchingDirections.IsGrounded && !(jumpAction > 0)) {
            jumpsRemaining = maxJumps;
        }
        if (!touchingDirections.IsGrounded && jumpsRemaining > (maxJumps - 2)) {
            jumpsRemaining = maxJumps - 1;
        }
    }

}
