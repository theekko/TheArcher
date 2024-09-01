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
    public event EventHandler<OnHealthChangedEventArgs> healthChanged;
    public class OnHealthChangedEventArgs : EventArgs {
        public int health;
        public int maxHealth;
    }
    public event EventHandler<OnMaxHealthChangedEventArgs> maxHealthChanged;
    public class OnMaxHealthChangedEventArgs : EventArgs {
        public int maxHealth;
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
            maxHealthChanged?.Invoke(this, new OnMaxHealthChangedEventArgs {
                maxHealth = _maxHealth
            });
        }
    }

    public int Health {

        get {
            return _health;
        }
        set {
            _health = value;
            healthChanged?.Invoke(this, new OnHealthChangedEventArgs {
                health = _health,
                maxHealth = _maxHealth
            });

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


            if (value == false) {
                damageableDeath?.Invoke(this, EventArgs.Empty);
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
        maxHealthChanged?.Invoke(this, new OnMaxHealthChangedEventArgs {
            maxHealth = _maxHealth
        });
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


