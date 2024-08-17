using Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.Helper.Numerics;
using Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.Model;
using UnityEngine;

namespace Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.AimAssists
{
    /// <summary>
    /// Smoothly rotates the player's aim towards the target. A curve is available to smooth out the rotation and allow some wiggle room.
    /// </summary>
    public class AimLock : AimAssistBase
    {
        private const float UnderAimMultiplier = 0.85f;

        [Tooltip("How much time it should take to get from the edge of the aim assist to the center of the target, on vertical axis.")]
        public float timeToAim = 0.8f;

        [Header("Smooth aimlock")]
        [Tooltip("Enables or disables the angular velocity dampening curve.")]
        public bool enableAngularVelocityCurve = true;

        [Tooltip("Angular velocity curve to multiply the aim assist with. Values closer to 0 refer to the crosshair being close to the target, e.g. looking at its center.")]
        public AnimationCurve angularVelocityCurve;

        [Tooltip("The angle limit in degrees that allows for rotating the player's gun. Goes for negative and positive directions.")]
        [Min(1)]
        public float angleLimit = 89f;

        private readonly AngleLimiter angleLimiter = new AngleLimiter();

        /// <summary>
        /// Smoothly snaps aim to the target's position, at its center.
        /// 
        /// Returns the needed adjustment in degrees for the rotation.
        /// This adjustment is an addition - you need to add it to your Z axis rotations.
        /// </summary>
        public AimAssistResult AssistAim()
        {
            var deltaTime = Time.deltaTime;

            if (!aimAssistEnabled)
            {
                return AimAssistResult.Empty;
            }

            var target = Target;

            if (!target)
            {
                return AimAssistResult.Empty;
            }

            var targetPos = target.transform.position;
            var totalVerticalRotationAngles = CalculateTotalRotationAngles(Gun.forward, targetPos);
            var dx = CalculateDeltaRotationDegrees(totalVerticalRotationAngles, timeToAim, deltaTime, targetPos);

            if (enableAngularVelocityCurve)
            {
                var aimPoint = CalculateAimPoint(targetPos, Gun.position);
                dx *= SampleCurve(targetPos, aimPoint);
            }

            var isOutOfAngleLimit = angleLimiter.IsRotationOutsideLimit(dx, angleLimit, Gun.eulerAngles.z);

            return !isOutOfAngleLimit ? new AimAssistResult(rotationAdditionInDegrees: dx.Sanitized()) : AimAssistResult.Empty;
        }

        private Vector2 CalculateAimPoint(Vector2 targetPos, Vector2 playerPos)
        {
            var playerToTarget = targetPos - playerPos;
            return AimDirection * playerToTarget.magnitude + playerPos;
        }

        private float CalculateDeltaRotationDegrees(float totalRotation, float timeToAim, float deltaTime, Vector3 target)
        {
            var adjustedTimeToAim = timeToAim * AimAssistRadius;
            var distance = (target - Gun.transform.position).magnitude;
            var angularVelocity = Mathf.Atan2(1f, distance) * Mathf.Rad2Deg / adjustedTimeToAim;
            return Mathf.Min(angularVelocity * deltaTime, Mathf.Abs(totalRotation) * UnderAimMultiplier) * Mathf.Sign(totalRotation);
        }

        private float SampleCurve(Vector2 targetPos, Vector2 aimPoint)
        {
            var sample = (targetPos - aimPoint).magnitude / AimAssistRadius;
            return angularVelocityCurve.Evaluate(sample);
        }

        private float CalculateTotalRotationAngles(Vector3 planeNormal, Vector3 target)
        {
            var camForwardProjected = Vector3.ProjectOnPlane(AimDirection, planeNormal);
            var playerToTargetProjected = Vector3.ProjectOnPlane((target - Gun.position).normalized, planeNormal);
            return Vector3.SignedAngle(camForwardProjected, playerToTargetProjected, planeNormal);
        }
    }
}