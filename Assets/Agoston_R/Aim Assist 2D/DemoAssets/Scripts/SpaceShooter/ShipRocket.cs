using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.TagManagement;
using UnityEngine;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.SpaceShooter
{
    public class ShipRocket : MonoBehaviour
    {
        private Rigidbody2D rb;

        private void OnEnable()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Shoot(Vector2 direction, float speed)
        {
            rb.velocity = direction * speed;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var tag = collision.gameObject.GetComponent<GameTag>();

            if (tag == null)
            {
                return;
            }

            if (tag.CompareGameTag("Enemy"))
            {
                collision.GetComponent<EnemyInvader>().Die();
            }
            else if (tag.CompareGameTag("Wall") || tag.CompareGameTag("Umbrella"))
            {
                Destroy(gameObject);
            }
        }
    }

}
