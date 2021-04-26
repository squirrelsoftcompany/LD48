using Player;
using UnityEngine;

namespace Props {
    public class BonusMental : MonoBehaviour {
        [Tooltip("Bonus health per second")] [SerializeField] [Range(0.1f, 50)]
        private float bonusAmountPerSecond;
        
        private void OnTriggerEnter(Collider other) {
            var otherHealth = other.GetComponent<MentalHealth>();
            if (otherHealth == null) return;
            otherHealth.startGainHealth();
        }

        private void OnTriggerStay(Collider other) {
            var otherHealth = other.GetComponent<MentalHealth>();
            if (otherHealth == null) return;
            otherHealth.gainHealth(bonusAmountPerSecond * Time.fixedDeltaTime);
        }

        private void OnTriggerExit(Collider other) {
            var otherHealth = other.GetComponent<MentalHealth>();
            if (otherHealth == null) return;
            otherHealth.endGainHealth();
        }
    }
}