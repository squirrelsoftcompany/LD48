using UnityEngine;

namespace Player {
    public class Particles : MonoBehaviour {
        [SerializeField] private ParticleSystem particlesRight;
        [SerializeField] private ParticleSystem particlesLeft;

        public void toggleParticles(bool play) {
            if (play) {
                particlesLeft.Play();
                particlesRight.Play();
            } else {
                particlesLeft.Stop();
                particlesRight.Stop();
            }
        }
    }
}