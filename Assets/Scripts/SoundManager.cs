using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    [SerializeField] private AudioSource audioSourceBubbles;
    [SerializeField] private AudioSource audioSourceAmbient;
    [SerializeField] private AudioSource audioSourceAcceleration;
    [SerializeField] private AudioSource audioSourceSonar;

    private bool shouldPlayBubbles;
    [CanBeNull] private IEnumerator bubbleLoop;
    [CanBeNull] private IEnumerator fadeOutCoroutine, fadeInCoroutine;

    // Start is called before the first frame update
    private void Start() {
        shouldPlayBubbles = false;
    }

    public void playBubbles(bool start) {
        shouldPlayBubbles = start;
        if (bubbleLoop != null) {
            StopCoroutine(bubbleLoop);
            bubbleLoop = null;
        }

        if (!shouldPlayBubbles) return;
        bubbleLoop = loopDistantBubbles(3);
        StartCoroutine(bubbleLoop);
    }

    public void playSonar() {
        audioSourceSonar.Play();
    }

    private IEnumerator loopDistantBubbles(float intervalSeconds) {
        while (shouldPlayBubbles) {
            yield return new WaitUntil(() => !audioSourceBubbles.isPlaying);
            audioSourceBubbles.Play();
            yield return new WaitForSeconds(intervalSeconds);
        }
    }
    
    public void playAcceleration(bool start) {
        if (start) {
            fadeAmbientToAcceleration();
        } else {
            fadeAccelerationToAmbient();
        }
    }

    private void fadeAccelerationToAmbient() {
        killAllCoroutines();
        fadeOutCoroutine = fadeOut(audioSourceAcceleration, 0.5f, 0.2f);
        fadeInCoroutine = fadeIn(audioSourceAmbient, 0.5f, 0.9f);
        StartCoroutine(fadeOutCoroutine);
        StartCoroutine(fadeInCoroutine);
    }

    private void fadeAmbientToAcceleration() {
        killAllCoroutines();
        fadeOutCoroutine = fadeOut(audioSourceAmbient, 0.5f, 0.2f);
        fadeInCoroutine = fadeIn(audioSourceAcceleration, 0.5f, 0.9f);
        StartCoroutine(fadeOutCoroutine);
        StartCoroutine(fadeInCoroutine);
    }

    private void killAllCoroutines() {
        if (fadeOutCoroutine != null) StopCoroutine(fadeOutCoroutine);
        if (fadeInCoroutine != null) StopCoroutine(fadeInCoroutine);
    }

    private static IEnumerator fadeOut(AudioSource audioSource, float fadeTime, float threshold) {
        while (audioSource.volume > threshold) {
            audioSource.volume -= Time.deltaTime / fadeTime;
            yield return null;
        }
    }

    private static IEnumerator fadeIn(AudioSource audioSource, float fadeTime, float threshold) {
        while (audioSource.volume < threshold) {
            audioSource.volume += Time.deltaTime / fadeTime;
            yield return null;
        }
    }
}