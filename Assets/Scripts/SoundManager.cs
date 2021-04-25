using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    [SerializeField] private AudioSource audioSourceBubbles;
    [SerializeField] private AudioSource audioSourceAmbient;
    [SerializeField] private AudioSource audioSourceAcceleration;
    [SerializeField] private AudioSource audioSourceSonar;
    [SerializeField] private AudioSource audioSourceBump;
    [SerializeField] private AudioSource audioSourceMusic;
    [SerializeField] private AudioClip[] audioClipsMusic;
    [SerializeField] private AudioSource audioSourceSpin;
    [SerializeField] private AudioSource audioSourcePush;
    [SerializeField] private AudioSource audioSourceExplosion;
    private int indexMusic;
    private bool shouldPlayBubbles;
    [CanBeNull] private IEnumerator bubbleLoop;
    [CanBeNull] private IEnumerator fadeOutCoroutine, fadeInCoroutine;

    private bool musicOn;

    // Start is called before the first frame update
    private void Start() {
        indexMusic = 0;
        musicOn = true;
        shouldPlayBubbles = false;
        StartCoroutine(playNextMusic());
    }

    private IEnumerator playNextMusic() {
        while (musicOn) {
            audioSourceMusic.clip = audioClipsMusic[indexMusic];
            audioSourceMusic.Play();
            yield return new WaitForSeconds(audioClipsMusic[indexMusic].length + 2);
            indexMusic = (indexMusic + 1) % audioClipsMusic.Length;
        }
    }

    private void OnEnable() {
        musicOn = true;
    }

    private void OnDisable() {
        musicOn = false;
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

    public void playBump() {
        audioSourceBump.Play();
    }

    public void playCrazy(int crazyType) {
        switch (crazyType) {
            case 1:
                playCrazySpin();
                break;
            case 2:
                playCrazyPush();
                break;
            case 3:
                playCrazyExplosion();
                break;
        }
    }

    private void playCrazySpin() {
        audioSourceSpin.Play();
    }

    private void playCrazyPush() {
        audioSourcePush.Play();
    }

    private void playCrazyExplosion() {
        audioSourceExplosion.Play();
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
        fadeOutCoroutine = fadeOut(audioSourceAcceleration, 0.5f, 0.1f);
        fadeInCoroutine = fadeIn(audioSourceAmbient, 0.5f, 0.5f);
        StartCoroutine(fadeOutCoroutine);
        StartCoroutine(fadeInCoroutine);
    }

    private void fadeAmbientToAcceleration() {
        killAllCoroutines();
        fadeOutCoroutine = fadeOut(audioSourceAmbient, 0.5f, 0.1f);
        fadeInCoroutine = fadeIn(audioSourceAcceleration, 0.5f, 0.5f);
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