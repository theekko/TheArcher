using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaspAnimator : MonoBehaviour {
    private Wasp wasp;
    private Rigidbody2D rb;
    private TouchingDirections touchingDirections;
    private Damageable damageable;

    private Animator animator;
    private SpriteRenderer spriteRenderer;


    private void Awake() {
        wasp = GetComponentInParent<Wasp>();
        rb = GetComponentInParent<Rigidbody2D>();
        touchingDirections = GetComponentInParent<TouchingDirections>();
        damageable = GetComponentInParent<Damageable>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        damageable.damageableHit += Damageable_damageableHit;
    }

    private void Damageable_damageableHit(object sender, Damageable.OnHitEventArgs e) {
        animator.SetTrigger(AnimatorStrings.hit);
    }



    private void Update() {
        spriteRenderer.flipX = wasp.IsFacingRight;
        animator.SetBool(AnimatorStrings.targetDetected, wasp.TargetDetected);
        animator.SetBool(AnimatorStrings.isAlive, damageable.IsAlive);
        animator.SetBool(AnimatorStrings.isReturningHome, wasp.IsReturningHome);
        animator.SetBool(AnimatorStrings.arrowCaptured, wasp.ArrowCaptured);
    }
}
