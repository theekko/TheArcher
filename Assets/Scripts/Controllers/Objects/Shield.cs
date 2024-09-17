using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;

public class Shield : MonoBehaviour {
    [SerializeField] private float shieldInvincibilityTime = 0.25f;
    //[SerializeField] private int maxNumShields = 3;
    [SerializeField] private int numShields = 3;
    //[SerializeField] private float shieldRefreshCooldownMax = 10;
    [SerializeField] private bool _canShield = true;
    [SerializeField] private bool _isShielded = false;
    //private float timeShieldRefresh = 0;
    private float timeSinceShield = 0;
    private Damageable damageable;

    public event EventHandler<OnShieldEventArgs> OnShieldEvent;
    public class OnShieldEventArgs : EventArgs {
        public float shieldInvincibilityTime;
    }

    public bool CanShield {
        get { 
            return _canShield;
        } private set { 
            _canShield = value;
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


    public void OnShield(InputAction.CallbackContext context) {
        if (context.performed && numShields > 0 && CanShield) {
            numShields -= 1;
            CanShield = false;
            IsShielded = true;
            OnShieldEvent?.Invoke(this, new OnShieldEventArgs {
                shieldInvincibilityTime = shieldInvincibilityTime
            });
        }
    }

    private void Update() {
        //Shield Refresh
        //if (numShields < maxNumShields) { 
        //    timeShieldRefresh += Time.deltaTime;
        //    if (timeShieldRefresh >= shieldRefreshCooldownMax) {
        //        numShields += 1;
        //        timeShieldRefresh = 0;
        //    }
        //}

        //Shield Cooldown
        if (!CanShield) {
            timeSinceShield += Time.deltaTime;
            if (timeSinceShield >= shieldInvincibilityTime) {
                CanShield = true;
                IsShielded = false;
                timeSinceShield = 0;
            }
        }
    }

    private void Awake() {
        damageable = GetComponentInParent<Damageable>();
    }
}