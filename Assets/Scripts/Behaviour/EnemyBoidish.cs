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

        [Space]
        public bool m_deepGizmos = false;

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
            if (m_deepGizmos)
            {
                _gizmoDatum.Clear();
            }

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
                        //finalDirection = -direction;
                        Vector3 inter = RaySphereIntersection(transform.position, direction, relevant.transform.position, data.m_avoidanceRadius);

                        Vector3 N = inter - relevant.transform.position;
                        N.Normalize();
                        Vector3 D = inter - _rigidbody.position;
                        D.Normalize();
                        Vector3 R = D - 2 * (Vector3.Dot(D, N)) * N; // reflection vector
                        R.Normalize();

                        float cosTheta = Vector3.Dot(transform.forward, D);
                        if (cosTheta < 0) // inter is behind this
                            finalDirection = -D;
                        else
                            finalDirection = R;
                    }

                    directionSum += finalDirection * w;

                    // GIZMOS
                    if (m_deepGizmos)
                    {
                        var gizmoData = new GizmoData();
                        gizmoData.m_position = relevant.transform.position;
                        gizmoData.m_direction = finalDirection;
                        gizmoData.m_weight = w;
                        _gizmoDatum.Add(gizmoData);
                        //_gizmoDatum.Add(new GizmoData() { m_position = relevant.transform.position, m_direction = finalDirection, m_weight = w });
                    }
                }
            }
            _wantedDirection = directionSum.normalized;
        }

        Vector3 RaySphereIntersection(
            Vector3 rayPos, Vector3 rayDir,
            Vector3 spherePos, float sphereRadius)
        {
            Vector3 o_minus_c = rayPos - spherePos;

            float p = Vector3.Dot(rayDir, o_minus_c);
            float q = Vector3.Dot(o_minus_c, o_minus_c) - (sphereRadius * sphereRadius);

            float discriminant = (p * p) - q;
            if (discriminant< 0.0f)
            {
                return Vector3.positiveInfinity;
            }

            float dRoot = Mathf.Sqrt(discriminant);
            float dist1 = -p - dRoot;
            float dist2 = -p + dRoot;

            float initialD = (discriminant > Mathf.Epsilon) ? Mathf.Min(dist1, dist2) : dist2;
            return Vector3.Lerp(rayPos, spherePos, initialD / Vector3.Distance(rayPos, spherePos));
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
            if (!Application.isPlaying || !m_deepGizmos)
                return;

            Gizmos.color = Color.green;
            foreach (var data in _gizmoDatum)
            {
                Gizmos.DrawLine(data.m_position, data.m_position + data.m_direction * 10 * data.m_weight);
            }
        }
    }
}
