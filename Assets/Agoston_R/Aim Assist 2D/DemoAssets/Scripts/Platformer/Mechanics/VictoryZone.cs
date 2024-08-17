using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Gameplay;
using UnityEngine;
using static Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Core.Simulation;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Mechanics
{
    /// <summary>
    /// Marks a trigger as a VictoryZone, usually used to end the current game level.
    /// </summary>
    public class VictoryZone : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D collider)
        {
            var p = collider.gameObject.GetComponent<PlayerController>();
            if (p != null)
            {
                var ev = Schedule<PlayerEnteredVictoryZone>();
                ev.victoryZone = this;
                PlatformerGame.Instance.EndGame();
            }
        }
    }
}