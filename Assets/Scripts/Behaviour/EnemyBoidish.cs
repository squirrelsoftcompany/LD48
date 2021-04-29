using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Profiling;

namespace Behaviour
{
    public class EnemyBoidish : MonoBehaviour
    {
        [Tooltip("Forward velocity (m/s)")]
        public float m_forwardVelocity = 30; // 100km/h (it's fast for a submarine)
        [Tooltip("Angular velocity (deg/s)")]
        public float m_angularVelocity = 5;

        [Space]
        public bool m_deepGizmos = false;

        private Vector3 _wantedDirection;
        private Rigidbody _rigidbody;
        private EnemiesManager.BoidSettings _selfSettings;
        private EnemiesManager.BoidData _selfData;

        // Start is called before the first frame update
        void Start()
        {
            _wantedDirection = transform.forward;
            _rigidbody = GetComponent<Rigidbody>();
            _selfSettings = EnemiesManager.Get[tag];
            _selfData = new EnemiesManager.BoidData() { position = gameObject.transform.position, forward = gameObject.transform.forward };

#if UNITY_EDITOR
            // GIZMOS
            _gizmoDatum = new List<GizmoData>();
#endif // UNITY_EDITOR

            EnemiesManager.Get.AddAgent(this);
        }

        private void FixedUpdate()
        {
            // Apply
            float lRadAngularVelocity = m_angularVelocity * Mathf.Deg2Rad;
            float lCosTheta = Vector3.Dot(_selfData.forward, _wantedDirection);
            _rigidbody.maxAngularVelocity = lRadAngularVelocity;

            // compute rotation function based on current and wanted direction and angular velocity
            if (lCosTheta < 1 - Mathf.Epsilon)
            {
                // get axis and angle
                Vector3 rotationAxis = Vector3.Cross(_selfData.forward, _wantedDirection);
                float theta = Mathf.Asin(rotationAxis.magnitude);
                Vector3 w = rotationAxis.normalized * Mathf.Min(theta, lRadAngularVelocity) / Time.fixedDeltaTime;

                // compute tensor
                Quaternion q = transform.rotation * _rigidbody.inertiaTensorRotation;
                Vector3 tensor = q * Vector3.Scale(_rigidbody.inertiaTensor, (Quaternion.Inverse(q) * w));
                _rigidbody.AddTorque(tensor, ForceMode.Impulse);
            }
            // decrease velocity when there is huge rotation to do
            float forwardVelocity = Mathf.Lerp(m_forwardVelocity/ 2, m_forwardVelocity, lCosTheta);
            _rigidbody.AddForce(transform.forward * forwardVelocity);
        }

        public void UpdateWantedDirection()
        {
            Profiler.BeginSample("EnemyBoidish_UpdateWantedDirection");

#if UNITY_EDITOR
            // GIZMOS
            if (m_deepGizmos)
            {
                _gizmoDatum.Clear();
            }
#endif // UNITY_EDITOR

            _selfData.position = gameObject.transform.position;
            _selfData.forward = gameObject.transform.forward;

            // Take in count current direction for a weight of one
            Vector3 directionSum = _selfData.forward;

            // Take in count relevants
            var relevants = EnemiesManager.Get.RelevantBoidData(_selfData);
            foreach (var pair in relevants)
            {
                var settings = pair.Key;
                var datum = pair.Value;

                foreach (var data in datum)
                {
                    Vector3 difference = data.position - _selfData.position;
                    Vector3 direction = difference.normalized;
                    float distance = difference.magnitude;

                    float w = settings.GetWeight(distance);
                    Vector3 forward = settings.m_avoid ? -direction : direction;

                    directionSum += forward * w;

#if UNITY_EDITOR
                    // GIZMOS
                    if (m_deepGizmos)
                    {
                        _gizmoDatum.Add(new GizmoData() { m_position = data.position, m_direction = forward, m_weight = w });
                    }
#endif // UNITY_EDITOR
                }
            }

            // Take in count avoid data
            var relevantAvoidDatum = EnemiesManager.Get.RelevantAvoidData(_selfData);
            var avoidSettings = EnemiesManager.Get.m_avoidSettings;

            foreach (var avoidData in relevantAvoidDatum)
            {
                Vector3 difference = avoidData.position - _selfData.position;
                Vector3 directionToAvoid = difference.normalized;
                float distance = difference.magnitude;

                float w = avoidSettings.GetWeight(distance);
                Vector3 finalDirection = directionToAvoid;
                if (avoidSettings.m_avoid) // avoid
                {
                    float cosTheta = Vector3.Dot(_selfData.forward, directionToAvoid);
                    if (cosTheta < 0) // other boid is behind this
                        finalDirection = Vector3.Reflect(directionToAvoid, -avoidData.forward);
                    else
                        finalDirection = Vector3.Reflect(directionToAvoid, avoidData.forward);
                }

                directionSum += finalDirection * w;

#if UNITY_EDITOR
                // GIZMOS
                if (m_deepGizmos)
                {
                    _gizmoDatum.Add(new GizmoData() { m_position = avoidData.position, m_direction = finalDirection, m_weight = w });
                }
#endif // UNITY_EDITOR
            }

            // compute wantedDirection
            _wantedDirection = directionSum.normalized;

            Profiler.EndSample();
        }

#if UNITY_EDITOR
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
            if (!Application.isPlaying || !m_deepGizmos)
                return;

            Gizmos.color = Color.green;
            foreach (var data in _gizmoDatum)
            {
                Gizmos.DrawLine(data.m_position, data.m_position + data.m_direction * 10 * data.m_weight);
            }
        }
#endif // UNITY_EDITOR
    }
}
