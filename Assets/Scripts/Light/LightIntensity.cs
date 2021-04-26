using UnityEngine;

namespace Light {
    public class LightIntensity : MonoBehaviour {
        public float maxIntensity = 100000.0f;
        public float minIntensity = 0.0f;
        public float maxDistance = 1000.0f;
        public float minDistance = 20.0f;

        private UnityEngine.Light myLight;
        private Camera mainCamera;

        // Start is called before the first frame update
        private void Start() {
            myLight = GetComponent<UnityEngine.Light>();
            mainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update() {
            float dist = Vector3.Distance(mainCamera.transform.position, transform.position - new Vector3(0, 100, 0));

            float distanceRange = Mathf.InverseLerp(minDistance, maxDistance, dist);
            float intensity = Mathf.Lerp(Mathf.Sqrt(minIntensity), Mathf.Sqrt(maxIntensity), distanceRange);
            myLight.intensity = intensity * intensity;
        }
    }
}