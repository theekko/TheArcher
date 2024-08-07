using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    [SerializeField] float moveSpeed = 200f;
    [SerializeField] float jumpImpulse = 5f;
    [SerializeField] float doubleJumpeImpulse = 2f;
    [SerializeField] private bool _isFacingRight = true;

    private TouchingDirections touchingDirections;
    private InputAction jumpAction;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    [SerializeField] private bool doubleJump;




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


    public void OnJump(InputAction.CallbackContext context) {
        if (context.started && (touchingDirections.IsGrounded || doubleJump)) {
            rb.velocity = new Vector2(rb.velocity.x, doubleJump ? doubleJumpeImpulse : jumpImpulse);

            doubleJump = !doubleJump;
        } 
        if(context.canceled && rb.velocity.y > 0) { 
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    } 

    public void OnMove(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>();

        SetFacingDirection(moveInput);
    }

    public void SetFacingDirection(Vector2 moveInput) {
        if (moveInput.x > 0 && !IsFacingRight) {
            // face right
            IsFacingRight = true;
        } else if (moveInput.x < 0 && IsFacingRight) {
            // face left
            IsFacingRight = false;
        }
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();

        var inputActionAsset = GetComponent<PlayerInput>().actions;
        jumpAction = inputActionAsset["Jump"];
    }

    private void FixedUpdate() {
        rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed * Time.deltaTime, rb.velocity.y);
    }

    private void Update() {
        // Check if the jump button is being pressed
        if (touchingDirections.IsGrounded && !(jumpAction.ReadValue<float>() > 0)) {
            doubleJump = false;
        }
    }

}
