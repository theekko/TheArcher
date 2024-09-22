using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallexEffect : MonoBehaviour {
    [SerializeField] private Camera cam;
    [SerializeField] private Transform followTarget;

    private Vector2 startingPosition;
    private Vector2 camMoveSinceStart;
    private float startingZ;
    private float parallaxFactor;
    private float zDistanceFromTarget;
    private float clippingPlane;

    private void Start() {
        startingPosition = transform.position;
        startingZ = transform.position.z;
    }

    private void FixedUpdate() {
        zDistanceFromTarget = transform.position.z - followTarget.transform.position.z;
        camMoveSinceStart = (Vector2)cam.transform.position - startingPosition;

        clippingPlane = (cam.transform.position.z + (zDistanceFromTarget > 0 ? cam.farClipPlane : cam.nearClipPlane));
        parallaxFactor = Mathf.Abs(zDistanceFromTarget) / clippingPlane;

        Vector2 newPosition = startingPosition + camMoveSinceStart * parallaxFactor;
        transform.position = new Vector3(newPosition.x, newPosition.y, startingZ);
    }
}
