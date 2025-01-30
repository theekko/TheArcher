using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MusicPlayer : MonoBehaviour {
    [SerializeField] private AudioSource introSource; 
    [SerializeField] private AudioSource level1Source; 
    [SerializeField] private float fadeDuration = 2.0f; 
    [SerializeField] private float level1SourceStartTime = 0f; 
    [SerializeField] private float volume = 0.5f;

    private string[] scenesToKeepMusic = { SceneStrings.MainMenu, SceneStrings.IntroMovie, SceneStrings.Level1, SceneStrings.LevelEndWinMovie, SceneStrings.LevelEndLoseMovie, SceneStrings.LevelEndSecretWinMovie };
    private static MusicPlayer instance;
    private bool isFading = false;
    private bool hasSwitchedToLevel1Music = false;
    private float introSourceSavedTime = 0f; // To resume first track from where it left off

    private void Awake() {
        // Ensure only one MusicPlayer exists
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        // Set this as the instance and don't destroy on load
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        // Play the intro audio and make sure level1Source is not playing
        introSource.Play();
        level1Source.Stop();

        // Subscribe to sceneLoaded event to check when the scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // Check if the current scene is in the list of scenes where music should persist
        bool shouldKeepMusic = false;
        foreach (string sceneName in scenesToKeepMusic) {
            if (scene.name == sceneName) {
                shouldKeepMusic = true;
                break;
            }
        }

        // If the current scene is not in the list, destroy the MusicPlayer
        if (!shouldKeepMusic) {
            Destroy(gameObject);
        }

        // If the current scene is "Main Menu", restart the introSource music from the beginning
        if (scene.name == SceneStrings.MainMenu) {
            introSource.time = 0; // Restart from the beginning
            introSource.Play();   // Play the music
        }

        // If we are in "Level1", start monitoring for player input
        if (scene.name == "Level1" && !hasSwitchedToLevel1Music) {
            StartCoroutine(CheckForInputInLevel1());
        }
        // If we're leaving "Level1" and the level1Source is playing, switch back to the first track
        else if (scene.name != "Level1" && hasSwitchedToLevel1Music) {
            StartCoroutine(FadeToTrack(level1Source, introSource, fadeDuration, 0f));
            hasSwitchedToLevel1Music = false;
        }
    }

    // Coroutine to check for any player input in Level1
    private IEnumerator CheckForInputInLevel1() {
        while (SceneManager.GetActiveScene().name == "Level1") {
            // If any input is detected and the music hasn't switched yet
            if (Input.anyKeyDown && !hasSwitchedToLevel1Music && !isFading) {
                hasSwitchedToLevel1Music = true;
                StartCoroutine(FadeToTrack(introSource, level1Source, fadeDuration, level1SourceStartTime));
            }
            yield return null;
        }
    }

    // Coroutine to fade between two audio sources
    private IEnumerator FadeToTrack(AudioSource fromAudio, AudioSource toAudio, float fadeDuration, float startTime = 0f, bool resumeIntro = false) {
        isFading = true;
        float currentTime = 0f;
        float fromVolume = volume;
        float toVolume = volume;
        Debug.Log("fromVolume: " + fromVolume);
        Debug.Log("toVolume: " + toVolume);
        // Set the loop property for the level1Source when switching to it
        if (toAudio == level1Source) {
            toAudio.loop = true; // Enable looping for level1Source
        }
        toAudio.time = startTime;
        toAudio.Play(); // Start playing the new track
        
        // Fade out the current track and fade in the new one
        while (currentTime < fadeDuration) {
            currentTime += Time.deltaTime;
            fromAudio.volume = Mathf.Lerp(fromVolume, 0, currentTime / fadeDuration); // Fade out the old track
            Debug.Log("fromAudio: "  + fromAudio.volume);
            toAudio.volume = Mathf.Lerp(0, toVolume, currentTime / fadeDuration); // Fade in the new track
            Debug.Log("toAudio: " + toAudio.volume);
            yield return null;
        }


        fromAudio.Stop(); // Stop the old track completely after fading out
        isFading = false;
    }

    private void OnDestroy() {
        // Unsubscribe from the event to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
