using System.Collections;
using Player;
using UnityEngine;

namespace Props {
    public class BonusMental : MonoBehaviour {
        [SerializeField] private float bonusAmount;
        [SerializeField] private float coolDownSeconds;
        [SerializeField] private Light bonusLight;
        private float _leftCoolDownTime;

        private void Start() {
            _leftCoolDownTime = 0;
        }

        private void OnTriggerEnter(Collider other) {
            if (_leftCoolDownTime > 0) return;

            var otherHealth = other.GetComponent<MentalHealth>();
            if (otherHealth == null) return;
            otherHealth.gainHealth(bonusAmount);
            sendToCoolDown();
        }

        private void sendToCoolDown() {
            StartCoroutine(coolDown());
        }

        private IEnumerator coolDown() {
            _leftCoolDownTime = coolDownSeconds;
            bonusLight.intensity = 0.01f;
            var startTime = Time.time;
            while (_leftCoolDownTime > 0) {
                var elapsed = Time.time - startTime;
                yield return new WaitForFixedUpdate();
                _leftCoolDownTime = coolDownSeconds - elapsed;
            }
            bonusLight.intensity = 1f;
            _leftCoolDownTime = 0;
        }
    }
}