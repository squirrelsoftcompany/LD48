using System.Collections;
using JetBrains.Annotations;
using Props;
using UnityEngine;

namespace Player {
    public class Echolocation : MonoBehaviour {
        // We need to drop a fart whenever we use echolocation
        // This will be used for enemies to find you
        [SerializeField] private GameObject fartPrefab;
        [CanBeNull] private GameObject _previousFart;
        [SerializeField] private GameObject goalMarker;
        [SerializeField] private float secondsShowGoal;
        [CanBeNull] private IEnumerator _showCoroutine;

        public void dropFart() {
            var currentPosition = transform.position;
            // No more than 1 fart
            if (_previousFart != null) {
                Destroy(_previousFart);
            }

            _previousFart = Instantiate(fartPrefab, currentPosition, Quaternion.identity);
        }

        public void showGoalLocation() {
            var goal = FindObjectOfType<Goal>();
            if (_showCoroutine != null) {
                StopCoroutine(_showCoroutine);
            }

            _showCoroutine = lookForNSeconds(goal, secondsShowGoal);
            StartCoroutine(_showCoroutine);
        }

        private IEnumerator lookForNSeconds(Component goal, float seconds) {
            var timeStart = Time.time;
            goalMarker.SetActive(true);
            while (Time.time - timeStart < seconds) {
                goalMarker.transform.LookAt(goal.transform);
                yield return new WaitForFixedUpdate();
            }

            goalMarker.SetActive(false);
        }
    }
}