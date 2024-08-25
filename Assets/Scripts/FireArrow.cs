using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArrow : MonoBehaviour {
    [SerializeField] private Transform arrowTransform;
    [SerializeField] private Transform arrow;
    [SerializeField] private Transform teleportArrow;

    private void Awake() {
        GetComponent<BowController>().OnFireSuccessEvent += FireArrow_OnFireEvent;
        GetComponent<BowController>().OnFireTeleportSuccessEvent += FireArrow_OnFireTeleportSuccessEvent;
    }


    private void FireArrow_OnFireEvent(object sender, BowController.OnFireSuccessEventArgs e) {

        // Phyics shot
        Transform arrowTransform = Instantiate(arrow, e.bowEndpointPosition, Quaternion.identity);
        Vector3 shootDir = e.shootPosition;
        arrowTransform.GetComponent<Arrow>().Setup(shootDir);


        // Transform Shot
        //Transform arrowTransform = Instantiate(arrowTransform, e.bowEndpointPosition, Quaternion.identity);
        //Vector3 shootDir = e.shootPosition;
        //arrowTransform.GetComponent<ArrowTransform>().Setup(shootDir);

        // Raycast Shot
        //Debug.DrawLine(e.bowEndpointPosition, e.shootPosition, Color.white, .1f);
        //Vector3 shootDir = e.shootPosition;
        //ArrowRaycast.Fire(e.bowEndpointPosition, shootDir);
    }

    private void FireArrow_OnFireTeleportSuccessEvent(object sender, BowController.OnFireTeleportSuccessEventArgs e) {
        Transform arrowTransform = Instantiate(teleportArrow, e.bowEndpointPosition, Quaternion.identity);
        Vector3 shootDir = e.shootPosition;
        arrowTransform.GetComponent<TeleportArrow>().Setup(shootDir);
    }
}
