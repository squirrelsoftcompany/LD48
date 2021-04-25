using System;
using InputSystem;
using Settings;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
    [RequireComponent(typeof(Rigidbody))]
    [ExecuteInEditMode]
    public class SubmarineController : MonoBehaviour {
        private Rigidbody _rigidbody;
        [SerializeField] private GameEvent bubbleEvent;
        [SerializeField] private GameEvent echolocationEvent;
        [SerializeField] private GameEvent depthEvent;
        [SerializeField] private GameEvent accelerationEvent;
        [SerializeField] private Transform referenceZeroDepth;
        [SerializeField] private Transform referenceMaxDepth;
        [SerializeField] private PlayerData playerData;

        private bool _isGoingDown, _isAccelerating;

        // private float _depth;
        private float _lastSentDepth;
        private float _sqrMaxVelocity;
        private SubmarineInput controls;
        private Vector3 _currentMove;

        private void Awake() {
            controls = new SubmarineInput();
            controls.Player.Echolocation.started += context => echolocation();
            playerData.maxDepth = referenceZeroDepth.position.y - referenceMaxDepth.position.y;
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
            setMaxVelocity(playerData.maxVelocity);
        }

        private void setMaxVelocity(float value) {
            playerData.maxVelocity = value;
            _sqrMaxVelocity = playerData.maxVelocity * playerData.maxVelocity;
        }

        // Start is called before the first frame update
        private void Start() {
            _isGoingDown = false;
            _isAccelerating = false;
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.maxAngularVelocity = Mathf.Deg2Rad * playerData.angularVelocity;
            _sqrMaxVelocity = playerData.maxVelocity * playerData.maxVelocity;
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
                _rigidbody.velocity = _rigidbody.velocity.normalized * playerData.maxVelocity;
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
            if (Math.Abs(newDepth - _lastSentDepth) > playerData.minChangeDepth) {
                _lastSentDepth = newDepth;
                depthEvent.sentFloat = newDepth;
                depthEvent.Raise();
            }

            // Add a rotational force
            if (_currentMove.y != 0) {
                // up direction
                _rigidbody.AddRelativeTorque(Vector3.left *
                                             (_currentMove.y * playerData.angularVelocity * Time.deltaTime));
                print("upDir: " + _currentMove.y);
            }

            if (_currentMove.x != 0) {
                // left dir
                _rigidbody.AddRelativeTorque(
                    Vector3.up * (_currentMove.x * playerData.angularVelocity * Time.deltaTime));
                print("leftDir: " + _currentMove.x);
            }

            // Accelerate
            if (Keyboard.current.spaceKey.isPressed) {
                // Acceleration
                _rigidbody.AddForce(transform.forward * playerData.velocity, ForceMode.Impulse);
                if (!_isAccelerating) {
                    _isAccelerating = true;
                    accelerationEvent.sentBool = true;
                    accelerationEvent.Raise();
                }
            } else if (_isAccelerating) {
                _isAccelerating = false;
                accelerationEvent.sentBool = false;
                accelerationEvent.Raise();
            }

            // do not allow z rotations!
            var transform1 = transform;
            var localEulerAngles = transform1.localEulerAngles;
            localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y, 0);
            transform1.localEulerAngles = localEulerAngles;
        }
    }
}