using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrowRaycast {
  
    public static void Fire(Vector3 firePosition, Vector3 fireDirection) { 
        RaycastHit2D raycastHit2D = Physics2D.Raycast(firePosition, fireDirection);

        if (raycastHit2D.collider != null) {
            Damageable damageable = raycastHit2D.collider.GetComponent<Damageable>();
            if (damageable != null) {
                Debug.Log("Hit");
                bool gotHit = damageable.Hit(1, Vector2.zero);

            }
        }
    }
}
