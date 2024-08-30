using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour {
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _health = 100;
    [SerializeField] private bool _isAlive = true;
    [SerializeField] private bool _isHit = false;
    [SerializeField] private bool isInvincible = false;
    [SerializeField] private float invincibilityTime = 0.25f;
    private float timeSinceHit = 0;

    public event EventHandler damageableDeath;
    public event EventHandler<OnHitEventArgs> damageableHit;
    public class OnHitEventArgs : EventArgs {
        public int damage;
        public Vector2 knockback;
    }

    public bool IsHit {
        get {
            return _isHit;
        }
        private set {
            _isHit = value;
        }
    }

    public int MaxHealth {
        get {
            return _maxHealth;
        }
        set {
            _maxHealth = value;
        }
    }

    public int Health {

        get {
            return _health;
        }
        set {
            _health = value;

            // If health drops below 0, the character is no longer alive 
            if (_health <= 0) {
                IsAlive = false;
            }
        }
    }

    public bool IsAlive {
        get {
            return _isAlive;
        }
        set {
            _isAlive = value;
            //animator.SetBool(AnimationStrings.isAlive, value);

            if (value == false) {
                damageableDeath?.Invoke(this, EventArgs.Empty);
                Destroy(gameObject);
            }
        }
    }


    public bool Hit(int damage, Vector2 knockback) {
        if (IsAlive && !isInvincible) {
            Health -= damage;
            isInvincible = true;
            timeSinceHit = 0;
            IsHit = true;
            damageableHit?.Invoke(this, new OnHitEventArgs {
                damage = damage,
                knockback = knockback
            });
            return true;
        }
        return false;
    }

    public bool Heal(int healthRestore) {
        if (IsAlive && Health < MaxHealth) {
            int maxHeal = Mathf.Max(MaxHealth - Health, 0);
            int actualHeal = Mathf.Min(maxHeal, healthRestore);
            Health += actualHeal;
            return true;
        } else {
            return false;
        }

    }

    private void Awake() {
    }

    public void Update() {
        if (isInvincible) {

            if (timeSinceHit > invincibilityTime) {
                // remove invincibility
                isInvincible = false;
                IsHit = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
    }


}


