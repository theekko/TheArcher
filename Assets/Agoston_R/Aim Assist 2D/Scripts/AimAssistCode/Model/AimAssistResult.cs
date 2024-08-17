using UnityEngine;

namespace Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.Model
{
    /// <summary>
    /// Contains output from the aim assist calculations.
    ///
    /// The values are unclamped - they are additions. When assigning to properties with a limit like the camera pitch, the resulting value has to be clamped before assigning to the camera pitch.
    /// 
    /// The result is valid for one frame, when it is calculated. Do not store the data as it needs to be updated every frame. 
    /// </summary>
    public struct AimAssistResult
    {
        /// <summary>
        /// The rotation addition calculated, in degrees.
        /// </summary>
        public float RotationAdditionInDegrees { get; }
        
        /// <summary>
        /// The rotation addition in degrees, that is represented along the forward Z axis.
        /// </summary>
        public Vector3 RotationAddition { get; }
       
        public AimAssistResult(float rotationAdditionInDegrees)
        {
            RotationAdditionInDegrees = rotationAdditionInDegrees;
            RotationAddition = Vector3.forward * rotationAdditionInDegrees;
        }

        /// <summary>
        /// Returns an empty result. You can add this to your rotations as if they were actual populated values and they'll make no difference.
        /// </summary>
        public static AimAssistResult Empty => new AimAssistResult();
    }
}