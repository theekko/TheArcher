using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.TagManagement;
using UnityEngine;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.SpaceShooter
{
    public class EnemyFinish : MonoBehaviour
    {
        private SpaceShipGame gameController;

        private void Start()
        {
            gameController = FindObjectOfType<SpaceShipGame>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var tag = collision.GetComponent<GameTag>();

            if (tag == null || !tag.CompareGameTag("Enemy"))
            {
                return;
            }

            Destroy(collision.gameObject);
            gameController.EndGame();
        }
    }
}

