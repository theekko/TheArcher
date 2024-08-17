using UnityEngine;
using Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.Target;
using Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.Helper.Numerics;

namespace Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.AimAssists
{
    /// <summary>
    /// Slows down the look input using a curve to ease up aim on the target.
    /// </summary>
    public class PrecisionAim : AimAssistBase
    {
        [Header("Sensitivity")]
        [Tooltip("The sensitivity multiplier at the center of the aim assist. This will be lerped from the outer edge of the radius.")]
        [Range(0.001f, 0.99f)]
        public float sensitivityMultiplierAtCenter = 0.18f;

        [Tooltip("The sensitivity multiplier at the edge of the aim assist. This will be eased out back to the original sensitivity when the assist loses the target. " +
            "Has to be more than center multiplier (or will be set to center multiplier).")]
        [Range(0.1f, 0.99f)]
        public float sensitivityMultiplierAtEdge = 0.5f;

        [Header("Ease Out")]
        [Tooltip("The time in seconds to regain the original input sensitivity after leaving the target." +
            "Helps get rid of unnatural, robotic stutter from the aim.")]
        [Min(0.01f)]
        public float timeToRegainOriginalInputSensitivity = 1f;

        private float timeAccumulator;

        protected override void Awake()
        {
            base.Awake();
            SubscribeToTargetSelectorEvents();
            SetUpTimeAccumulator();
        }

        private void OnDestroy()
        {
            TearDownTargetSelectorEvents();
        }

        /// <summary>
        /// Calculates the slowed down player input delta using the curve.
        ///
        /// Receives a look input delta, returns a modified look input delta.
        ///
        /// Before calculating your rotations from the player input, run that input through this.
        /// </summary>
        /// <param name="input">inputs: the player's look input delta</param>
        /// <returns>the modified look input delta</returns>
        public Vector2 AssistAim(Vector2 input)
        {
            if (!aimAssistEnabled)
            {
                return input;
            }

            var target = Target;

            if (sensitivityMultiplierAtEdge < sensitivityMultiplierAtCenter)
            {
                sensitivityMultiplierAtEdge = sensitivityMultiplierAtCenter;
            }

            if (target == null)
            {
                return LerpEaseOut(input);
            }

            var targetPos = target.transform.position.XY();
            var playerPos = Gun.position.XY();
            var aimPoint = CalculateAimPoint(targetPos, playerPos);
            var multiplier = CalculatePlayerAimToTargetMultiplier(targetPos, aimPoint);

            return input * multiplier;
        }

        /// <summary>
        /// Overload for the other method, used in cases when there's only one axis for the look input.
        /// </summary>
        /// <param name="input">the precision aim input, which contains the look input</param>
        /// <returns>the modified look input</returns>
        public float AssistAim(float input)
        {
            return AssistAim(new Vector2(input, 0)).x;
        }

        private Vector2 CalculateAimPoint(Vector2 targetPos, Vector2 playerPos)
        {
            var playerToTarget = targetPos - playerPos;
            var aimDistance = playerToTarget.magnitude * Mathf.Cos(Vector2.Angle(AimDirection, playerToTarget) * Mathf.Deg2Rad);
            return AimDirection.normalized * aimDistance + playerPos;
        }

        private float CalculatePlayerAimToTargetMultiplier(Vector2 targetPos, Vector2 aimPoint)
        {
            return Mathf.Lerp(sensitivityMultiplierAtCenter, 1, Vector2.Distance(targetPos, aimPoint) / AimAssistRadius);
        }

        private Vector2 LerpEaseOut(Vector2 input)
        {
            timeAccumulator = Mathf.Min(timeAccumulator + Time.deltaTime, timeToRegainOriginalInputSensitivity);
            return Mathf.Lerp(sensitivityMultiplierAtEdge, 1, timeAccumulator / timeToRegainOriginalInputSensitivity) * input;
        }

        private void SubscribeToTargetSelectorEvents()
        {
            OnTargetLost.AddListener(ResetEaseOut);
        }

        private void SetUpTimeAccumulator()
        {
            timeAccumulator = timeToRegainOriginalInputSensitivity;
        }

        private void ResetEaseOut(AimAssistTarget target)
        {
            timeAccumulator = 0f;
        }

        private void TearDownTargetSelectorEvents()
        {
            if (OnTargetLost != null)
            {
                OnTargetLost.RemoveListener(ResetEaseOut);
            }
        }
    }
}