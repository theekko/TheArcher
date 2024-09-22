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
    [SerializeField] private bool _isInvincible = false;
    [SerializeField] private float hitInvincibilityTime = 0.25f;
    [SerializeField] private bool _isShielded = false;
    private float timeSinceHit = 0;
    private float timeSinceShield = 0;
    private float shieldInvincibilityTime;

    public static event EventHandler damageableEnemyDeath;
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

    public bool IsShielded {
        get {
            return _isShielded;
        }
        private set {
            _isShielded = value;
        }
    }

    public bool IsInvincible {
        get {
            return _isInvincible;
        }
        private set {
            _isInvincible = value;
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
                if (gameObject.layer == LayerMask.NameToLayer(LayerStrings.Enemies) || gameObject.layer == LayerMask.NameToLayer(LayerStrings.EnemySlime)) {
                    damageableEnemyDeath?.Invoke(this, EventArgs.Empty);
                }

            }
        }
    }


    public bool Hit(int damage, Vector2 knockback) {
        if (IsAlive && !IsInvincible) {
            Health -= damage;
            IsInvincible = true;
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
    private void Damageable_OnShieldEvent(object sender, Shield.OnShieldEventArgs e) {
        IsShielded = true;
        IsInvincible = true;
        timeSinceShield = 0;
        shieldInvincibilityTime = e.shieldInvincibilityTime;
    }

    private void Awake() {
        maxHealthChanged?.Invoke(this, new OnMaxHealthChangedEventArgs {
            maxHealth = _maxHealth
        });
        if (gameObject.layer == LayerMask.NameToLayer(LayerStrings.Player)) {
            FindObjectOfType<Shield>().OnShieldEvent += Damageable_OnShieldEvent;
        }
    }


    public void Update() {
        // Invincible due to damage
        if (IsInvincible && IsHit) {
            if (timeSinceHit > hitInvincibilityTime) {
                // remove invincibility
                IsInvincible = false;
                IsHit = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }

        // Invincible due to shield
        if (IsInvincible && IsShielded) {
            if (timeSinceShield > shieldInvincibilityTime) {
                // remove invincibility
                IsInvincible = false;
                IsShielded = false;
                timeSinceShield = 0;
            }

            timeSinceShield += Time.deltaTime;
        }
    } 
}


