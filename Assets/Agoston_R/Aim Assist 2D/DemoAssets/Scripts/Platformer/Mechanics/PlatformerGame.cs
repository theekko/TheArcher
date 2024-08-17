using Agoston_R.Aim_Assist_2D.Scripts.AimAssistCode.AimAssists;
using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Core;
using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Model;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Mechanics
{
    /// <summary>
    /// This class exposes the the game model in the inspector, and ticks the
    /// simulation.
    /// </summary> 
    public class PlatformerGame : MonoBehaviour
    {
        private const string MagnetismButtonName = "Magnetism";
        private const string AimLockButtonName = "AimLock";
        private const string PrecisionAimButtonName = "PrecisionAim";

        private readonly Dictionary<string, AimAssistBase> buttonNameToAimAssistMap = new Dictionary<string, AimAssistBase>();

        public static PlatformerGame Instance { get; private set; }

        public delegate void GameEnded();
        public event GameEnded OnGameEnded;

        private Magnetism magnetism;
        private AimLock aimLock;
        private PrecisionAim precisionAim;

        public int targetFrameRate = 120;

        //This model field is public and can be therefore be modified in the 
        //inspector.
        //The reference actually comes from the InstanceRegister, and is shared
        //through the simulation and events. Unity will deserialize over this
        //shared reference when the scene loads, allowing the model to be
        //conveniently configured inside the inspector.
        public PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        void OnEnable()
        {
            Instance = this;
            FindAimAssists();
            SetUpButtonToAimAssistMapping();
        }

        private void FindAimAssists()
        {
            magnetism = FindObjectOfType<Magnetism>();
            precisionAim = FindObjectOfType<PrecisionAim>();
            aimLock = FindObjectOfType<AimLock>();
        }

        public void EndGame()
        {
            OnGameEnded?.Invoke();
        }

        private void SetUpButtonToAimAssistMapping()
        {
            buttonNameToAimAssistMap.Add(MagnetismButtonName, magnetism);
            buttonNameToAimAssistMap.Add(AimLockButtonName, aimLock);
            buttonNameToAimAssistMap.Add(PrecisionAimButtonName, precisionAim);
        }

        void OnDisable()
        {
            if (Instance == this) Instance = null;
        }

        void Update()
        {
            if (Instance == this) Simulation.Tick();

            if (Application.targetFrameRate != targetFrameRate)
            {
                Application.targetFrameRate = targetFrameRate;
            }
        }

        public void Restart()
        {
            Time.timeScale = 1;
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        internal void SwitchAimAssist(string buttonName)
        {
            var aimAssist = buttonNameToAimAssistMap[buttonName];
            aimAssist.aimAssistEnabled = !aimAssist.aimAssistEnabled;
        }

        internal bool GetAimAssistActivationState(string buttonName)
        {
            return buttonNameToAimAssistMap[buttonName].aimAssistEnabled;
        }
    }
}