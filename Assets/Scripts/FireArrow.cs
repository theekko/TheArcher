using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArrow : MonoBehaviour {
    [SerializeField] private Transform arrow;

    private void Awake() {
        GetComponent<BowController>().OnFireEvent += FireArrow_OnFireEvent;
    }

    private void FireArrow_OnFireEvent(object sender, BowController.OnFireEventArgs e) {
        Transform arrowTransform = Instantiate(arrow, e.bowEndpointPosition, Quaternion.identity);

        Vector3 shootDir = e.shootPosition;
        arrowTransform.GetComponent<Arrow>().Setup(shootDir);
    }
}
