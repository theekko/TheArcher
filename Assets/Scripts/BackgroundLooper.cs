using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLooper : MonoBehaviour {

    [SerializeField] private float scrollSpeed = 0.3f; // Speed of scrolling
    [SerializeField] private float xLimit = -35.6f; // Hardcoded x position at which the sprite resets
    [SerializeField] private float resetPositionX = 35.6f; // Hardcoded position to move the sprite to

    private void Update() {
        // Move the sprite to the left
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        // Check if the sprite has reached the xLimit
        if (transform.position.x <= xLimit) {
            // Reposition the sprite to the new hardcoded x position (resetPositionX)
            transform.position = new Vector3(resetPositionX, transform.position.y, transform.position.z);
        }
    }
}
