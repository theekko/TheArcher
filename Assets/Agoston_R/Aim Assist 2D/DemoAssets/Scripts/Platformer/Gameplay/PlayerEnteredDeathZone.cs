using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Core;
using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Mechanics;
using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Model;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Gameplay
{
    /// <summary>
    /// Fired when a player enters a trigger with a DeathZone component.
    /// </summary>
    /// <typeparam name="PlayerEnteredDeathZone"></typeparam>
    public class PlayerEnteredDeathZone : Simulation.Event<PlayerEnteredDeathZone>
    {
        public DeathZone deathzone;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            Simulation.Schedule<PlayerDeath>(0);
        }
    }
}