using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour.Gizmo
{
    public class BoidSettingsGizmos : MonoBehaviour
    {
#if UNITY_EDITOR
        Behaviour.EnemiesManager.BoidSettings _boidData;

        // Start is called before the first frame update
        void Start()
        {
            _boidData = Behaviour.EnemiesManager.Get[tag];
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(transform.position, 15);
                return;
            }

            if (_boidData == null || !_boidData.m_activated)
                return;

            Gizmos.color = _boidData.m_avoid ? new Color(1, 0, 0, 0.8f) : new Color(0, 1, 0, 0.8f);
            if (_boidData.m_relevantRadius != Mathf.Infinity)
                Gizmos.DrawSphere(transform.position, _boidData.m_avoidanceRadius);
            if (_boidData.m_avoidanceRadius != Mathf.Infinity)
                Gizmos.DrawWireSphere(transform.position, _boidData.m_relevantRadius);
        }
#endif // UNITY_EDITOR
    }
}