using Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.AimAssists;
using Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.TargetSelection;
using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.InputHandling;
using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Mechanics;
using System.Collections;
using UnityEngine;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Weapon
{
    public class WeaponControls : MonoBehaviour
    {
        public float rateOfFire = 1;
        public float projectileSpeed = 10f;
        public float aimAngleLimit = 89f;

        [SerializeField]
        private Bullet bullet;

        [SerializeField]
        private float aimSensitivity = 1f;

        private PlayerController playerController;
        private TargetSelector targetSelector;
        private bool isShootEnabled = true;
        private Transform bulletSpawnLocator;
        private float currentLookDirection = 1;
        private Magnetism magnetism;
        private PrecisionAim precisionAim;
        private AimLock aimLock;

        private readonly IPlayerInputHandler inputHandler = InputHandler.Instance;

        private void Awake()
        {
            SetUpPlayerController();
            CheckBulletPrefabNotNull();
            SetUpBulletLocator();
            SetUpTargetSelector();
            // SubscribeToTargetSelectionEvents();
            SetUpAimAssist();
        }

        private void SetUpAimAssist()
        {
            magnetism = GetComponent<Magnetism>();
            precisionAim = GetComponent<PrecisionAim>();
            aimLock = GetComponent<AimLock>();
        }

        private void OnDestroy()
        {
            targetSelector.OnTargetSelected.RemoveAllListeners();
            targetSelector.OnTargetLost.RemoveAllListeners();
        }

        private void Update()
        {
            if (!playerController.controlEnabled)
            {
                return;
            }

            if (inputHandler.Fire() && isShootEnabled)
            {
                Shoot();
                StartCoroutine(CooldownWeapon());
            }

            var horizontalInput = inputHandler.AimHorizontal();

            if (Mathf.Abs(horizontalInput) > 0.6f)
            {
                OnLookDirectionChanged(Mathf.Sign(horizontalInput));
                playerController.Flip(horizontalInput < 0);
            }

            var rawInput = inputHandler.AimVertical();
            var assistedInput = precisionAim.AssistAim(rawInput);

            if (Mathf.Abs(assistedInput) > 0.05f)
            {
                Aim(assistedInput);
            }

            UseMagnetism();
            UseAimLock();
        }

        private void UseAimLock()
        {
            var result = aimLock.AssistAim();
            transform.Rotate(result.RotationAddition);
        }

        private void UseMagnetism()
        {
            var result = magnetism.AssistAim();
            transform.Rotate(result.RotationAddition);
        }

        public void ResetAimAngle()
        {
            transform.rotation = Quaternion.identity;
        }

        private void Aim(float aimInput)
        {
            var angle = aimInput * aimSensitivity * Time.deltaTime;
            var orientation = transform.localEulerAngles;
            transform.Rotate(Vector3.forward * angle);
            ClampAimRotation(aimInput, orientation);
        }

        private void ClampAimRotation(float aimInput, Vector3 orientation)
        {
            var sinZAngle = Mathf.Sin(transform.localEulerAngles.z * Mathf.Deg2Rad);
            var sinLimit = Mathf.Sin(aimAngleLimit * Mathf.Deg2Rad);
            if (sinZAngle > sinLimit && aimInput > 0 ||
                sinZAngle < -sinLimit && aimInput < 0)
            {
                transform.localEulerAngles = orientation;
            }
        }

        public void OnLookDirectionChanged(float newLookDirection)
        {
            if (Mathf.Sign(newLookDirection) == Mathf.Sign(currentLookDirection))
            {
                return;
            }

            transform.RotateAround(playerController.transform.position, Vector3.down, 180);
            currentLookDirection = newLookDirection;
        }

        private void SetUpBulletLocator()
        {
            bulletSpawnLocator = transform.Find("BulletLocator");
        }

        private void SetUpPlayerController()
        {
            playerController = GetComponentInParent<PlayerController>();
        }

        private void Shoot()
        {
            var projectile = Instantiate(bullet);
            projectile.transform.position = bulletSpawnLocator.position;
            projectile.Shoot(magnetism.AimDirection, projectileSpeed);
        }

        private void SetUpTargetSelector()
        {
            targetSelector = GetComponent<TargetSelector>();
        }

        private void SubscribeToTargetSelectionEvents()
        {
            targetSelector.OnTargetSelected.AddListener((t) => Debug.Log($"Found target: {t.name}"));
            targetSelector.OnTargetLost.AddListener((t) => Debug.Log($"Lost target: {t.name}"));
        }

        private IEnumerator CooldownWeapon()
        {
            isShootEnabled = false;
            yield return new WaitForSeconds(1 / rateOfFire);
            isShootEnabled = true;
        }

        private void CheckBulletPrefabNotNull()
        {
            if (!bullet)
            {
                throw new MissingComponentException("Bullet prefab reference is missing.");
            }
        }
    }

}
