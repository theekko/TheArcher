using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeArmor : MonoBehaviour {
    private Damageable damageable;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite slimeArmored;
    [SerializeField] private Sprite slime;

    private Collider2D armor;

    private void OnTriggerEnter2D(Collider2D collision) {
        EmpoweredArrow empoweredArrow = collision.gameObject.GetComponent<EmpoweredArrow>();
        if (empoweredArrow != null) {
            Destroy(empoweredArrow);
            damageable.enabled = true;
            spriteRenderer.sprite = slime;
            armor.enabled = false;
        }
    }

    private void Awake() {
        // Initialize the SpriteRenderer on the current object
        if (spriteRenderer == null) {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        spriteRenderer.sprite = slimeArmored;


        // Initialize the Damageable component on the parent object
        if (damageable == null) {
            damageable = GetComponentInParent<Damageable>();
            armor = GetComponent<Collider2D>();
        }
    }
}
