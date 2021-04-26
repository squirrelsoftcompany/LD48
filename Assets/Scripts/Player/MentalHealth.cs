using System;
using System.Collections;
using Settings;
using UnityEngine;
using UnityEngine.Assertions;

namespace Player {
    public class MentalHealth : MonoBehaviour {
        private float health;
        [SerializeField] private float tickLoseInterval = 1;

        // loseHealth = ax + b, with x corresponding to depth
        [SerializeField] private float loseCoefficientA;

        // loseHealth = ax + b, with x corresponding to depth
        [SerializeField] private float loseCoefficientB;

        // in seconds, what interval we want for health=0 (minCrazyStuffInterval) and health=0.8*max
        // (= maxCrazyStuffInterval)
        [SerializeField] private float minCrazyStuffInterval, maxCrazyStuffInterval;

        // crazyStuffInterval = ax + b, with x corresponding to mental health (-> in seconds)
        private float crazyStuffIntervalA;

        // crazyStuffInterval = ax + b, with x corresponding to mental health (-> in seconds)
        private float crazyStuffIntervalB;

        // If we are in the interval when the crazy stuff happen, we have a possibility that the crazy stuff happen:
        [SerializeField] private float percentChanceCrazy = 0.80f;

        // Below this ratio of max health, we can start the crazy stuff
        [SerializeField] private float thresholdCrazy = 0.8f;
        private float _lastTimeCrazyStuff;

        [SerializeField] private GameEvent mentalHealthEvent;
        [SerializeField] private GameEvent depthEvent;
        [SerializeField] private GameEvent crazyEvent;
        [SerializeField] private PlayerData playerData;

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
            crazyStuffIntervalB = minCrazyStuffInterval;
            crazyStuffIntervalA = (maxCrazyStuffInterval - minCrazyStuffInterval) / thresholdCrazy;
            _lastTimeCrazyStuff = 0;
            Health = playerData.maxHealth;
            StartCoroutine(CountDown());
        }

        public void gainHealth(float amount) {
            Assert.IsTrue(amount > 0);
            Health += amount;
        }

        private IEnumerator CountDown() {
            while (Health > 0) {
                yield return new WaitForSeconds(tickLoseInterval);
                var depthPercent = depthEvent.sentFloat / playerData.maxDepth;
                Health -= loseCoefficientA * depthPercent + loseCoefficientB;
            }

            mentalHealthEvent.sentFloat = 0;
            mentalHealthEvent.Raise();
        }

        private void Update() {
            var ratioHealth = mentalHealthEvent.sentFloat / playerData.maxHealth;
            if (ratioHealth >= thresholdCrazy) return;
            // we are now crazy enough to do crazy stuff
            var interval = Time.time - _lastTimeCrazyStuff;
            if (interval < crazyStuffIntervalA * ratioHealth + crazyStuffIntervalB) return;
            // Enough time has passed since we did something crazy 
            if (UnityEngine.Random.value < percentChanceCrazy) return;
            // do crazy stuff
            _lastTimeCrazyStuff = Time.time;
            doSomethingCrazy();
        }

        private void doSomethingCrazy() {
            crazyEvent.Raise();
        }
    }
}