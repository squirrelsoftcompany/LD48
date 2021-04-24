using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            // Update direction here
            _wantedDirection = new Vector3(0, -150, 0) - _rigidbody.position;
            _wantedDirection.Normalize();
        }
    }
}