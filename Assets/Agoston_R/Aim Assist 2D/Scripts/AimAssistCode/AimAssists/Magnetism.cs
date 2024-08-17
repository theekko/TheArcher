using Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.Helper.Info;
using Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.Helper.Numerics;
using Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.Model;
using UnityEngine;

namespace Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.AimAssists
{
    /// <summary>
    /// Compensates for the player's movement by loosely following the target while it's still in assist range.
    /// </summary>
    [RequireComponent(typeof(PlayerTransformInfo))]
    public class Magnetism : AimAssistBase
    {
        [Header("Strafing compensation")] [Tooltip("A divisor for the player's strafe movement when they are moving away from the target.")] [Min(1.08f)]
        public float smoothnessAwayFromTarget = 1.09f;

        [Tooltip(
            "A divisor for the player's strafe movement when they are strafing towards the target. To prevent turning the player away from the target during mirror strafing, it has to be greater than smoothness away from target.")]
        [Min(1.08f)]
        public float smoothnessTowardsTarget = 2f;

        [Tooltip("The angle limit in degrees that allows for rotating the player's gun. Goes for negative and positive directions.")]
        [Min(1)]
        public float angleLimit = 89f;

        private PlayerTransformInfo playerPhysics;

        private readonly AngleLimiter angleLimiter = new AngleLimiter();

        private void Start()
        {
            SetUpPlayerPhysicsInfo();
        }

        /// <summary>
        /// Calculates the aim assist. 
        /// 
        /// The result is a rotation addition in degrees, that you'll have to add to your Z axis rotations. 
        /// </summary>
        /// <returns>the aim assist result</returns>
        public AimAssistResult AssistAim()
        {
            var target = Target;
            if (!target || smoothnessAwayFromTarget < 1f || !aimAssistEnabled)
            {
                return AimAssistResult.Empty;
            }

            var targetPos = target.transform.position.XY();
            var playerPos = Gun.position.XY();
            var velocity = playerPhysics.Velocity.XY();
            var aimPoint = CalculateAimPoint(targetPos, playerPos);
            var moveTowardsTarget = IsPlayerMovingTowardsTarget(aimPoint, targetPos, velocity);
            var smoothness = SelectSmoothness(moveTowardsTarget);
            var aimAdjustment = (!IsNoMovement() ? CalculateSignedAssistAngle(targetPos) / smoothness : 0f).Sanitized();

            return !angleLimiter.IsRotationOutsideLimit(aimAdjustment, angleLimit, Gun.eulerAngles.z) ? new AimAssistResult(rotationAdditionInDegrees: aimAdjustment) : AimAssistResult.Empty;
        }

        private Vector2 CalculateAimPoint(Vector2 targetPos, Vector2 playerPos)
        {
            var playerToTarget = targetPos - playerPos;
            var aimDistance = playerToTarget.magnitude * Mathf.Cos(Vector2.Angle(AimDirection, playerToTarget) * Mathf.Deg2Rad);
            return AimDirection.normalized * aimDistance + playerPos;
        }

        private float CalculateSignedAssistAngle(Vector2 targetPos)
        {
            var gunPos = Gun.transform.position.XY();
            var playerToTarget = targetPos - gunPos;
            var velocity = playerPhysics.Velocity.XY();
            var playerTranslated = gunPos + velocity * Time.deltaTime;
            var playerTranslatedToTarget = targetPos - playerTranslated;
            var targetTranslated = targetPos + velocity * Time.deltaTime;
            var playerToTargetTranslated = targetTranslated - gunPos;
            var sign = Mathf.Sign(Vector2.SignedAngle(playerToTargetTranslated, playerToTarget)) * Mathf.Sign(Gun.transform.forward.z);
            return Vector2.Angle(playerToTarget, playerTranslatedToTarget) * sign;
        }

        private float SelectSmoothness(bool moveTowardsTarget)
        {
            return moveTowardsTarget ? smoothnessTowardsTarget : smoothnessAwayFromTarget;
        }

        private bool IsNoMovement()
        {
            return playerPhysics.Velocity.XY().EqualsApprox(Vector2.zero);
        }

        private bool IsPlayerMovingTowardsTarget(Vector2 aimPoint, Vector2 targetPos, Vector2 velocity)
        {
            var aimToTarget = targetPos - aimPoint;
            return Mathf.Sign(Vector2.Dot(velocity, aimToTarget)) > 0;
        }

        private void SetUpPlayerPhysicsInfo()
        {
            playerPhysics = GetComponent<PlayerTransformInfo>();
            playerPhysics.PlayerTransform = transform;
        }
    }
}