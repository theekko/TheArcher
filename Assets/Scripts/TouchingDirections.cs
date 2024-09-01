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
    [SerializeField] private bool _isOnWallBack;
    [SerializeField] private bool _isOnWallFront;
    [SerializeField] private bool _isOnCeiling;
    [SerializeField] SpriteRenderer objectSprite;

    private Collider2D touchingCol;

    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    RaycastHit2D[] ceilingHits = new RaycastHit2D[5];



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

    public bool IsOnWallFront {
        get {
            return _isOnWallFront;
        }
        private set {
            _isOnWallFront = value;
        }
    }

    public bool IsOnWallBack {
        get {
            return _isOnWallBack;
        }
        private set {
            _isOnWallBack = value;
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
        touchingCol = GetComponent<Collider2D>();
    }

    // Because this is a phyics function you want to use FixedUpdate instead of Update
    private void FixedUpdate() {
        IsGrounded = touchingCol.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
        if (!objectSprite.flipX) {
            IsOnWallBack = touchingCol.Cast(Vector2.left, castFilter, wallHits, wallDistance) > 0;
            IsOnWallFront = touchingCol.Cast(Vector2.right, castFilter, wallHits, wallDistance) > 0;
        } else {
            IsOnWallBack = touchingCol.Cast(Vector2.right, castFilter, wallHits, wallDistance) > 0;
            IsOnWallFront = touchingCol.Cast(Vector2.left, castFilter, wallHits, wallDistance) > 0;
        }
        bool wallHitLeft = touchingCol.Cast(Vector2.left, castFilter, wallHits, wallDistance) > 0;
        bool wallHitRight = touchingCol.Cast(Vector2.right, castFilter, wallHits, wallDistance) > 0;
        IsOnWall = wallHitLeft || wallHitRight;
        IsOnCeiling = touchingCol.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;
    }
}
