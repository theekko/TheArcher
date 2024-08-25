using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArrow : MonoBehaviour {
    [SerializeField] private Transform arrow;
    [SerializeField] private Transform arrowPhysics;

    private void Awake() {
        GetComponent<BowController>().OnFireSuccessEvent += FireArrow_OnFireEvent;
    }

    private void FireArrow_OnFireEvent(object sender, BowController.OnFireSuccessEventArgs e) {

        // Phyics shot
        Transform arrowTransform = Instantiate(arrowPhysics, e.bowEndpointPosition, Quaternion.identity);
        Vector3 shootDir = e.shootPosition;
        arrowTransform.GetComponent<ArrowPhysics>().Setup(shootDir);


        // Transform Shot
        //Transform arrowTransform = Instantiate(arrow, e.bowEndpointPosition, Quaternion.identity);
        //Vector3 shootDir = e.shootPosition;
        //arrowTransform.GetComponent<Arrow>().Setup(shootDir);

        // Raycast Shot
        //Debug.DrawLine(e.bowEndpointPosition, e.shootPosition, Color.white, .1f);
        //Vector3 shootDir = e.shootPosition;
        //ArrowRaycast.Fire(e.bowEndpointPosition, shootDir);
    }
}
