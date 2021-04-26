using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class Menu : MonoBehaviour {
        [SerializeField] private Image image;
        [SerializeField] private Sprite playImage, pauseImage;
        [SerializeField] private GameObject winScreen, loseScreen;
        private bool isRunning;

        private void Start() {
            isRunning = true;
        }

        public void togglePlayPause() {
            if (isRunning) {
                pause();
            } else {
                resume();
            }
        }

        private void pause() {
            Time.timeScale = 0;
            isRunning = false;
            image.sprite = playImage;
        }

        private void resume() {
            Time.timeScale = 1;
            isRunning = true;
            image.sprite = pauseImage;
        }

        public void lose() {
            loseScreen.SetActive(true);
            Time.timeScale = 0;
            isRunning = false;
        }

        public void win() {
            winScreen.SetActive(true);
        }
    }
}