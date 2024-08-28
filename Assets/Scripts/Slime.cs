using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour {

    [SerializeField] private float jumpImpulse = 5f;
    [SerializeField] private float jumpMaxTime = 5f;

    private Rigidbody2D rb;
    private float jumpTimer = 0f;

    private void Jump() {
        rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        jumpTimer = 0;
    }

    private void Start() {
        Jump();
    }
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        jumpTimer += Time.deltaTime;
        if (jumpTimer >= jumpMaxTime) {
            Jump();
        }
    }
}
