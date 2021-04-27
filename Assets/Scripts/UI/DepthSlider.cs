using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class DepthSlider : MonoBehaviour {
        private Slider slider;

        [SerializeField] private PlayerData playerData;

        // Start is called before the first frame update
        private void Start() {
            slider = GetComponentInChildren<Slider>();
            slider.maxValue = playerData.maxDepth;
            slider.value = playerData.maxDepth;
        }
    }
}