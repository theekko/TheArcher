using System.Collections;
using UnityEngine;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.SpaceShooter
{
    public class DestroyParticleController : MonoBehaviour
    {
        private ParticleSystem explosion;

        private void OnEnable()
        {
            explosion = GetComponent<ParticleSystem>();
        }

        public void PlayThenDestroy(Vector3 position, Color color)
        {
            var main = explosion.main;
            main.startColor = color;
            transform.position = position;
            explosion.Play();

            var ttl = explosion.main.duration + 0.1f;
            StartCoroutine(DestroySelf(ttl));
        }

        private IEnumerator DestroySelf(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            explosion.Stop();
            Destroy(gameObject);
        }
    }
}

