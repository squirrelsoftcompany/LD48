using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class MentalSlider : MonoBehaviour {
        [SerializeField] private PlayerData playerData;
        private Slider slider;
        public GameObject mentalPosSpeed;
        public GameObject mentalNegSpeed1;
        public GameObject mentalNegSpeed2;
        public GameObject mentalNegSpeed3;

        // Start is called before the first frame update
        private void Start() {
            slider = GetComponentInChildren<Slider>();
            slider.maxValue = playerData.maxHealth;
            slider.value = playerData.maxHealth;
        }

        public void NeoMentalValue(float value)
        {
            if (slider)
            {
                slider.value = value;
            }
        }
    }
}