using Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.Helper.Numerics;
using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.TagManagement;
using System.Collections;
using UnityEngine;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.SpaceShooter
{
    public class EnemyInvader : MonoBehaviour, IDestroyable
    {
        public Color Color { get; private set; }

        public Vector3 Position => transform.position;

        public GameObject GameObject => gameObject;

        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private float ttl = 20f;

        private IInvaderDestroyer invaderDestroyer;

        private void OnEnable()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            StartCoroutine(WaitThenDestroySelf());
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var tag = collision.GetComponent<GameTag>();

            if (tag != null && tag.CompareGameTag("Wall"))
            {
                rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            }
        }

        public EnemyInvader SetColor(Color color)
        {
            spriteRenderer.color = color;
            this.Color = color;
            return this;
        }

        public EnemyInvader SetSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
            return this;
        }

        public EnemyInvader SetGameController(IInvaderDestroyer invaderDestroyer)
        {
            this.invaderDestroyer = invaderDestroyer;
            return this;
        }

        public void Shoot(Vector2 direction, float speed)
        {
            rb.velocity = direction.XYZ() * speed;
        }

        public void Die()
        {
            invaderDestroyer.DestroyObject(this);
        }

        private IEnumerator WaitThenDestroySelf()
        {
            yield return new WaitForSeconds(ttl);
            Destroy(gameObject);
        }
    }

}
