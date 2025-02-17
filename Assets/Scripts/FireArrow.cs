using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArrow : MonoBehaviour {
    [SerializeField] private Transform arrowTransform;
    [SerializeField] private Transform arrow;
    [SerializeField] private Transform teleportArrow;
    [SerializeField] private Transform empoweredArrow;

    private void Awake() {
        GetComponent<BowController>().OnFireSuccessEvent += FireArrow_OnFireEvent;
        GetComponent<BowController>().OnFireEmpoweredSuccessEvent += FireArrow_OnFireEmpoweredSuccessEvent;
        GetComponent<BowController>().OnFireTeleportSuccessEvent += FireArrow_OnFireTeleportSuccessEvent;
    }



    private void FireArrow_OnFireEvent(object sender, BowController.OnFireSuccessEventArgs e) {
        Transform arrowTransform = Instantiate(arrow, new Vector3(e.bowEndpointPosition.x, e.bowEndpointPosition.y, 0), Quaternion.identity);
        Vector3 shootDir = e.shootDirection;
        arrowTransform.GetComponent<Arrow>().Setup(shootDir);
    }

    private void FireArrow_OnFireEmpoweredSuccessEvent(object sender, BowController.OnFireSuccessEventArgs e) {
        Transform arrowTransform = Instantiate(empoweredArrow, new Vector3(e.bowEndpointPosition.x, e.bowEndpointPosition.y, 0), Quaternion.identity);
        Vector3 shootDir = e.shootDirection;
        arrowTransform.GetComponent<EmpoweredArrow>().Setup(shootDir);
    }


    private void FireArrow_OnFireTeleportSuccessEvent(object sender, BowController.OnFireTeleportSuccessEventArgs e) {
        Transform arrowTransform = Instantiate(teleportArrow, new Vector3(e.bowEndpointPosition.x, e.bowEndpointPosition.y, 0), Quaternion.identity);
        Vector3 shootDir = e.shootDirection;
        arrowTransform.GetComponent<TeleportArrow>().Setup(shootDir, e.destroyTimer);
    }
}
