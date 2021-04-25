using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Attribute;
using System.Linq;

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

        Dictionary<BoidData, List<GameObject>> _taggedGOs;

        static EnemiesManager _inst;
        public static EnemiesManager Get => _inst;
        public static List<BoidData> BoidDatum => _inst?.m_boidDatum;
        public BoidData this[string tag] => m_boidDatum?.Find(d => d.m_tag == tag);

        public List<GameObject> RelevantGOs(GameObject go)
        {
            List<GameObject> relevants = new List<GameObject>();
            foreach (var pair in _taggedGOs)
            {
                //  Get inside range
                relevants.AddRange(pair.Value.FindAll(
                    x => pair.Key.m_relevantRadius == Mathf.Infinity ||
                            Vector3.Distance(x.transform.position, go.transform.position) < pair.Key.m_relevantRadius));                
            }
            return relevants;
        }

        // Start is called before the first frame update
        void Start()
        {
            if (!_inst)
                _inst = this;

            // get relevant gos
            _taggedGOs = new Dictionary<BoidData, List<GameObject>>();
            foreach (var data in m_boidDatum)
            {
                if (data.m_activated) // if not activated, then.... we don't care...
                    _taggedGOs[data] = GameObject.FindGameObjectsWithTag(data.m_tag).ToList();
            }
        }
    }
}