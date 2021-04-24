using UnityEngine;

public class SubmarineController : MonoBehaviour {
    private Rigidbody _rigidbody;
    [SerializeField] private float angularVelocity;
    [SerializeField] private float speed;

    // Start is called before the first frame update
    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update() {
        Vector3? wantedDirection = null;
        if (Input.GetKey("up")) {
            // We want to go upward
            wantedDirection = Quaternion.AngleAxis(-angularVelocity * Time.deltaTime, Vector3.right) *
                              transform.forward;
        } else if (Input.GetKey("down")) {
            // We want to go upward
            wantedDirection = Quaternion.AngleAxis(angularVelocity * Time.deltaTime, Vector3.up) * transform.forward;
        }

        print("wanted Dir" + wantedDirection);
        if (wantedDirection == null) return; // no instruction, do nothing
        var newRotation = Quaternion.LookRotation((Vector3) wantedDirection);
        print("newRot" + newRotation);
        
        _rigidbody.AddTorque((Vector3) wantedDirection, ForceMode.Impulse);
        // _rigidbody.MoveRotation(newRotation);
        _rigidbody.AddForce(transform.forward * (speed ), ForceMode.Force);
    }
}