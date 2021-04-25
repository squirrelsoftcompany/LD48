using UnityEngine;

namespace Props {
    [RequireComponent(typeof(AudioSource))]
    public class EchoResponse : MonoBehaviour {
        private AudioSource audioSource;
        const float CELERITE_SOUND = 343f; // in m/s
        [SerializeField] private GameEvent echoResponse;
        private GameObject player;

        // Start is called before the first frame update
        private void Start() {
            audioSource = GetComponent<AudioSource>();
            player = GameObject.FindWithTag("Player");
        }

        public void respond() {
            var distance = Vector3.Distance(transform.position, player.transform.position);
            // twice the distance
            var time = distance * 2f / CELERITE_SOUND; 
            Invoke(nameof(play), time);
        }

        private void play() {
            audioSource.Play();
            echoResponse.Raise();
        }

        // Update is called once per frame
        void Update() { }
    }
}