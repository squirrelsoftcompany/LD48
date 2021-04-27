using System;
using System.Collections;
using JetBrains.Annotations;
using Settings;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Player {
    public class MentalHealth : MonoBehaviour {
        private float health;
        [SerializeField] private float tickLoseInterval = 1;

        [SerializeField] [Tooltip("When at minimum depth, that is the health by second that we lose")] [Range(0, 10)]
        private float minLoseZeroDepth;

        [SerializeField] [Tooltip("When at maximum depth, that is the health by second that we lose")] [Range(0, 10)]
        private float maxLoseMaxDepth;

        // loseHealth = ax + b, with x corresponding to depth
        private float loseCoefficientA;

        // loseHealth = ax + b, with x corresponding to depth
        private float loseCoefficientB;

        // in seconds, what interval we want for health=0 (minCrazyStuffInterval) and health=0.8*max
        // (= maxCrazyStuffInterval)
        [Tooltip(
            "Min interval for crazy stuff when we are at min mental health (for example: 5 for 'every 5 seconds')")]
        [SerializeField]
        private float minCrazyStuffInterval;

        [Tooltip(
            "Max interval for crazy stuff when we are at the threshold mental health (for example: 50 for 'every 50 seconds')")]
        [SerializeField]
        private float maxCrazyStuffInterval;

        [SerializeField] [Range(1, 10)] private float outOfBoundsMultiplier;

        // crazyStuffInterval = ax + b, with x corresponding to mental health (-> in seconds)
        private float crazyStuffIntervalA;

        // crazyStuffInterval = ax + b, with x corresponding to mental health (-> in seconds)
        private float crazyStuffIntervalB;
        private bool _isOutside;

        // If we are in the interval when the crazy stuff happen, we have a possibility that the crazy stuff happen:
        [SerializeField] private float percentChanceCrazy = 0.80f;

        // Below this ratio of max health, we can start the crazy stuff
        [SerializeField] private float thresholdCrazy = 0.8f;
        private float _lastTimeCrazyStuff;

        [SerializeField] private GameEvent mentalHealthEvent;
        [SerializeField] private GameEvent depthEvent;
        [SerializeField] private GameEvent crazyEvent;
        [SerializeField] private PlayerData playerData;
        [SerializeField] private GameEvent mentalGainEvent;

        [CanBeNull] private IEnumerator gainCoroutine;
        private float _gainPerSecond;
        private float _currentLoseAmount;
        private bool inBonusZone;

        private float Health {
            get => health;
            set {
                health = Math.Max(value, 0);
                mentalHealthEvent.sentFloat = health;
                mentalHealthEvent.Raise();
            }
        }

        // Start is called before the first frame update
        private void Start() {
            _isOutside = false;
            inBonusZone = false;
            crazyStuffIntervalB = minCrazyStuffInterval;
            crazyStuffIntervalA = (maxCrazyStuffInterval - minCrazyStuffInterval) / thresholdCrazy;
            loseCoefficientB = minLoseZeroDepth;
            loseCoefficientA = maxLoseMaxDepth - minLoseZeroDepth; // / 1, because 1 is full depth 
            _lastTimeCrazyStuff = 0;
            Health = playerData.maxHealth;
            StartCoroutine(CountDown());
        }

        public void gainHealth(float amount) {
            Assert.IsTrue(amount > 0);
            Health += amount;
        }

        public void goingOutOfBounds(bool isOutside) {
            _isOutside = isOutside;
        }

        private IEnumerator CountDown() {
            yield return new WaitForSeconds(tickLoseInterval);
        }

        private float _lastTimeTick = 0;

        private void loseHealth() {
            var time = Time.time;
            if (time - _lastTimeTick < tickLoseInterval) return;
            var depthPercent = depthEvent.sentFloat / playerData.maxDepth;
            _currentLoseAmount = (loseCoefficientA * depthPercent + loseCoefficientB) *
                                 (_isOutside
                                     ? outOfBoundsMultiplier
                                     : 1); // if we are outside of game bounds, lose more rapidly 
            Health -= _currentLoseAmount;
            _lastTimeTick = time;
        }

        private void Update() {
            loseHealth();

            // calculate mental health speed
            var healthSpeed = mentalHealthSpeed();
            Debug.Log("health: " + Health);
            playerData.ratioSpeedMentalHealth = healthSpeed;

            var ratioHealth = mentalHealthEvent.sentFloat / playerData.maxHealth;
            if (ratioHealth >= thresholdCrazy) return;
            // we are now crazy enough to do crazy stuff
            var interval = Time.time - _lastTimeCrazyStuff;
            if (interval < crazyStuffIntervalA * ratioHealth + crazyStuffIntervalB) return;
            // Enough time has passed since we did something crazy 
            if (Random.value < percentChanceCrazy) return;
            // do crazy stuff
            _lastTimeCrazyStuff = Time.time;
            doSomethingCrazy();
        }

        private float mentalHealthSpeed() {
            var res = (_gainPerSecond - _currentLoseAmount / tickLoseInterval) / playerData.maxHealth;
            Debug.Log("Now health speed= " + res + "\ngain = " + _gainPerSecond + "/s\nLose: " + _currentLoseAmount +
                      " in " + tickLoseInterval + " s\nMax health: " + playerData.maxHealth);
            return res;
        }

        private void doSomethingCrazy() {
            crazyEvent.Raise();
        }

        public void endGainHealth() {
            mentalGainEvent.sentBool = false;
            mentalGainEvent.Raise();
            inBonusZone = false;
            if (gainCoroutine == null) return;
            StopCoroutine(gainCoroutine);
            _gainPerSecond = 0;
            gainCoroutine = null;
        }


        public void startGainHealth(float amountPerSecond) {
            mentalGainEvent.sentBool = true;
            mentalGainEvent.Raise();
            inBonusZone = true;
            if (gainCoroutine != null) {
                StopCoroutine(gainCoroutine);
            }

            _gainPerSecond = amountPerSecond;
            gainCoroutine = gainHealthPerSecond(amountPerSecond);
            StartCoroutine(gainCoroutine);
        }

        private IEnumerator gainHealthPerSecond(float amount) {
            while (inBonusZone) {
                Health += amount * Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}