using Settings;
using System.Collections;
using System.Collections.Generic;
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

        List<GameObject> mentalSpeedSignificants;

        // Start is called before the first frame update
        private void Start() {
            slider = GetComponentInChildren<Slider>();
            slider.maxValue = playerData.maxHealth;
            slider.value = playerData.maxHealth;

            mentalSpeedSignificants = new List<GameObject> { mentalPosSpeed, mentalNegSpeed1, mentalNegSpeed2, mentalNegSpeed3 };
        }

        public void NeoMentalValue(float value)
        {
            if (!slider)
            {
                return;
            }

            StopAllCoroutines();
            mentalSpeedSignificants.ForEach(x => x.SetActive(false));

            slider.value = value;

            mentalPosSpeed.SetActive(playerData.ratioSpeedMentalHealth > 0);
            mentalNegSpeed1.SetActive(playerData.ratioSpeedMentalHealth < 0);
            mentalNegSpeed2.SetActive(playerData.ratioSpeedMentalHealth < -1/3f);
            mentalNegSpeed3.SetActive(playerData.ratioSpeedMentalHealth < -2/3f);
            StartCoroutine(DisactivateSignificantLater());
        }

        private IEnumerator DisactivateSignificantLater()
        {
            yield return new WaitForSeconds(2);
            mentalSpeedSignificants.ForEach(x => x.SetActive(false));
        }
    }
}