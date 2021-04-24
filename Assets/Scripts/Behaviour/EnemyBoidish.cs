using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Behaviour
{
    public class EnemyBoidish : MonoBehaviour
    {
        [Tooltip("Forward velocity (m/s)")]
        public float m_forwardVelocity = 30; // 100km/h (it's fast for a submarine)
        [Tooltip("Angular velocity (deg/s)")]
        public float m_angularVelocity = 5;

        private Vector3 _wantedDirection;
        private Rigidbody _rigidbody;

        // Start is called before the first frame update
        void Start()
        {
            _wantedDirection = transform.forward;
            _rigidbody = GetComponent<Rigidbody>();

            // GIZMOS
            _gizmoDatum = new List<GizmoData>();
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 currentDirection = transform.forward;
            float lRadAngularVelocity = m_angularVelocity * Mathf.Deg2Rad;
            float lCosTheta = Vector3.Dot(currentDirection, _wantedDirection);

            if (lCosTheta < 1 - Mathf.Epsilon)
            {
                Quaternion.FromToRotation(currentDirection, _wantedDirection).ToAngleAxis(out float lAngle, out Vector3 lAxis);
                _rigidbody.angularVelocity = lAxis * Mathf.Min(lAngle, lRadAngularVelocity);
            }
            else
            {
                _rigidbody.angularVelocity = Vector3.zero;
            }
            _rigidbody.maxAngularVelocity = lRadAngularVelocity;
            _rigidbody.velocity = transform.forward * m_forwardVelocity;
        }

        private void FixedUpdate()
        {
            // GIZMOS
            _gizmoDatum.Clear();

            Vector3 directionSum = transform.forward; // self weight = 1
            
            foreach (var relevant in EnemiesManager.RelevantGos)
            {
                if (gameObject == relevant) // self direction already taken in count
                    continue;

                EnemiesManager.BoidData data = EnemiesManager.Get[relevant.tag];
                if (data == null || ! data.m_activated) // boid data doesn't exist or is disactivated
                    continue;

                Vector3 difference = relevant.transform.position - _rigidbody.position;
                Vector3 direction = difference.normalized;
                float distance = Mathf.Abs(difference.magnitude);

                if (distance < data.m_relevantRadius) // inside range
                {
                    float w = data.GetWeight(distance);
                    Vector3 finalDirection = direction;
                    if (data.m_avoid) // avoid
                    {
                        finalDirection = -direction;
                    }

                    directionSum += finalDirection * w;

                    // GIZMOS
                    var gizmoData = new GizmoData();
                    gizmoData.m_position = relevant.transform.position;
                    gizmoData.m_direction = finalDirection;
                    gizmoData.m_weight = w;
                    _gizmoDatum.Add(gizmoData);
                }
            }
            _wantedDirection = directionSum.normalized;
        }


        // GIZMOS
        private struct GizmoData { public Vector3 m_position; public Vector3 m_direction; public float m_weight; }
        private List<GizmoData> _gizmoDatum;

        private void OnDrawGizmos()
        {
            Vector3 position = transform.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(position, position + transform.forward * 20);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(position, position + _wantedDirection * 20);
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                return;

            Gizmos.color = Color.green;
            foreach (var data in _gizmoDatum)
            {
                Gizmos.DrawLine(data.m_position, data.m_position + data.m_direction * 10 * data.m_weight);
            }
        }
    }
}
