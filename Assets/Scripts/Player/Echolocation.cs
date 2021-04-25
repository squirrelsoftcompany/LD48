using JetBrains.Annotations;
using UnityEngine;

namespace Player {
    public class Echolocation : MonoBehaviour {
        // We need to drop a fart whenever we use echolocation
        // This will be used for enemies to find you
        [SerializeField] private GameObject fartPrefab;
        [CanBeNull] private GameObject _previousFart;

        public void dropFart() {
            var currentPosition = transform.position;
            // No more than 1 fart
            if (_previousFart != null) {
                Destroy(_previousFart);
            }

            _previousFart = Instantiate(fartPrefab, currentPosition, Quaternion.identity);
        }
    }
}