using System;
using InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
    [RequireComponent(typeof(Rigidbody))]
    [ExecuteInEditMode]
    public class SubmarineController : MonoBehaviour {
        private Rigidbody _rigidbody;
        [SerializeField] private float angularVelocity;
        [SerializeField] private float velocity;
        [SerializeField] private float maxVelocity;
        [SerializeField] private GameEvent bubbleEvent;
        [SerializeField] private GameEvent echolocationEvent;
        [SerializeField] private GameEvent depthEvent;
        [SerializeField] private float minChangeDepth = 0.5f;
        [SerializeField] private Transform referenceZeroDepth;

        private bool _isGoingDown;

        // private float _depth;
        private float _lastSentDepth;
        private float _sqrMaxVelocity;
        private SubmarineInput controls;
        private Vector3 _currentMove;

        private void Awake() {
            controls = new SubmarineInput();
            controls.Player.Echolocation.started += context => echolocation();
        }

        private void echolocation() {
            print("echolocation");
            echolocationEvent.Raise();
            // todo ask for location of target
            // todo for enemies: listen to echolocation event, and find my position/direction
        }

        private void OnEnable() {
            controls?.Enable();
        }

        private void OnDisable() {
            controls?.Disable();
        }

        private void OnValidate() {
            setMaxVelocity(maxVelocity);
        }

        private void setMaxVelocity(float value) {
            maxVelocity = value;
            _sqrMaxVelocity = maxVelocity * maxVelocity;
        }

        // Start is called before the first frame update
        private void Start() {
            _isGoingDown = false;
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.maxAngularVelocity = Mathf.Deg2Rad * angularVelocity;
            _sqrMaxVelocity = maxVelocity * maxVelocity;
            _lastSentDepth = referenceZeroDepth.position.y;
        }

        public void Move(InputAction.CallbackContext context) {
            // This returns Vector2.zero when context.canceled
            // is true, so no need to handle these separately.
            _currentMove = context.ReadValue<Vector2>();
        }
        private void FixedUpdate() {
            if (_rigidbody.velocity.sqrMagnitude > _sqrMaxVelocity) {
                // clamp velocity
                _rigidbody.velocity = _rigidbody.velocity.normalized * maxVelocity;
            }

            // Is the submarine going down? 
            var velocityY = Vector3.Project(_rigidbody.velocity, Vector3.up).y;

            if (velocityY < -float.Epsilon) {
                if (!_isGoingDown) {
                    bubbleEvent.sentBool = true;
                    bubbleEvent.Raise();
                    _isGoingDown = true;
                }
            } else {
                if (_isGoingDown) {
                    bubbleEvent.sentBool = false;
                    bubbleEvent.Raise();
                    _isGoingDown = false;
                }
            }

            // depth of submarine?
            var newDepth = referenceZeroDepth.position.y - transform.position.y;
            if (Math.Abs(newDepth - _lastSentDepth) > minChangeDepth) {
                _lastSentDepth = newDepth;
                depthEvent.sentFloat = newDepth;
                depthEvent.Raise();
            }

            // Add a rotational force
            if (_currentMove.y != 0) {
                // up direction
                _rigidbody.AddRelativeTorque(Vector3.left * (_currentMove.y * angularVelocity * Time.deltaTime));
                print("upDir: " + _currentMove.y);
            }

            if (_currentMove.x != 0) {
                // left dir
                _rigidbody.AddRelativeTorque(Vector3.up * (_currentMove.x * angularVelocity * Time.deltaTime));
                print("leftDir: " + _currentMove.x);
            }

            // Accelerate
            if (Keyboard.current.spaceKey.isPressed) {
                // Acceleration
                _rigidbody.AddForce(transform.forward * velocity, ForceMode.Impulse);
            }

            // do not allow z rotations!
            var transform1 = transform;
            var localEulerAngles = transform1.localEulerAngles;
            localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y, 0);
            transform1.localEulerAngles = localEulerAngles;
        }
    }
}