using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BowController : MonoBehaviour {
    [SerializeField] private Transform bow;
    [SerializeField] private float bowDistance = 1.5f;

    private Vector3 mousePos;

    private void Update() {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - transform.position;

        bow.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
        Debug.Log("mousePos: " + mousePos + " direction " + direction + " bow.rotation " + bow.rotation);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bow.position = transform.position + Quaternion.Euler(0, 0, angle) * new Vector3(bowDistance, 0, 0);

    }

    public void OnFire(InputAction.CallbackContext context) {

    }

    public void Fire() { 
    
    }




}
