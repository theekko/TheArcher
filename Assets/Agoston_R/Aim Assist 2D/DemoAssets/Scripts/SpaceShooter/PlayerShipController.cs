using Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.AimAssists;
using Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.Helper.Numerics;
using Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.TargetSelection;
using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.TagManagement;
using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.InputHandling;
using System.Collections;
using UnityEngine;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.SpaceShooter
{
    public class PlayerShipController : MonoBehaviour, IDestroyable
    {
        private const float MovementInputThreshold = 0.1f;
        private const float LookInputThreshold = 0.1f;

        [Header("Movement")]
        public float ConfinerPadding = 0.2f;
        public float movementSpeed = 1f;

        [Header("Weapons")]
        public float aimSensitivity = 32f;
        public float aimAngleLimit = 45f;
        public float shootCooldown = 0.5f;
        public float bulletVelocity = 10f;

        [Header("Dependencies")]
        public LayerMask confiner;
        public ShipRocket bullet;
        public Transform bulletSpawnLocation;

        [Header("Afterburner")]
        public float afterBurnerSpeedMultiplier = 2f;
        public float afterBurnerDuration = 1f;
        public float afterBurnerCooldown = 5f;

        private readonly AngleLimiter angleLimiter = new AngleLimiter();

        private SpriteRenderer spriteRenderer;

        private Magnetism magnetism;
        private AimLock aimLock;
        private PrecisionAim precisionAim;
        private TargetSelector targetSelector;

        private SpaceShipGame gameController;

        private bool isFireEnabled = true;
        private bool isAfterBurnerEnabled = true;
        private bool isAfterBurnerActivated = false;

        public Color Color => spriteRenderer.color;

        public Vector3 Position => transform.position;

        public GameObject GameObject => gameObject;

        private readonly IPlayerInputHandler inputHandler = InputHandler.Instance;

        private void Awake()
        {
            magnetism = GetComponent<Magnetism>();
            aimLock = GetComponent<AimLock>();
            precisionAim = GetComponent<PrecisionAim>();
            targetSelector = GetComponent<TargetSelector>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            gameController = FindObjectOfType<SpaceShipGame>();
        }

        private void Start()
        {
            CheckBulletPrefab();
            CheckBulletSpawnLocation();
        }

        void Update()
        {
            HandleMovement();
            HandleLook();
            AssistAim();
            HandleShoot();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var tag = collision.gameObject.GetComponent<GameTag>();

            if (tag == null || !tag.CompareGameTag("Enemy"))
            {
                return;
            }

            gameController.EndGame();
        }

        private void HandleShoot()
        {
            var fire = inputHandler.Fire();
            if (fire && isFireEnabled)
            {
                Shoot();
                StartCoroutine(PutShootOnCooldown());
            }
        }

        private void Shoot()
        {
            var projectile = Instantiate(bullet, bulletSpawnLocation.position, Quaternion.identity);
            projectile.Shoot(targetSelector.AimDirection, bulletVelocity);
        }

        private IEnumerator PutShootOnCooldown()
        {
            isFireEnabled = false;
            yield return new WaitForSeconds(shootCooldown);
            isFireEnabled = true;
        }

        private void AssistAim()
        {
            var magnetismResult = magnetism.AssistAim();
            transform.Rotate(magnetismResult.RotationAddition);

            var aimLockResult = aimLock.AssistAim();
            transform.Rotate(aimLockResult.RotationAddition);
        }

        private void HandleLook()
        {
            var look = ReadLookInput();
            if (Mathf.Abs(look) > LookInputThreshold)
            {
                Aim(precisionAim.AssistAim(look));
            }
        }

        private void Aim(float look)
        {
            var rotationAddition = -look * Time.deltaTime * aimSensitivity;

            if (angleLimiter.IsRotationOutsideLimit(rotationAddition, aimAngleLimit, transform.eulerAngles.z))
            {
                return;
            }

            transform.Rotate(rotationAddition * Vector3.forward);
        }

        private float ReadLookInput()
        {
            return inputHandler.AimHorizontal();
        }

        private void HandleMovement()
        {
            var isAfterburnerActivated = ReadAfterBurner();
            var afterburner = CalculateAfterBurnerSpeedMultiplier(isAfterburnerActivated);
            var move = ReadMovementInput();
            if (move.magnitude > MovementInputThreshold && !IsOutsideConfiner(move))
            {
                Move(move * afterburner);
            }
        }

        private float CalculateAfterBurnerSpeedMultiplier(bool isButtonPressed)
        {
            var afterburner = 1f;
            if (isAfterBurnerEnabled && isButtonPressed)
            {
                afterburner = afterBurnerSpeedMultiplier;
                StartCoroutine(PutAfterBurnerOnCooldown());
            }

            return afterburner;
        }

        private IEnumerator PutAfterBurnerOnCooldown()
        {
            yield return new WaitForSeconds(afterBurnerDuration);
            isAfterBurnerEnabled = false;
            isAfterBurnerActivated = false;
            yield return new WaitForSeconds(afterBurnerCooldown);
            isAfterBurnerEnabled = true;
        }

        private bool ReadAfterBurner()
        {
            if (inputHandler.Jump() && isAfterBurnerEnabled)
            {
                isAfterBurnerActivated = true;
            }
            return isAfterBurnerActivated;
        }

        private bool IsOutsideConfiner(Vector2 input)
        {
            var hit = Physics2D.CircleCast(transform.position, ConfinerPadding, Vector2.up, 0f, confiner);
            return hit.collider != null && Vector2.Angle((hit.point - transform.position.XY()), input) < 100f;
        }

        private void Move(Vector2 input)
        {
            transform.position += (input * Time.deltaTime * movementSpeed).XYZ();
        }

        private Vector2 ReadMovementInput()
        {
            return new Vector2(inputHandler.MoveHorizontal(), inputHandler.MoveVertical());
        }

        private void CheckBulletPrefab()
        {
            if (bullet)
            {
                return;
            }

            throw new MissingComponentException("Ship rocket prefab needs to be set from inspector.");
        }

        private void CheckBulletSpawnLocation()
        {
            if (bulletSpawnLocation)
            {
                return;
            }

            throw new MissingComponentException("Bullet spawn location needs to be set from inspector.");
        }
    }

}
