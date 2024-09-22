using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextAnim : MonoBehaviour {
    [SerializeField] TextMeshProUGUI _textMeshPro;
    [SerializeField] float timeBtwnCharacters;
    [SerializeField] float timeBtwnWords;

    public string[] stringArray;
    int i = 0;

    private void EndCheck() {
        if (i <= stringArray.Length - 1) { 
            _textMeshPro.text = stringArray[i];
            StartCoroutine(TextVisable());
        }
    }
    private IEnumerator TextVisable() {
        _textMeshPro.ForceMeshUpdate();
        int totalVisibleCharacters = _textMeshPro.textInfo.characterCount;
        int counter = 0;

        while (true) { 
            int visibleCount = counter % (totalVisibleCharacters + 1);
            _textMeshPro.maxVisibleCharacters = visibleCount;
            if (visibleCount >= totalVisibleCharacters) {
                i++;
                Invoke("EndCheck", timeBtwnWords);
                break;
            }

            counter++;

            yield return new WaitForSeconds(timeBtwnCharacters);
        }
    }

    private void Start() {
        EndCheck();
    }
}
