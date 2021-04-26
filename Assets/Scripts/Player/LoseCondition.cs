using System;
using UnityEngine;

namespace Player {
    public class LoseCondition : MonoBehaviour {
        [SerializeField] private GameEvent loseEvent;

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Enemy")) {
                loseEvent.Raise();
            }
        }
    }
}