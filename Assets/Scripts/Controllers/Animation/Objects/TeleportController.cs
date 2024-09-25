using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportController : MonoBehaviour {
    [SerializeField] TeleportStartAnimator startAnimator;
    [SerializeField] TeleportEndAnimator endAnimator;

    private void Start() {
        Player.Instance.teleportEvent += Instance_teleportEvent;
    }
    private void Instance_teleportEvent(object sender, Player.teleportEventArgs e) {
        Vector2 startPosition = e.initialPosition;
        Vector2 endPosition = Player.Instance.transform.position;

        // Play the first animation (before teleport)
        startAnimator.PlayStartAnimation(startPosition);

        // Play the second animation (after teleport)
        endAnimator.PlayEndAnimation(endPosition);
    }
}
