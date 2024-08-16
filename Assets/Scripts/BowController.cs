using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BowController : MonoBehaviour {
    [SerializeField] private Transform bow;
    [SerializeField] private float bowDistance = 1.5f;
    [SerializeField] private Transform playerTransform;
    private void Update() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - transform.position;

        // Calculate the angle from the player to the mouse for the bow's rotation
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

    public void OnFire(InputAction.CallbackContext context) {

    }

    public void Fire() { 
    
    }




}
