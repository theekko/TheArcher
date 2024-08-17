using static Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Core.Simulation;
using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Gameplay;
using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Mechanics;
using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Core;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Weapon
{
    public class BulletEnemyCollision : Simulation.Event<BulletEnemyCollision>
    {
        public EnemyController enemy;
        public Bullet bullet;

        public override void Execute()
        {
            var health = enemy.GetComponent<Health>();
            if (health != null)
            {
                health.Decrement();
                if (!health.IsAlive)
                {
                    Schedule<EnemyDeath>().enemy = enemy;
                }
            }
            else
            {
                Schedule<EnemyDeath>().enemy = enemy;
            }
        }
    }

}
