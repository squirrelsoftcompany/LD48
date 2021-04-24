using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[ExecuteInEditMode]
public class SubmarineController : MonoBehaviour {
    private Rigidbody _rigidbody;
    [SerializeField] private float angularVelocity;
    [SerializeField] private float velocity;
    [SerializeField] private float maxVelocity;
    private float _sqrMaxVelocity;

    private void OnValidate() {
        setMaxVelocity(maxVelocity);
    }

    private void setMaxVelocity(float value) {
        maxVelocity = value;
        _sqrMaxVelocity = maxVelocity * maxVelocity;
    }

    // Start is called before the first frame update
    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.maxAngularVelocity = Mathf.Deg2Rad * angularVelocity;
        _sqrMaxVelocity = maxVelocity * maxVelocity;
    }

    private void FixedUpdate() {
        if (_rigidbody.velocity.sqrMagnitude > _sqrMaxVelocity) {
            // clamp velocity
            _rigidbody.velocity = _rigidbody.velocity.normalized * maxVelocity;
        }

        Vector3? wantedDirection = null;
        if (Input.GetKey("up")) {
            // We want to go upward
            wantedDirection = Vector3.left;
        } else if (Input.GetKey("down")) {
            // We want to go upward
            wantedDirection = Vector3.right;
        } else if (Input.GetKey("left")) {
            // We want to go left
            wantedDirection = Vector3.down;
        } else if (Input.GetKey("right")) {
            wantedDirection = Vector3.up;
        }

        print("wanted Dir" + wantedDirection);
        if (wantedDirection != null) {
            _rigidbody.AddTorque((Vector3) wantedDirection);
        }

        if (Input.GetKey("space")) {
            // Acceleration
            _rigidbody.AddForce(transform.forward * velocity, ForceMode.Impulse);
        }
    }
}