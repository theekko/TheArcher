using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeArmsAnimator : MonoBehaviour {
    [SerializeField] private Shade shade;
    [SerializeField] private Damageable damageable;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        shade.OnAttackEvent += Shade_OnAttackEvent;
        damageable.damageableHit += Damageable_damageableHit;
    }

    private void Shade_OnAttackEvent(object sender, Shade.OnAttackEventArgs e) {
        animator.SetTrigger(AnimatorStrings.attack);
    }

    private void Damageable_damageableHit(object sender, Damageable.OnHitEventArgs e) {
        animator.SetTrigger(AnimatorStrings.hit);
    }


    private void Update() {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("(Empty)")) {
            spriteRenderer.enabled = false;
        } else {
            spriteRenderer.enabled = true;
        }
        
    }
}
