using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAnimator : MonoBehaviour {
    private Slime slime;
    private Rigidbody2D rb;
    private TouchingDirections touchingDirections;
    private Damageable damageable;

    private Animator animator;
   

    private void Awake() {
        slime = GetComponentInParent<Slime>();
        rb = GetComponentInParent<Rigidbody2D>();
        touchingDirections = GetComponentInParent<TouchingDirections>();
        damageable = GetComponentInParent<Damageable>();
        animator = GetComponent<Animator>();

        slime.jumpEvent += Slime_jumpEvent;
        slime.fastFallEvent += Slime_fastFallEvent;
        damageable.damageableHit += Damageable_damageableHit;
    }

    private void Damageable_damageableHit(object sender, Damageable.OnHitEventArgs e) {
        animator.SetTrigger(AnimatorStrings.hit);
    }

    private void Slime_jumpEvent(object sender, System.EventArgs e) {
        animator.SetTrigger(AnimatorStrings.jump);
    }

    private void Slime_fastFallEvent(object sender, System.EventArgs e) {
        animator.SetTrigger(AnimatorStrings.fastFall);
    }


    private void Update() {
        animator.SetBool(AnimatorStrings.playerDetected, slime.PlayerDetected);
        animator.SetBool(AnimatorStrings.isGrounded, touchingDirections.IsGrounded);
        animator.SetBool(AnimatorStrings.isAlive, damageable.IsAlive);
        animator.SetFloat(AnimatorStrings.yVelocity, rb.velocity.y);
        
    }
}
