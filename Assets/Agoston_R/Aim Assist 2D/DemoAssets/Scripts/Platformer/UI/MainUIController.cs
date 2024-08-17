using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.InputHandling;
using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.Mechanics;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.Platformer.UI
{
    /// <summary>
    /// A simple controller for switching between UI panels.
    /// </summary>
    public class MainUIController : MonoBehaviour
    {
        private const string UnpausedCanvas = "InGameCanvas";
        private const string VictoryCanvas = "VictoryCanvas";
        
        public Button[] aimAssistButtons;
        public Color selectedAssistButtonColor;
        public Color deselectedAssistButtonColor;

        public float waitForEndGameCanvas = 1;

        public GameObject[] uiCanvases;

        private PlatformerGame gameController;

        private int index = 0;

        private IPlayerInputHandler inputHandler = InputHandler.Instance;

        private void Start()
        {
            gameController = FindObjectOfType<PlatformerGame>();
            gameController.OnGameEnded += OnGameEnded;
            foreach (var b in aimAssistButtons)
            {
                b.onClick.AddListener(delegate { SelectAimAssist(b.name); });
            }
        }

        private void Update()
        {
            if (inputHandler.Menu())
            {
                HandlePauseMenu();
            }
        }

        public void SelectCanvas(int index)
        {
            if (index < 0 || index >= uiCanvases.Length)
            {
                Debug.LogError($"Cannot select canvas of index {index}: out of bounds.");
                return;
            }

            this.index = index;

            for (int i = 0; i < uiCanvases.Length; i++)
            {
                uiCanvases[i].SetActive(false);
            }

            if (!uiCanvases[index].activeSelf)
            {
                uiCanvases[index].SetActive(true);
            }

            SetTimeScale(index);
        }

        private void OnGameEnded()
        {
            StartCoroutine(ShowEndGameCanvas());
        }

        private IEnumerator ShowEndGameCanvas()
        {
            yield return new WaitForSecondsRealtime(waitForEndGameCanvas);
            SelectCanvas(uiCanvases.ToList().FindIndex(x => x.name == VictoryCanvas));
        }

        public void HandlePauseMenu()
        {
            if (index != 0)
            {
                index = 0;
            }
            else
            {
                index = 1;
            }

            SelectCanvas(index);
        }

        private void SelectAimAssist(string buttonName)
        {
            gameController.SwitchAimAssist(buttonName);

            var button = aimAssistButtons.Where(b => buttonName == b.name).First();
            var activationState = gameController.GetAimAssistActivationState(buttonName);

            var img = button.GetComponent<Image>();
            if (activationState)
            {
                img.color = selectedAssistButtonColor;
            }
            else
            {
                img.color = deselectedAssistButtonColor;
            }
        }

        private void SetTimeScale(int index)
        {
            var canvasName = uiCanvases[index].name;
            if (UnpausedCanvas == canvasName)
            {
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;
            }
        }
    }
}