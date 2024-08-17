using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Core;
using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Model;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Gameplay
{
    /// <summary>
    /// This event is fired when user input should be enabled.
    /// </summary>
    public class EnablePlayerInput : Simulation.Event<EnablePlayerInput>
    {
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            var player = model.player;
            player.controlEnabled = true;
        }
    }
}