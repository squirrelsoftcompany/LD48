using UnityEngine;

namespace Player {
    [RequireComponent(typeof(Rigidbody))]
    public class CrazyAction : MonoBehaviour {
        private Rigidbody _rigidbody;

        [Header("Crazy Spin")] [SerializeField]
        private Vector2 minMaxSpinDegrees;

        [Header("Crazy Backward")] [SerializeField]
        private Vector2 minMaxBackwardForce;

        [Header("Crazy Explosion")] [Tooltip("Explosion force")] [SerializeField]
        private Vector2 minMaxExplosion;

        [Tooltip("Radius of the explosion")] [SerializeField]
        private Vector2 minMaxExplosionRadius;

        [Tooltip("Maximum distance from the submarine for the explosion")] [SerializeField]
        private float maxDistanceExplosion;

        [Tooltip("Min and max force upwards for explosion")] [SerializeField]
        private Vector2 minMaxUpwardsModifier;

        [Header("GameEvent")] [SerializeField] private GameEvent crazyAction;

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
            var amount = Random.Range(minMaxSpinDegrees.x, minMaxSpinDegrees.y);
            var direction = Random.value >= 0.5 ? Vector3.up : Vector3.right;
            _rigidbody.AddRelativeTorque(direction * amount, ForceMode.Impulse);
        }

        private void crazyBackward() {
            var amount = Random.Range(minMaxBackwardForce.x, minMaxBackwardForce.y);
            _rigidbody.AddForce(-transform.forward * amount, ForceMode.Impulse);
        }

        public void crazyExplosion() {
            var amount = Random.Range(minMaxExplosion.x, minMaxExplosion.y);
            var explosionRadius = Random.Range(minMaxExplosionRadius.x, minMaxExplosionRadius.y);
            var upwardsModifier = Random.Range(minMaxUpwardsModifier.x, minMaxUpwardsModifier.y);
            var position = Random.insideUnitSphere * maxDistanceExplosion + transform.position;
            _rigidbody.AddExplosionForce(amount, position, explosionRadius,
                upwardsModifier, ForceMode.Impulse);
        }
    }
}