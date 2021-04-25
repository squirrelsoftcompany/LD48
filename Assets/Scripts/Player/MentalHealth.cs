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
        [SerializeField] private float maxHealth = 200f;
        [SerializeField] private GameEvent mentalHealthEvent;
        [SerializeField] private GameEvent depthEvent;
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
            Health = maxHealth;
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
    }
}