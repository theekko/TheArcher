using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAnimator : MonoBehaviour {
    [SerializeField] private BowController bow;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    private void Update() {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("(Empty)")) {
            spriteRenderer.enabled = false;
        } else {
            spriteRenderer.enabled = true;
        }
        animator.SetBool(AnimatorStrings.isDrawing, bow.IsDrawing);
    }
}
