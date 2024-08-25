using System;
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

    [SerializeField] private bool canDash = true;
    [SerializeField] private bool _isDashing;
    [SerializeField] private float dashingPower = 3f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCooldown = 1f;

    [SerializeField] private float teleportFallReduction;
    [SerializeField] private float maxTeleportFallReduction = 1f;

    private Vector2 _leftStickInput;
    private TouchingDirections touchingDirections;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float jumpAction;
    private float horizontal;
    private Damageable damageable;
    private BowController bow;
    private bool _isKnockedBack = false;
    private bool slowFall = true;

    


    public float CurrentMoveSpeed {
        get {
            if (!touchingDirections.IsOnWallFront || IsWallSliding || IsWallJumping) {
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

    public bool IsDashing {
        get {
            return _isDashing;
        }
        private set {
            _isDashing = value;
        }
    }

    public bool IsKnockedBack {
        get { 
            return _isKnockedBack;
        }
        private set { 
            _isKnockedBack = value;
        }
    }

    public Vector2 LeftStickInput {
        get {
            return _leftStickInput;
        }
        private set {
            _leftStickInput = value;
        }
    }

    public void OnMove(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>();

        //SetFacingDirection(moveInput);
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

        //Wall jump
        if (context.started && wallJumpingCounter > 0f && !touchingDirections.IsGrounded) {

            jumpsRemaining++;
            IsWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    public void OnDash(InputAction.CallbackContext context) {
        if (context.started && canDash && !bow.IsDrawing) {
            StartCoroutine(Dash());
        }
    }

    public void OnTeleport(InputAction.CallbackContext context) {
        TeleportPoint existingTeleportPoint = FindObjectOfType<TeleportPoint>();
        if (existingTeleportPoint != null) {
            transform.position = existingTeleportPoint.transform.position;
            existingTeleportPoint.gameObject.SetActive(false);
            Destroy(existingTeleportPoint.gameObject);
        }
    }

    public void OnHit(object sender, Damageable.OnHitEventArgs e) {
        if (!IsKnockedBack) {
            StartCoroutine(ApplyKnockback(e.knockback));
        }
    }

    // Method to capture right stick input
    public void OnLeftStickMove(InputAction.CallbackContext context) {
        LeftStickInput = context.ReadValue<Vector2>();
    }

    public void Bow_OnFireSuccessEvent(object sender, BowController.OnFireSuccessEventArgs e) {
        slowFall = false;
    }
    private void Bow_OnFireFailEvent(object sender, EventArgs e) {
        slowFall = false;
    }

    private void SetFacingDirection() {
        if (Gamepad.current != null) {
            if (LeftStickInput.x > 0 && !IsFacingRight) {
                // face right
                IsFacingRight = true;
            } else if (LeftStickInput.x < 0 && IsFacingRight) {
                // face left
                IsFacingRight = false;
            }
        } else {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (mousePos.x > transform.position.x && !IsFacingRight) {
                // face right
                IsFacingRight = true;
            } else if (mousePos.x < transform.position.x && IsFacingRight) {
                // face left
                IsFacingRight = false;
            }
        }

    }

    //    private void SetFacingDirection(Vector2 moveInput) {

    //    if (moveInput.x > 0 && !IsFacingRight) {
    //        // face right
    //        IsFacingRight = true;
    //    } else if (moveInput.x < 0 && IsFacingRight) {
    //        // face left
    //        IsFacingRight = false;
    //    }
    //}

    private void WallSlide() {
        if (touchingDirections.IsOnWall && horizontal != 0f && !touchingDirections.IsGrounded) {
            IsWallSliding = true;
            //Clamp is to keep from wall sliding while going up
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        } else {
            IsWallSliding = false;
        }
    }

    private void WallJump() {
        if (IsWallSliding) {
            IsWallJumping = false;
            wallJumpingDirection = -moveInput.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        } else {
            wallJumpingCounter -= Time.deltaTime;
        }

    }

    private void StopWallJumping() { 
        IsWallJumping = false;
    }


    private IEnumerator Dash() {
        canDash = false;
        IsDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        if (moveInput.x == 0f) {
            rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0);
        } else {
            rb.velocity = new Vector2(moveInput.x * dashingPower, 0);
        }
        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originalGravity;
        IsDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private IEnumerator ApplyKnockback(Vector2 knockback) {
        IsKnockedBack = true;
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
        yield return new WaitForSeconds(0.2f); // Adjust the duration as needed
        IsKnockedBack = false;
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
        damageable.damageableHit += OnHit;
        bow = GetComponent<BowController>();
        bow.OnFireSuccessEvent += Bow_OnFireSuccessEvent;
        bow.OnFireFailEvent += Bow_OnFireFailEvent;
    }

    private void FixedUpdate() {
        if (IsDashing || IsKnockedBack) {
            return;
        }

        if (bow.IsDrawing && slowFall) {
            rb.velocity = new Vector2(0, rb.velocity.y / 2);
            return;
        }

        rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed * Time.deltaTime, rb.velocity.y);
    }

    private void Update() {
        SetFacingDirection();
        if (IsDashing) {
            return;
        }

        if (touchingDirections.IsGrounded) {
            slowFall = true;
        }

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
