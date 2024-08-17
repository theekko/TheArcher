using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Core;
using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Mechanics;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player character lands after being airborne.
    /// </summary>
    /// <typeparam name="PlayerLanded"></typeparam>
    public class PlayerLanded : Simulation.Event<PlayerLanded>
    {
        public PlayerController player;

        public override void Execute()
        {

        }
    }
}