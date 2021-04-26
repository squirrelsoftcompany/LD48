using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class DepthSlider : MonoBehaviour {
        [SerializeField] private Slider slider;

        [SerializeField] private PlayerData playerData;

        // Start is called before the first frame update
        private void Start() {
            slider.maxValue = playerData.maxDepth;
        }
    }
}