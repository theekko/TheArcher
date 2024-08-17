using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Mechanics;
using UnityEngine;
using static Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Core.Simulation;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Weapon
{
    public class Bullet : MonoBehaviour
    {
        public int damage = 1;
        public float ttl = 0.2f;

        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Shoot(Vector2 direction, float velocity)
        {
            rb.velocity = direction.normalized * velocity;
            transform.rotation = Quaternion.Euler(Vector3.forward * Vector2.SignedAngle(Vector2.right, direction));
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                var resolution = Schedule<BulletEnemyCollision>();
                resolution.enemy = enemy;
                resolution.bullet = this;
            }

            Destroy(gameObject);
        }
    }

}
