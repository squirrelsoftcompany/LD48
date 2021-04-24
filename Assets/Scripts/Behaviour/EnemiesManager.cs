using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Attribute;

namespace Behaviour
{
    public class EnemiesManager : MonoBehaviour
    {
        [System.Serializable]
        public class BoidData
        {
            [TagSelector] public string m_tag;
            public bool m_activated = true;
            public bool m_avoid = false;
            public float m_maxWeight;
            public float m_relevantRadius;
            public float m_avoidanceRadius;

            public float GetWeight(float distance)
            {
                float range = m_relevantRadius - m_avoidanceRadius;
                return range > 0 ? Mathf.Lerp(m_maxWeight, 0, (distance - m_avoidanceRadius) / range) : m_maxWeight;
            }
        }

        public List<BoidData> m_boidDatum;

        List<GameObject> _relevantGos;

        static EnemiesManager _inst;
        public static EnemiesManager Get => _inst;
        public static List<GameObject> RelevantGos => _inst?._relevantGos;
        public static List<BoidData> BoidDatum => _inst?.m_boidDatum;
        public BoidData this[string tag] => m_boidDatum?.Find(d => d.m_tag == tag);

        // Start is called before the first frame update
        void Start()
        {
            if (!_inst)
                _inst = this;

            // get relevant gos
            _relevantGos = new List<GameObject>();
            foreach (var data in m_boidDatum)
            {
                _relevantGos.AddRange(GameObject.FindGameObjectsWithTag(data.m_tag));
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}