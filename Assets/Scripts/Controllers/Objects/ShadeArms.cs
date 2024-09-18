using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShadeArms : MonoBehaviour {
    [SerializeField] private Transform fireballPoint;
    [SerializeField] private Transform fireball;
    [SerializeField] private Shade shade;
    [SerializeField] private int framesDelayed = 100;
    private Transform shadeArms;

    private void Awake() {
        shadeArms = GetComponent<Transform>();
        shade.OnAttackEvent += Shade_OnAttackEvent;
    }

    private void Shade_OnAttackEvent(object sender, Shade.OnAttackEventArgs e) {
        StartCoroutine(DelayedFireballInstantiation(e));
    }

    private IEnumerator DelayedFireballInstantiation(Shade.OnAttackEventArgs e) {
        // Wait for 6 frames
        for (int i = 0; i < framesDelayed; i++) {
            yield return new WaitForEndOfFrame();
        }

        // Instantiate the fireball after 6 frames
        Transform fireballTransform = Instantiate(fireball, new Vector3(fireballPoint.position.x, fireballPoint.position.y, 0), Quaternion.identity);
        Vector3 shootDir = e.direction;
        fireballTransform.GetComponent<Fireball>().Setup(shootDir);
    }

}
