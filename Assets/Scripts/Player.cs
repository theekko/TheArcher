using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    [SerializeField] float MoveSpeed = 200f;
    

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate() {
        rb.velocity = new Vector2(moveInput.x * MoveSpeed * Time.deltaTime, rb.velocity.y);
    }
}
