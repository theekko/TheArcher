using System.Collections.Generic;
using UnityEngine;

namespace Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.TargetSelection
{
    /// <summary>
    /// Calculates the forward axis of a transform, based on the enum it receives.
    /// </summary>
    public class ForwardDirectionCalculator
    {
        private readonly Dictionary<ForwardDirection, Vector3> forwardDirections = new Dictionary<ForwardDirection, Vector3>();

        public ForwardDirectionCalculator()
        {
            SetUpDirectionMappings();
        }

        /// <summary>
        /// Calculates the forward direction in local space.
        /// </summary>
        /// <param name="dir">The sepected enum for forward direction.</param>
        /// <param name="isFlipped">Whether the aim is flipped.</param>
        /// <returns>The aim's direction in local space.</returns>
        public Vector3 CalculateForwardLocalSpace(ForwardDirection dir, bool isFlipped)
        {
            return isFlipped ? forwardDirections[dir] * -1 : forwardDirections[dir];
        }

        private void SetUpDirectionMappings()
        {
            forwardDirections.Add(ForwardDirection.X, Vector3.right);
            forwardDirections.Add(ForwardDirection.Y, Vector3.up);
            forwardDirections.Add(ForwardDirection.Minus_X, Vector3.left);
            forwardDirections.Add(ForwardDirection.Minus_Y, Vector3.down);
        }
    }
}

