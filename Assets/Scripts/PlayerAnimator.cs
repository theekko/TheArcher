using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
    [SerializeField] Player player;
    [SerializeField] TouchingDirections touchingDirections;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Damageable damageable;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
        player.jumpEvent += Player_jumpEvent;
        damageable.damageableHit += Damageable_damageableHit;
    }

    

    private void Damageable_damageableHit(object sender, Damageable.OnHitEventArgs e) {
        animator.SetTrigger(AnimatorStrings.hit);
    }

    private void Player_jumpEvent(object sender, System.EventArgs e) {
        animator.SetTrigger(AnimatorStrings.jump);
    }


    private void Update() {
        animator.SetBool(AnimatorStrings.isGrounded, touchingDirections.IsGrounded);
        animator.SetBool(AnimatorStrings.isRunning, player.isRunning);
        animator.SetBool(AnimatorStrings.isOnWall, touchingDirections.IsOnWall);
        animator.SetBool(AnimatorStrings.isAlive, damageable.IsAlive);
        animator.SetFloat(AnimatorStrings.yVelocity, rb.velocity.y);
    }
}
