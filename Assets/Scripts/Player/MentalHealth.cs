using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Player {
    public class MentalHealth : MonoBehaviour {
        private float health;
        [SerializeField] private float tickInterval = 1;
        [SerializeField] private float loseHealth = 0.5f;
        [SerializeField] private float maxHealth = 200f;
        [SerializeField] private GameEvent mentalHealthEvent;

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
                yield return new WaitForSeconds(tickInterval);
                Health -= loseHealth;
            }

            mentalHealthEvent.sentFloat = 0;
            mentalHealthEvent.Raise();
        }
    }
}