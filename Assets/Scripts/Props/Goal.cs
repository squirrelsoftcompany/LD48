using UnityEngine;

namespace Props {
    public class Goal : MonoBehaviour {
        [SerializeField] private GameEvent winEvent;

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Player")) {
                winEvent.Raise();
            }
        }
    }
}