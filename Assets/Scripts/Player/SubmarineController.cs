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
        [SerializeField] private GameEvent bumpEvent;
        [SerializeField] private GameEvent outOfBoundsEvent;
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
        }

        private void echolocation() {
            echolocationEvent.Raise();
        }

        private void OnEnable() {
            controls?.Enable();
        }

        private void OnDisable() {
            controls?.Disable();
        }

        private void OnCollisionEnter(Collision target) {
            if (!target.gameObject.CompareTag("Bounds")) { 
                bumpEvent.Raise(); 
            }
        }

        private void OnTriggerEnter(Collider target) {
            if (target.CompareTag("Bounds")) {
                outOfBoundsEvent.sentBool = true;
                outOfBoundsEvent.Raise();
            }
        }
        private void OnTriggerExit(Collider target) {
            if (target.CompareTag("Bounds")){
                outOfBoundsEvent.sentBool = false;
                outOfBoundsEvent.Raise();
            }
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
            controls.Player.Echolocation.started += context => echolocation();
            var zeroDepthY = referenceZeroDepth.position.y;
            playerData.maxDepth = zeroDepthY - referenceMaxDepth.position.y;
            _isGoingDown = false;
            _isAccelerating = false;
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.maxAngularVelocity = Mathf.Deg2Rad * playerData.angularVelocity;
            _sqrMaxVelocity = playerData.maxVelocity * playerData.maxVelocity;
            _lastSentDepth = zeroDepthY;
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
            var newDepth = Math.Max(referenceZeroDepth.position.y - transform.position.y, 0);
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
            }

            if (_currentMove.x != 0) {
                // left dir
                _rigidbody.AddRelativeTorque(
                    Vector3.up * (_currentMove.x * playerData.angularVelocity * Time.deltaTime));
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