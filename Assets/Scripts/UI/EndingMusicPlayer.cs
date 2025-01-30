using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingMusicPlayer : MonoBehaviour {
    [SerializeField] private AudioSource introSource;
    [SerializeField] private double delay;

    private void Start() {
        introSource.Play();
    }
}
