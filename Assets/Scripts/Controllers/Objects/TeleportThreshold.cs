using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TeleportThreshold : MonoBehaviour {
    private Collider2D teleportThreshold;

    public event EventHandler OnThresholdCross;

    private void Awake() {
        teleportThreshold = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        OnThresholdCross?.Invoke(this, EventArgs.Empty);
    }
}
