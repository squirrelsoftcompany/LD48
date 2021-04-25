using UnityEngine;

namespace Player {
    [RequireComponent(typeof(Rigidbody))]
    public class CrazyAction : MonoBehaviour {
        private Rigidbody _rigidbody;
        [SerializeField] private float minSpinDegrees, maxSpinDegrees, minForce, maxForce, minExplosion, maxExplosion;
        [SerializeField] private float minExplosionRadius;
        [SerializeField] private float maxExplosionRadius;
        [SerializeField] private float maxDistanceExplosion;
        [SerializeField] private float minUpwardsModifier;
        [SerializeField] private float maxUpwardsModifier;
        [SerializeField] private GameEvent crazyAction;

        private void Start() {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void launchCrazy() {
            var r = Random.value;
            if (r >= 1 / 2f) {
                crazyAction.sentInt = 1;
                crazySpin();
            } else if (r >= 1 / 4f) {
                crazyAction.sentInt = 2;
                crazyBackward();
            } else {
                crazyAction.sentInt = 3;
                crazyExplosion();
            }

            crazyAction.Raise();
        }

        private void crazySpin() {
            var amount = Random.Range(minSpinDegrees, maxSpinDegrees);
            var direction = Random.value >= 0.5 ? Vector3.up : Vector3.right;
            _rigidbody.AddRelativeTorque(direction * amount, ForceMode.Impulse);
        }

        private void crazyBackward() {
            var amount = Random.Range(minForce, maxForce);
            _rigidbody.AddForce(-transform.forward * amount, ForceMode.Impulse);
        }

        public void crazyExplosion() {
            var amount = Random.Range(minExplosion, maxExplosion);
            var explosionRadius = Random.Range(minExplosionRadius, maxExplosionRadius);
            var upwardsModifier = Random.Range(minUpwardsModifier, maxUpwardsModifier);
            var position = Random.insideUnitSphere * maxDistanceExplosion + transform.position;
            _rigidbody.AddExplosionForce(amount, position, explosionRadius,
                upwardsModifier, ForceMode.Impulse);
        }
    }
}