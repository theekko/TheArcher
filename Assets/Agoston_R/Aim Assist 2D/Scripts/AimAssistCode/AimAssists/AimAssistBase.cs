using Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.Target;
using Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.TargetSelection;
using UnityEngine;

namespace Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.AimAssists
{
    /// <summary>
    /// Base class for all aim assist classes that handles the setup of the target selector.
    /// </summary>
    [RequireComponent(typeof(TargetSelector))]
    public abstract class AimAssistBase : MonoBehaviour
    {
        /// <summary>
        /// The current target available from the selector
        /// </summary>
        public AimAssistTarget Target => targetSelector.Target;
        
        /// <summary>
        /// The radius of the selector in metres
        /// </summary>
        public float AimAssistRadius => targetSelector.aimAssistRadius;
        
        /// <summary>
        /// The near clip distance of the selector
        /// </summary>
        public float NearClipDistance => targetSelector.nearClipDistance;
        
        /// <summary>
        /// The far clip distance of the selector
        /// </summary>
        public float FarClipDistance => targetSelector.farClipDistance;

        /// <summary>
        /// The aim direction vector of the gun. Adjusted for flipping and using either the X or Y axis to aim.
        /// </summary>
        public Vector2 AimDirection => targetSelector.AimDirection;

        /// <summary>
        /// The aim origin, or gun of the aim assist. Can be the player itself, or a gun object on the player.
        /// 
        /// It will be rotated as the aim assist takes effect.
        /// </summary>
        public Transform Gun => targetSelector.AimOrigin;

        /// <summary>
        /// Event that triggers once on a target is found.
        /// </summary>
        public NotifyTargetFound OnTargetFound => targetSelector.OnTargetSelected;

        /// <summary>
        /// Event that triggers once a target is lost.
        /// </summary>
        public NotifyTargetFound OnTargetLost => targetSelector.OnTargetLost;        
        
        [Header("Master switch")] [Tooltip("Enable aim assist")]
        public bool aimAssistEnabled = true;
        
        private TargetSelector targetSelector;
        
        protected virtual void Awake()
        {
            SetUpTargetSelector();
        }

        private void SetUpTargetSelector()
        {
            targetSelector = GetComponent<TargetSelector>();
        }
    }
}