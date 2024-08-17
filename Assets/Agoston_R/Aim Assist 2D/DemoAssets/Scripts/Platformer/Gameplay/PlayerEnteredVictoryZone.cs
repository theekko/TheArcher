using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Core;
using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Mechanics;
using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Model;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Gameplay
{

    /// <summary>
    /// This event is triggered when the player character enters a trigger with a VictoryZone component.
    /// </summary>
    /// <typeparam name="PlayerEnteredVictoryZone"></typeparam>
    public class PlayerEnteredVictoryZone : Simulation.Event<PlayerEnteredVictoryZone>
    {
        public VictoryZone victoryZone;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            model.player.animator.SetTrigger("victory");
            model.player.controlEnabled = false;
        }
    }
}