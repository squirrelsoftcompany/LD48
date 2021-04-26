using System.Collections;
using JetBrains.Annotations;
using Props;
using UnityEngine;

namespace Player {
    public class Echolocation : MonoBehaviour {
        [Tooltip("We need to drop a fart whenever we use echolocation. This will be used for enemies to find you")]
        [SerializeField]
        private GameObject fartPrefab;

        [Tooltip(
            "We need to keep a tracker on whenever we use echolocation. This will be used for enemies to find you")]
        [SerializeField]
        private GameObject trackerPrefab;

        [CanBeNull] private GameObject _previousFart, _previousTracker;

        [Tooltip("A prefab to show the direction of the goal")] [SerializeField]
        private GameObject goalMarker;

        [Tooltip("For how much time we show the goal")] [SerializeField]
        private float secondsShowGoal;

        [CanBeNull] private IEnumerator _showCoroutine;

        [Tooltip("Duration before fart disappears")] [SerializeField]
        private float fartDuration;

        [Tooltip("Duration before tracker disappears")] [SerializeField]
        private float trackerDuration;

        [CanBeNull] private IEnumerator _destroyCoroutineFart, _destroyCoroutineTracker;

        private delegate void Method();

        public void dropFart() {
            var currentPosition = transform.position;
            // No more than 1 fart
            if (_previousFart != null) {
                if (_destroyCoroutineFart != null) {
                    StopCoroutine(_destroyCoroutineFart);
                }

                Destroy(_previousFart);
            }

            _previousFart = Instantiate(fartPrefab, currentPosition, Quaternion.identity);
            _destroyCoroutineFart = getCoroutineDelay(fartDuration, () => {
                Destroy(_previousFart);
                _destroyCoroutineFart = null;
            });
            StartCoroutine(_destroyCoroutineFart);
        }

        public void dropTracker() {
            // no more than 1 tracker
            if (_previousTracker != null) {
                if (_destroyCoroutineTracker != null) {
                    StopCoroutine(_destroyCoroutineTracker);
                }

                Destroy(_previousTracker);
            }

            _previousTracker = Instantiate(trackerPrefab, transform);
            _destroyCoroutineTracker = getCoroutineDelay(trackerDuration, () => {
                Destroy(_previousTracker);
                _destroyCoroutineTracker = null;
            });
            StartCoroutine(_destroyCoroutineTracker);
        }

        private static IEnumerator getCoroutineDelay(float nSeconds, Method method) {
            yield return new WaitForSeconds(nSeconds);
            method();
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