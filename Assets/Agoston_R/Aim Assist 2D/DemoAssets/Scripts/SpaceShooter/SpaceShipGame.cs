using Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.AimAssists;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.SpaceShooter
{
    public class SpaceShipGame : MonoBehaviour, IInvaderDestroyer
    {
        private const string MagnetismButtonName = "Magnetism";
        private const string AimLockButtonName = "AimLock";
        private const string PrecisionAimButtonName = "PrecisionAim";

        public delegate void GameEnded();
        public event GameEnded OnGameEnded;

        public DestroyParticleController destroyParticles;

        private PlayerShipController player;

        private Magnetism magnetism;
        private AimLock aimLock;
        private PrecisionAim precisionAim;

        private readonly Dictionary<string, AimAssistBase> buttonNameToAimAssistMap = new Dictionary<string, AimAssistBase>();

        private void Start()
        {
            CheckDestroyParticles();
            FindPlayer();
            FindAimAssists();
            SetUpButtonToAimAssistMapping();
            SetUpEdgeColliderPositions();
        }

        private void SetUpEdgeColliderPositions()
        {
            var leftEdge = GameObject.Find("LeftEdge").transform;
            var rightEdge = GameObject.Find("RightEdge").transform;
            var cam = Camera.main;

            leftEdge.position = cam.ScreenToWorldPoint(new Vector3(0, Screen.height / 2, 0));
            rightEdge.position = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2, 0));
        }

        private void FindAimAssists()
        {
            magnetism = FindObjectOfType<Magnetism>();
            precisionAim = FindObjectOfType<PrecisionAim>();
            aimLock = FindObjectOfType<AimLock>();
        }

        private void FindPlayer()
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerShipController>();
        }

        private void CheckDestroyParticles()
        {
            if (destroyParticles)
            {
                return;
            }

            throw new MissingComponentException("Destroy particles need to be set from inspector.");
        }

        public void SwitchAimAssist(string buttonName)
        {
            var aimAssist = buttonNameToAimAssistMap[buttonName];
            aimAssist.aimAssistEnabled = !aimAssist.aimAssistEnabled;
        }

        public bool GetAimAssistActivationState(string aimAssistButtonName)
        {
            return buttonNameToAimAssistMap[aimAssistButtonName].aimAssistEnabled;
        }

        public void Restart()
        {
            Time.timeScale = 1;
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        public void EndGame()
        {
            if (!player)
            {
                return;
            }

            Destroy(player.gameObject);
            PlayExplosionParticles(player);
            OnGameEnded?.Invoke();
        }

        public void DestroyObject(IDestroyable destroyable)
        {
            Destroy(destroyable.GameObject);
            PlayExplosionParticles(destroyable);
        }

        private void PlayExplosionParticles(IDestroyable destroyable)
        {
            if (destroyable == null)
            {
                return;
            }

            var particles = Instantiate(destroyParticles);
            particles.PlayThenDestroy(destroyable.Position, destroyable.Color);
        }

        private void SetUpButtonToAimAssistMapping()
        {
            buttonNameToAimAssistMap.Add(MagnetismButtonName, magnetism);
            buttonNameToAimAssistMap.Add(AimLockButtonName, aimLock);
            buttonNameToAimAssistMap.Add(PrecisionAimButtonName, precisionAim);
        }
    }

}
