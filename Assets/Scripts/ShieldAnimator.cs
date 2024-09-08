using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAnimator : MonoBehaviour {
    [SerializeField] Shield shield;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        shield.OnShieldEvent += Shield_OnShieldEvent;
        
    }

    private void Shield_OnShieldEvent(object sender, Shield.OnShieldEventArgs e) {
        animator.SetTrigger(AnimatorStrings.shield);
    }

    private void Update() {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("(Empty)")) {
            spriteRenderer.enabled = false;  // Hide the sprite
        } else {
            spriteRenderer.enabled = true;   // Show the sprite
        }
        animator.SetBool(AnimatorStrings.isShielded, shield.IsShielded);
    }
}
