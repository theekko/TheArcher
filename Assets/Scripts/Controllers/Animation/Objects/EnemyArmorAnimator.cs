using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArmorAnimator : MonoBehaviour {
    [SerializeField] private EnemyArmor enemyArmor;
    [SerializeField] float alpha = 1;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Transform armorTransform;

    private void Awake() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        armorTransform = GetComponent<Transform>();
        enemyArmor.armorHitEvent += EnemyArmor_armorHitEvent;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);

    }

    private void EnemyArmor_armorHitEvent(object sender, System.EventArgs e) {
        animator.SetTrigger(AnimatorStrings.hit);
    }

    private void Update() {
        animator.SetBool(AnimatorStrings.isArmored, enemyArmor.IsArmored);
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("(Empty)")) {
            spriteRenderer.enabled = false;
        } 
    }
}
