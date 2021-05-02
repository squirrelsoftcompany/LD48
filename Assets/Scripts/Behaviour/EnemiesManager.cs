using System;
using System.Collections.Generic;
using System.Linq;
using Attribute;
using UnityEngine;
using UnityEngine.Profiling;

namespace Behaviour
{
    public class EnemiesManager : MonoBehaviour
    {
        [Serializable]
        public class BoidSettings
        {
            [TagSelector] public string m_tag;
            public bool m_activated = true;
            public bool m_avoid = false;
            public bool m_myBackCulling = false;
            public bool m_itsBackCulling = false;
            public float m_maxWeight;
            public float m_relevantRadius;
            public float m_avoidanceRadius;
            public AnimationCurve m_curve = AnimationCurve.Linear(0, 1, 1, 0);

            public float GetWeight(float distance)
            {
                float range = m_relevantRadius - m_avoidanceRadius;
                if (range <= 0)
                    return m_maxWeight;
                return m_maxWeight * m_curve.Evaluate((distance - m_avoidanceRadius) / range);
            }
        }

        [Serializable]
        public struct BoidData
        {
            public Vector3 position;
            public Vector3 forward;
        }

        public List<BoidSettings> m_boidSettings;
        public BoidSettings m_avoidSettings;
        public uint agentUpdateByFixedUpdate;

        Dictionary<BoidSettings, List<GameObject>> _taggedGOs;
        Dictionary<BoidSettings, List<BoidData>> _boidDatum;
        List<BoidData> _avoidDatum;

        Queue<EnemyBoidish> enemyBoidishQueue;

        public static EnemiesManager Get { get; private set; }

        public BoidSettings this[string boidTag] => m_boidSettings?.Find(d => d.m_tag == boidTag);

        // Start is called before the first frame update
        private void Start()
        {
            if (!Get)
                Get = this;

            // get relevant gos
            _taggedGOs = new Dictionary<BoidSettings, List<GameObject>>();
            foreach (var data in m_boidSettings)
            {
                _taggedGOs[data] = GameObject.FindGameObjectsWithTag(data.m_tag).ToList();
            }

            // init _avoidDatum
            _avoidDatum = new List<BoidData>();
            GameObject[] avoidsWall = GameObject.FindGameObjectsWithTag(m_avoidSettings.m_tag);
            foreach(var wall in avoidsWall)
            {
                var avoids = wall.GetComponent<AvoidList>();
                if (avoids)
                {
                    _avoidDatum.AddRange(avoids.m_avoidDatum);
                }
            }

            // init _boidDatum
            _boidDatum = new Dictionary<BoidSettings, List<BoidData>>();

            // queue of agent
            enemyBoidishQueue = new Queue<EnemyBoidish>();
        }

        private void FixedUpdate()
        {
            Profiler.BeginSample("EnemiesManager_FixedUpdate");

            // update boidDatum
            _boidDatum.Clear();
            foreach (var pair in _taggedGOs.Where(pair => pair.Key.m_activated)) { 
                // if not activated, then.... we don't care...
                _boidDatum[pair.Key] = pair.Value.ConvertAll(x => new BoidData { position = x.transform.position, forward = x.transform.forward });
            }

            // update wantedDirection
            for (int i = 0; i < Mathf.Min(agentUpdateByFixedUpdate, enemyBoidishQueue.Count); ++i)
            {
                var currentAgent = enemyBoidishQueue.Dequeue();
                currentAgent.UpdateWantedDirection();
                enemyBoidishQueue.Enqueue(currentAgent);
            }

            Profiler.EndSample();
        }

        // Boid computation

        public void AddAgent(EnemyBoidish agent)
        {
            enemyBoidishQueue.Enqueue(agent);
        }

        public Dictionary<BoidSettings, List<BoidData>> RelevantBoidData(BoidData self)
        {
            Dictionary<BoidSettings, List<BoidData>> relevants = new Dictionary<BoidSettings, List<BoidData>>();
            foreach (var pair in _boidDatum)
            {
                BoidSettings settings = pair.Key;
                List<BoidData> datum = pair.Value;
                relevants[settings] = RelevantBoidData(self, settings, datum);
            }
            return relevants;
        }

        public List<BoidData> RelevantAvoidData(BoidData self)
        {
            return RelevantBoidData(self, m_avoidSettings, _avoidDatum);
        }

        private List<BoidData> RelevantBoidData(BoidData self, BoidSettings settings, List<BoidData> boidDatum)
        {
            // Get those inside range
            var relevants = boidDatum.FindAll(x =>
            {
                float d = Vector3.Distance(x.position, self.position); // d can't be negative
                // if x not at self.position and weight at this distance is not null
                return d > 0 && settings.GetWeight(d) != 0;
            });

            if (settings.m_myBackCulling || settings.m_itsBackCulling)
            {
                // Back Culling
                relevants = relevants.FindAll(x =>
                {
                    Vector3 selfToX = x.position - self.position;
                    return (!settings.m_myBackCulling || Vector3.Dot(self.forward, selfToX) > 0) // cull if x is in self's back
                        && (!settings.m_itsBackCulling || Vector3.Dot(x.forward, -selfToX) > 0); // cull if self is in x's back
                });
            }
            return relevants;
        }
    }
}