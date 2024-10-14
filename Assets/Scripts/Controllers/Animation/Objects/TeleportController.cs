using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportController : MonoBehaviour {
    [SerializeField] private TeleportStartAnimator startAnimator;
    [SerializeField] private TeleportEndAnimator endAnimator;
    [SerializeField] private TeleportStartAnimator shadeStartAnimator;
    [SerializeField] private TeleportEndAnimator shadeEndAnimator;
    [SerializeField] private Shade shade;


    private void Start() {
        Player.Instance.teleportEvent += Instance_teleportEvent;
        shade.teleportEvent += Shade_teleportEvent;
    }

    private void Shade_teleportEvent(object sender, Shade.teleportEventArgs e) {
        Vector2 startPosition = e.initialPosition;
        Vector2 endPosition = shade.transform.position;

        // Play the first animation (before teleport)
        shadeStartAnimator.PlayStartAnimation(startPosition);

        // Play the second animation (after teleport)
        shadeEndAnimator.PlayEndAnimation(endPosition);
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
