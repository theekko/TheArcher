using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    [SerializeField] private ContactFilter2D castFilter;
    [SerializeField] private float groundDistance = 0.05f;
    [SerializeField] private float wallDistance = 0.05f;
    [SerializeField] private float ceilingDistance = 0.05f;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isOnWall;
    [SerializeField] private bool _isOnCeiling;

    private CapsuleCollider2D touchingCol;

    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    RaycastHit2D[] ceilingHits = new RaycastHit2D[5];

    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;


    public bool IsGrounded {
        get {
            return _isGrounded;
        }
        private set {
            _isGrounded = value;
        }
    }

    public bool IsOnWall {
        get {
            return _isOnWall;
        }
        private set {
            _isOnWall = value;
        }
    }

    public bool IsOnCeiling {
        get {
            return _isOnCeiling;
        }
        private set {
            _isOnCeiling = value;
        }
    }


    private void Awake() {
        touchingCol = GetComponent<CapsuleCollider2D>();
    }

    // Because this is a phyics function you want to use FixedUpdate instead of Update
    private void FixedUpdate() {
        IsGrounded = touchingCol.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
        IsOnWall = touchingCol.Cast(wallCheckDirection, castFilter, wallHits, wallDistance) > 0;
        IsOnCeiling = touchingCol.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;
    }
}
