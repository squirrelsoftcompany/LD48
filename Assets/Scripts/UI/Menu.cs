using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class Menu : MonoBehaviour {
        [SerializeField] private Image image;
        [SerializeField] private Sprite playImage, pauseImage;
        [SerializeField] private GameObject winScreen, loseScreen, pauseScreen, startScreen;
        private bool isRunning;
        [SerializeField] private GameObject controls;

        private void Start() {
            isRunning = false;
            Time.timeScale = 0;
        }

        public void PlayGame() {
            isRunning = true;
            startScreen.SetActive(false);
            Time.timeScale = 1f;
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
            pauseScreen.SetActive(true);
        }

        private void resume() {
            Time.timeScale = 1;
            isRunning = true;
            image.sprite = pauseImage;
            pauseScreen.SetActive(false);
        }

        public void lose() {
            loseScreen.SetActive(true);
            Time.timeScale = 0;
            isRunning = false;
        }

        public void win() {
            winScreen.SetActive(true);
        }

        public void displayControls() {
            controls.SetActive(true);
        }
    }
}