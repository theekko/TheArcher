using UnityEngine;

namespace Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.TargetSelection
{
    /// <summary>
    /// Debug drawer that shows what the target selector sees in the Scene view.
    /// </summary>
    [RequireComponent(typeof(TargetSelector))]
    [RequireComponent(typeof(LineRenderer))]
    [ExecuteInEditMode]
    public class TargetSelectorDebugDrawer : MonoBehaviour
    {
        private LineRenderer lineRenderer;
        private TargetSelector targetSelector;
        private ForwardDirectionCalculator directionCalculator = new ForwardDirectionCalculator();

        private void OnEnable()
        {
            targetSelector = GetComponent<TargetSelector>();
            lineRenderer = GetComponent<LineRenderer>();
        }

#if UNITY_EDITOR        
        private void OnDrawGizmos()
        {
            if (!targetSelector || !targetSelector.AimOrigin)
            {
                return;
            }

            var aimDrawerData = CalculateAimDrawerData();

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(aimDrawerData.StartPoint, targetSelector.aimAssistRadius);
            Gizmos.DrawWireSphere(aimDrawerData.EndPoint, targetSelector.aimAssistRadius);
            Gizmos.DrawLine(aimDrawerData.StartPoint - targetSelector.AimOrigin.right * targetSelector.aimAssistRadius, aimDrawerData.EndPoint - targetSelector.AimOrigin.right * targetSelector.aimAssistRadius);
            Gizmos.DrawLine(aimDrawerData.StartPoint + targetSelector.AimOrigin.right * targetSelector.aimAssistRadius, aimDrawerData.EndPoint + targetSelector.AimOrigin.right * targetSelector.aimAssistRadius);
            Gizmos.DrawLine(aimDrawerData.StartPoint - targetSelector.AimOrigin.up * targetSelector.aimAssistRadius, aimDrawerData.EndPoint - targetSelector.AimOrigin.up * targetSelector.aimAssistRadius);
            Gizmos.DrawLine(aimDrawerData.StartPoint + targetSelector.AimOrigin.up * targetSelector.aimAssistRadius, aimDrawerData.EndPoint + targetSelector.AimOrigin.up * targetSelector.aimAssistRadius);
        }
#endif
        private void Update()
        {
            var aimDrawerData = CalculateAimDrawerData();
            lineRenderer?.SetPositions(new Vector3[] { aimDrawerData.StartPoint, aimDrawerData.EndPoint });
        }

        private AimDrawerDto CalculateAimDrawerData()
        {
            var direction = targetSelector.AimOrigin.TransformVector(directionCalculator.CalculateForwardLocalSpace(targetSelector.forwardDirection, targetSelector.flip)).normalized;
            var startCenter = targetSelector.AimOrigin.position + direction * targetSelector.nearClipDistance;
            var endCenter = targetSelector.AimOrigin.position + direction * targetSelector.farClipDistance;
            return new AimDrawerDto(startCenter, endCenter);
        }

        internal struct AimDrawerDto
        {
            internal Vector3 StartPoint { get; private set; }
            internal Vector3 EndPoint { get; private set; }

            public AimDrawerDto(Vector3 startPoint, Vector3 endPoint)
            {
                this.StartPoint = startPoint;
                this.EndPoint = endPoint;
            }
        }
    }
}