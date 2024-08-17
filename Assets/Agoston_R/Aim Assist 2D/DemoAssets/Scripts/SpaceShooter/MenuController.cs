using Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.InputHandling;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Agoston_R.Aim_Assist_2D.DemoAssets.Scripts.SpaceShooter
{
    public class MenuController : MonoBehaviour
    {
        private const string UnpausedCanvas = "InGameCanvas";
        private const string DeathMenu = "DeathMenu";

        public GameObject[] uiCanvases;
        public float waitForEndGameCanvas = 1;

        private int index = 0;
        private SpaceShipGame gameController;

        public Button[] aimAssistButtons;

        public Color selectedAssistButtonColor;
        public Color deselectedAssistButtonColor;

        private bool isGameEnded = false;

        private readonly IPlayerInputHandler inputHandler = InputHandler.Instance;

        private void Awake()
        {
            gameController = FindObjectOfType<SpaceShipGame>();
            gameController.OnGameEnded += OnGameEnded;
        }

        private void Start()
        {
            foreach (var b in aimAssistButtons)
            {
                b.onClick.AddListener(delegate { SelectAimAssist(b.name); });
            }
        }

        private void Update()
        {
            if (!isGameEnded && inputHandler.Menu())
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

        private void OnGameEnded()
        {
            StartCoroutine(ShowEndGameCanvas());
        }

        private IEnumerator ShowEndGameCanvas()
        {
            yield return new WaitForSecondsRealtime(waitForEndGameCanvas);
            isGameEnded = true;
            SelectCanvas(uiCanvases.ToList().FindIndex(x => x.name == DeathMenu));
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
