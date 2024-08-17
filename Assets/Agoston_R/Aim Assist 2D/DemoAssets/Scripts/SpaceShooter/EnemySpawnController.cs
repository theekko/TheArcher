using UnityEngine;
using Random = UnityEngine.Random;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.SpaceShooter
{
    public class EnemySpawnController : MonoBehaviour
    {
        public float spawnInterval = 1f;
        public Sprite[] enemySprites;
        public Color[] colors;
        public EnemyInvader enemyPrefab;
        public float enemyVelocity = 5f;

        private BoxCollider2D spawnArea;
        private float timeAccumulator;

        private SpaceShipGame gameController;

        private bool spawnEnabled = true;

        private void Awake()
        {
            spawnArea = GetComponent<BoxCollider2D>();
            gameController = FindObjectOfType<SpaceShipGame>();
            gameController.OnGameEnded += OnGameEnded;
        }

        private void Start()
        {
            spawnEnabled = true;
        }

        private void OnGameEnded()
        {
            spawnEnabled = false;
        }

        private void Update()
        {
            timeAccumulator += Time.deltaTime;
            if (spawnEnabled && timeAccumulator > spawnInterval)
            {
                timeAccumulator = 0;
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            var spawnLocation = CalculateSpawnLocation();
            var enemy = Instantiate(enemyPrefab, spawnLocation, Quaternion.identity);
            enemy
                .SetColor(GetRandomColor())
                .SetSprite(GetRandomSprite())
                .SetGameController(gameController)
                .Shoot(GetRandomDirection(), enemyVelocity);
        }

        private Vector2 GetRandomDirection()
        {
            var y = Random.Range(0.0f, 1.0f);
            var x = Random.Range(-y, y);
            return new Vector2(x, -y).normalized;
        }

        private Sprite GetRandomSprite()
        {
            return enemySprites[Random.Range(0, enemySprites.Length)];
        }

        private Color GetRandomColor()
        {
            return colors[Random.Range(0, colors.Length)];
        }

        private Vector3 CalculateSpawnLocation()
        {
            var bounds = spawnArea.bounds;
            var x = Random.Range(bounds.min.x, bounds.max.x);
            var y = Random.Range(bounds.min.y, bounds.max.y);
            return new Vector3(x, y, 0);
        }
    }

}
