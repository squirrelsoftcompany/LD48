using Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class DepthSlider : MonoBehaviour {
        private Slider slider;

        [SerializeField] private PlayerData playerData;
        public GameObject depthPosSpeed;
        public GameObject depthNegSpeed;

        List<GameObject> depthSpeedSignificants;

        // Start is called before the first frame update
        private void Start() {
            slider = GetComponentInChildren<Slider>();
            slider.maxValue = playerData.maxDepth;
            slider.value = playerData.maxDepth;
            depthSpeedSignificants = new List<GameObject> { depthPosSpeed, depthNegSpeed };
        }

        public void NeoDepthValue(float value)
        {
            if (!slider)
            {
                return;
            }

            StopAllCoroutines();
            depthSpeedSignificants.ForEach(x => x.SetActive(false));

            float previousValue = slider.value;
            slider.value = value;

            depthPosSpeed.SetActive(previousValue > value);
            depthNegSpeed.SetActive(previousValue < value);
            StartCoroutine(DisactivateSignificantLater());
        }

        private IEnumerator DisactivateSignificantLater()
        {
            yield return new WaitForSeconds(2);
            depthSpeedSignificants.ForEach(x => x.SetActive(false));
        }
    }
}