using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class EmpoweredElectricityAnimator : MonoBehaviour {
    [SerializeField] private BowController bowController;
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
        animator.SetBool(AnimatorStrings.isEmpowered, bowController.IsEmpoweredShot);
    }
}

