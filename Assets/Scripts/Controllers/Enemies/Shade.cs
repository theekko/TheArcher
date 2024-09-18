using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shade : MonoBehaviour {
    [Header("Attack")]
    [SerializeField] private bool _isAttacking = false;

    [Header("Time Between Attacks")]
    [SerializeField] private float attackTime = 3f; 

    [Header("Target Detection")]
    [SerializeField] private float targetDetectionDistance = 3f;
    [SerializeField] private bool _playerDetected = false;

    [Header("Teleportation")]
    [SerializeField] private TeleportThreshold teleportThreshold;
    [SerializeField] private List<Transform> teleportPoints;
    private int nextTeleportPointIndex = 1;


    private Rigidbody2D rb;
    private TouchingDirections touchingDirections;
    private float attackTimer = 0f;
    private float distance;
    private Damageable damageable;
    private Vector2 direction;
    private bool _isfacingRight = false;
    

    public event EventHandler<OnAttackEventArgs> OnAttackEvent;
    public class OnAttackEventArgs : EventArgs {
        public Vector3 direction;
    }

    public bool PlayerDetected {
        get {
            return _playerDetected;
        }
        private set {
            _playerDetected = value;
        }
    }

    public bool IsAttacking {
        get {
            return _isAttacking;
        }
        private set {
            _isAttacking = value;
        }
    }


    public bool IsFacingRight {
        get {
            return _isfacingRight;
        }
        private set {
            _isfacingRight = value;
        }
    }


    private void UpdateAttackDirection() {
        distance = Vector2.Distance(transform.position, Player.Instance.transform.position);
        direction = Player.Instance.transform.position - transform.position;
    }

    private void Attack() {
        OnAttackEvent?.Invoke(this, new OnAttackEventArgs { 
            direction = direction
        });
        attackTimer = 0f;
        IsAttacking = false;
    }

    private void TeleportThreshold_OnThresholdCross(object sender, EventArgs e) {
        if (damageable.IsAlive) {
            transform.position = teleportPoints[nextTeleportPointIndex].position;
            nextTeleportPointIndex++;
            if (nextTeleportPointIndex >= teleportPoints.Count) {
                nextTeleportPointIndex = 0;
            }
        }
    }



    void Update() {
        if (!damageable.IsAlive) {
            rb.velocity = Vector2.zero;
            return;
        }
        UpdateAttackDirection();

        attackTimer += Time.deltaTime;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, LayerMask.GetMask(LayerStrings.Ground));

        if (direction.x > 0) {
            IsFacingRight = true;
        } else if (direction.x < 0) {
            IsFacingRight = false;
        }

        if (distance < targetDetectionDistance && hit.collider == null) {
            PlayerDetected = true;


            if (attackTimer >= attackTime && !IsAttacking) {
                IsAttacking = true;
                Attack();
            }
        } 
    }


    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
        touchingDirections = GetComponent<TouchingDirections>();
        teleportThreshold.OnThresholdCross += TeleportThreshold_OnThresholdCross;
    }


}
