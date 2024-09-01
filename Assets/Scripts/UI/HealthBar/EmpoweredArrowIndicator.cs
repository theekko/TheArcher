using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmpoweredArrowIndicator : MonoBehaviour {
    [SerializeField] private BowController bowController;
    [SerializeField] private Image empoweredArrowIndicator;



    private void Update() {
        if (bowController.IsEmpoweredShot) {
            empoweredArrowIndicator.enabled = true;
        } else {
            empoweredArrowIndicator.enabled = false;
        }
    }
}
