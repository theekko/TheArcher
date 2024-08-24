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
    [SerializeField] private bool _isFiring;
    

    private Vector3 lastDirection;
    private Player player;
    private Rigidbody2D rb;
    private Vector3 direction;

    public event EventHandler<OnFireEventArgs> OnFireEvent;
    public class OnFireEventArgs : EventArgs {
        public Vector3 bowEndpointPosition;
        public Vector3 shootPosition;
    }

    public bool IsFiring {
        get {
            return _isFiring;
        } private set { 
            _isFiring = value;  
        }
    }


    public void OnFire(InputAction.CallbackContext context) {
        if (context.performed) {
            IsFiring = true;

        } else if (context.canceled) {
            IsFiring = false;

            OnFireEvent?.Invoke(this, new OnFireEventArgs {
                bowEndpointPosition = bowEndpointPostion.position,
                shootPosition = direction
            });
        }
    }



    private void Update() {

        // Check if a gamepad is connected and the right stick is being used
        if (Gamepad.current != null && player.RightStickInput.sqrMagnitude > 0f) {
            direction = new Vector3(player.RightStickInput.x, player.RightStickInput.y, 0f);
            lastDirection = direction; // Update the last valid direction
        } else if (Gamepad.current != null && lastDirection != Vector3.zero) {
            direction = lastDirection; // Use the last valid direction
        } else {
            // Fallback to mouse input
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 directiontoMouse = mousePos - transform.position;
            directiontoMouse.z = 0;
            direction = directiontoMouse.normalized;
            lastDirection = direction; // Update the last valid direction
        }


        // Calculate the angle from the player to the input direction for the bow's rotation
        float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Determine if the player is facing right or left
        bool isFacingRight = playerTransform.localScale.x > 0;

        // Apply the adjusted rotation angle to the bow with an offset of -133 degrees
        if (isFacingRight) {
            bow.rotation = Quaternion.Euler(new Vector3(0, 0, rotationAngle - 133f));
        } else {
            bow.rotation = Quaternion.Euler(new Vector3(0, 0, rotationAngle - 40));
        }

        // Calculate the bow's position separately without modifying the angle
        float positionAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bow.position = transform.position + Quaternion.Euler(0, 0, positionAngle) * new Vector3(bowDistance, 0, 0);
    }



    private void Awake() {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
    }

}
