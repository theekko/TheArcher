using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shield : MonoBehaviour {
    [SerializeField] private float shieldInvincibilityTime = 0.25f;
    [SerializeField] private int maxNumShields = 3;
    [SerializeField] private int numShields = 3;
    [SerializeField] private float shieldCooldownMax = 10;
    [SerializeField] private bool _canShield = true;
    [SerializeField] private float timeUntilCanShield = 5f;
    private float timeShieldRefresh = 0;
    private float timeSinceShield = 0;

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


    public void OnShield(InputAction.CallbackContext context) {

        if (context.performed && numShields > 0 && CanShield) {
            numShields -= 1;
            CanShield = false;
            OnShieldEvent?.Invoke(this, new OnShieldEventArgs {
                shieldInvincibilityTime = shieldInvincibilityTime
            });
        }
    }

    private void Update() {
        if (numShields < maxNumShields) { 
            timeShieldRefresh += Time.deltaTime;
            if (timeShieldRefresh >= shieldCooldownMax) {
                numShields += 1;
                timeShieldRefresh = 0;
            }
        }
        if (!CanShield) {
            timeSinceShield += Time.deltaTime;
            if (timeSinceShield >= timeUntilCanShield) {
                CanShield = true;
                timeSinceShield = 0;
            }
        }
    }
}
