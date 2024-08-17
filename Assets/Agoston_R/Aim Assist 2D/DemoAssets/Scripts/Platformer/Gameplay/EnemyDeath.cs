using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Core;
using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Mechanics;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Gameplay
{
    /// <summary>
    /// Fired when the health component on an enemy has a hitpoint value of  0.
    /// </summary>
    /// <typeparam name="EnemyDeath"></typeparam>
    public class EnemyDeath : Simulation.Event<EnemyDeath>
    {
        public EnemyController enemy;

        public override void Execute()
        {
            enemy._collider.enabled = false;
            enemy.control.enabled = false;
            enemy.animator.SetTrigger("hurt");
        }
    }
}