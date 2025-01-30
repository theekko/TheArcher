using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLooper : MonoBehaviour {

    [SerializeField] private float scrollSpeed = 0.3f; 
    [SerializeField] private float xLimit = -35.6f; 
    [SerializeField] private float resetPositionX = 35.6f; 

    private void Update() {
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        if (transform.position.x <= xLimit) {
            transform.position = new Vector3(resetPositionX, transform.position.y, transform.position.z);
        }
    }
}
