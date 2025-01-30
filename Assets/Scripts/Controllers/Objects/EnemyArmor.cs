using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArmor : MonoBehaviour {
    [SerializeField] private bool _isArmored = true;
    [SerializeField] private Wasp wasp;

    private Collider2D armor;
    private Damageable damageable;

    public event EventHandler armorHitEvent;
    static public event EventHandler empoweredArrowHitEvent;
    public bool IsArmored {
        get {
            return _isArmored;
        }
        private set { 
            _isArmored = value;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        EmpoweredArrow empoweredArrow = collision.gameObject.GetComponent<EmpoweredArrow>();
        Arrow arrow = collision.gameObject.GetComponent<Arrow>();
        if (empoweredArrow != null) {
            Destroy(empoweredArrow.gameObject);
            damageable.enabled = true;
            armor.enabled = false;
            IsArmored = false;
            empoweredArrowHitEvent?.Invoke(this, EventArgs.Empty);
        }
        if (arrow != null) {
            Destroy(arrow.gameObject);
            armorHitEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Wasp_ArmoredCapturedArrowEvent(object sender, EventArgs e) {
        damageable.enabled = true;
        armor.enabled = false;
        IsArmored = false;
    }

    private void Awake() {
        damageable = GetComponentInParent<Damageable>();
        armor = GetComponent<Collider2D>();
        if (wasp != null) {
            wasp.ArmoredCapturedArrowEvent += Wasp_ArmoredCapturedArrowEvent;
        }
    }


}
