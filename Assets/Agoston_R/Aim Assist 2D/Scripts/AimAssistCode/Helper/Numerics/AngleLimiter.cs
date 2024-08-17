using UnityEngine;

namespace Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.Helper.Numerics
{
    /// <summary>
    /// Limits the angle considering the addition it will receive.
    /// 
    /// Converts degrees over 180 to negatives.
    /// </summary>
    public class AngleLimiter
    {
        /// <summary>
        /// Limit the angle considering the addition.
        /// </summary>
        /// <param name="addition">The addition the angle will receive</param>
        /// <param name="angleLimit">The limit for the angle + addition</param>
        /// <param name="angle">The angle whom we'll increase with the addition</param>
        /// <returns>If the angle + addition is outside of the limit.</returns>
        public bool IsRotationOutsideLimit(float addition, float angleLimit, float angle)
        {
            angle = (angle > 180) ? angle - 360 : angle;
            return Mathf.Abs(angle + addition) > angleLimit;
        }
    }
}
