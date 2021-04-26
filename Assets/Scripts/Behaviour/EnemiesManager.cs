using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Attribute;
using System.Linq;
using UnityEngine.Profiling;

namespace Behaviour
{
    public class EnemiesManager : MonoBehaviour
    {
        [System.Serializable]
        public class BoidSettings
        {
            [TagSelector] public string m_tag;
            public bool m_activated = true;
            public bool m_avoid = false;
            public bool m_backCulling = false;
            public float m_maxWeight;
            public float m_relevantRadius;
            public float m_avoidanceRadius;

            public float GetWeight(float distance)
            {
                float range = m_relevantRadius - m_avoidanceRadius;
                return range > 0 ? Mathf.Lerp(m_maxWeight, 0, (distance - m_avoidanceRadius) / range) : m_maxWeight;
            }
        }

        public struct BoidData
        {
            public Vector3 position;
            public Vector3 forward;
        }

        public List<BoidSettings> m_boidSettings;

        Dictionary<BoidSettings, List<GameObject>> _taggedGOs;
        Dictionary<BoidSettings, List<BoidData>> _boidDatum;

        static EnemiesManager _inst;
        public static EnemiesManager Get => _inst;
        public static List<BoidSettings> BoidsSettings => _inst?.m_boidSettings;
        public BoidSettings this[string tag] => m_boidSettings?.Find(d => d.m_tag == tag);

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

        public Dictionary<BoidSettings, List<BoidData>> RelevantBoidData(BoidData self)
        {
            Dictionary<BoidSettings, List<BoidData>> relevants = new Dictionary<BoidSettings, List<BoidData>>();
            foreach (var pair in _boidDatum)
            {
                BoidSettings settings = pair.Key;
                List<BoidData> datum = pair.Value;

                //  Get inside range
                relevants[settings] = datum.FindAll(
                    x => settings.m_relevantRadius == Mathf.Infinity ||
                            Vector3.Distance(x.position, self.position) < pair.Key.m_relevantRadius);
                if (settings.m_backCulling)
                {
                    // Back Culling
                    relevants[settings] = relevants[settings].FindAll(x =>
                        {
                            Vector3 selfToX = x.position - self.position;
                            return Vector3.Dot(self.forward, selfToX) > 0
                                && Vector3.Dot(x.forward, -selfToX) > 0;
                        });
                }
            }
            return relevants;
        }

        // Start is called before the first frame update
        void Start()
        {
            if (!_inst)
                _inst = this;

            // get relevant gos
            _taggedGOs = new Dictionary<BoidSettings, List<GameObject>>();
            foreach (var data in m_boidSettings)
            {
                _taggedGOs[data] = GameObject.FindGameObjectsWithTag(data.m_tag).ToList();
            }

            // init _boidDatum
            _boidDatum = new Dictionary<BoidSettings, List<BoidData>>();
        }

        void FixedUpdate()
        {
            Profiler.BeginSample("EnemyBoidish_FixedUpdate");

            _boidDatum.Clear();
            foreach (var pair in _taggedGOs)
            {
                if (pair.Key.m_activated) // if not activated, then.... we don't care...
                    _boidDatum[pair.Key] = pair.Value.ConvertAll(x => new BoidData { position = x.transform.position, forward = x.transform.forward });
            }

            Profiler.EndSample();
        }
    }
}