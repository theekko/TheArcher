using UnityEngine;

namespace Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.Helper.Info
{
    /// <summary>
    /// Physics information that returns the player's velocity.
    /// </summary>
    public class PlayerTransformInfo : MonoBehaviour
    {
        private Vector3 playerPosition;
        public Transform PlayerTransform { get; set; }
        
        /// <summary>
        /// The Player's velocity
        /// </summary>
        public Vector3 Velocity { get; private set; }

        private void Update()
        {
            var currentPosition = PlayerTransform.position;
            var velocity = (currentPosition - playerPosition) / Time.deltaTime;
            Velocity = new Vector3(velocity.x, velocity.y, 0);
            playerPosition = currentPosition;
        }
    }
}