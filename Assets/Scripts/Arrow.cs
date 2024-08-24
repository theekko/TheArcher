using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    private Vector3 shootDir;
    [SerializeField] float moveSpeed = 1f;

    public void Setup(Vector3 shootDir) {
        this.shootDir = shootDir;
    }

    private void Update() {
        transform.position += shootDir * moveSpeed * Time.deltaTime;
    }
}
