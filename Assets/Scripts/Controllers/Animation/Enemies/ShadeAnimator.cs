using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeAnimator : MonoBehaviour {
    private Shade shade;
    private Rigidbody2D rb;
    private TouchingDirections touchingDirections;
    private Damageable damageable;

    private Animator animator;
    private SpriteRenderer spriteRenderer;


    private void Awake() {
        shade = GetComponentInParent<Shade>();
        rb = GetComponentInParent<Rigidbody2D>();
        touchingDirections = GetComponentInParent<TouchingDirections>();
        damageable = GetComponentInParent<Damageable>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        damageable.damageableHit += Damageable_damageableHit;
        shade.OnAttackEvent += Shade_OnAttackEvent;
    }

    private void Shade_OnAttackEvent(object sender, Shade.OnAttackEventArgs e) {
        animator.SetTrigger(AnimatorStrings.attack);
    }

    private void Damageable_damageableHit(object sender, Damageable.OnHitEventArgs e) {
        animator.SetTrigger(AnimatorStrings.hit);
    }



    private void Update() {
        spriteRenderer.flipX = shade.IsFacingRight;
        animator.SetBool(AnimatorStrings.isAlive, damageable.IsAlive);
    }
}
