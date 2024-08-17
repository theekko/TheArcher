using Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.Helper.Caching;
using Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.Target;
using UnityEngine;

namespace Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.TargetSelection
{
    /// <summary>
    /// Finds and selects a given target for the aim assists and invokes events on the target if any are defined.
    ///
    /// Separating the selection can enable you to use multiple aim assists together with no additional performance hit from the target selection process.
    /// </summary>
    public class TargetSelector : MonoBehaviour
    {
        [Tooltip("The radius of the aim assist in metres.")]
        [Min(0f)]
        public float aimAssistRadius = 0.5f;

        [Tooltip("The near clip distance in metres. Aim assist doesn't work for targets closer than this.")]
        [Min(0f)]
        public float nearClipDistance = 0.5f;

        [Tooltip("The far clip distance in metres. Aim assist doesn't work for target further than this. Increasing this takes more computing power.")]
        [Min(0f)]
        public float farClipDistance = 50f;

        [Header("Settings")]
        [Tooltip("Layers to take into account during the aim assist.")]
        public LayerMask layerMask;

        [Tooltip("The minimum Z depth that the physics takes into account.")]
        public float minDepth = -Mathf.Infinity;

        [Tooltip("The maximum Z depth that the physics takes into account.")]
        public float maxDepth = Mathf.Infinity;

        [Tooltip("The direction the player is shooting at by default. In a platformer tipically it's X, in a topdown game it could be Z. " +
            "This direction of the aim origin will determine where the aim assist will shoot.")]
        public ForwardDirection forwardDirection = ForwardDirection.X;

        [Tooltip("Flips the forward direction. Useful when the player sprite isn't rotated when the player turns but is just flipped, but the gun's aim points to the same direction as before the flip.")]
        public bool flip;

        public NotifyTargetFound OnTargetSelected { get; } = new NotifyTargetFound();
        public NotifyTargetFound OnTargetLost { get; } = new NotifyTargetFound();

        /// <summary>
        /// The aim origin is the transform of the GameObject that has the aim assist on it. 
        /// </summary>
        public Transform AimOrigin => transform;

        public Vector3 AimDirection 
        { 
            get
            {
                return AimOrigin.TransformVector(directionCalculator.CalculateForwardLocalSpace(forwardDirection, flip)).normalized;
            }
        }

        /// <summary>
        /// The target that is currently found by the selector. Null if currently no targets are found.
        /// </summary>
        public AimAssistTarget Target { get; private set; }
        
        private readonly Cache<AimAssistTarget> _targetCache = Cache<AimAssistTarget>.Instance;
        private readonly SelectedTargetStore _selectedTargetStore = new SelectedTargetStore();
        private readonly ForwardDirectionCalculator directionCalculator = new ForwardDirectionCalculator();

        private void FixedUpdate()
        {
            var foundTarget = SelectClosestTarget();

            if (foundTarget != null)
            {
                NotifyOnTargetFound(foundTarget);
            } 
            else
            {
                NotifyOnTargetLost();
            }

            Target = foundTarget;
        }

        private void NotifyOnTargetFound(AimAssistTarget foundTarget)
        {
            if (foundTarget != Target)
            {
                OnTargetSelected?.Invoke(foundTarget);
            }
        }

        private void NotifyOnTargetLost()
        {
            if (Target != null)
            {
                OnTargetLost?.Invoke(Target);
            }
        }

        private AimAssistTarget SelectClosestTarget()
        {
            var target = SelectTarget();
            _selectedTargetStore.ProcessTarget(target);
            return target;
        }

        private AimAssistTarget SelectTarget()
        {
            var startPoint = AimOrigin.position + AimDirection * nearClipDistance;

            var hit = Physics2D.CircleCast(startPoint, aimAssistRadius, AimDirection, farClipDistance, layerMask, minDepth, maxDepth);

            if (hit.collider == null)
            {
                return null;
            }

            var target = _targetCache.FindOrInsert(hit.collider);

            if (target)
            {
                return target;
            }

            var raycastHit = Physics2D.Raycast(startPoint, AimDirection, farClipDistance, layerMask, minDepth, maxDepth);

            if (raycastHit.collider == null)
            {
                return null;
            }

            return _targetCache.FindOrInsert(raycastHit.collider);
        }
    }
}