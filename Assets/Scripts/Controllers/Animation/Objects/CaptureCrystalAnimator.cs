using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureCrystalAnimator : MonoBehaviour {
    private Wasp wasp;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        wasp = GetComponentInParent<Wasp>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    private void Shield_OnShieldEvent(object sender, Shield.OnShieldEventArgs e) {
        animator.SetTrigger(AnimatorStrings.shield);
    }

    private void Update() {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("(Empty)")) {
            spriteRenderer.enabled = false;
        } else {
            spriteRenderer.enabled = true;
        }
        animator.SetBool(AnimatorStrings.arrowCaptured, wasp.ArrowCaptured);
    }
}
