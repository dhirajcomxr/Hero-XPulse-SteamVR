using UnityEngine;

public class WheelRotationZ : MonoBehaviour {

    public Transform wheel;
    public float rotationSpeed = 270f; // degrees per unit moved

    private Vector3 lastPosition;

    void Start() {
        lastPosition = transform.position;
    }

    void Update() {
        Vector3 movement = transform.position - lastPosition;

        if (Mathf.Abs(movement.x) > 0.0001f) {
            float direction = movement.x > 0 ? -1f : 1f;
            float rotationAmount = Mathf.Abs(movement.x) * rotationSpeed;

            wheel.Rotate(Vector3.forward, rotationAmount * direction);
        }

        lastPosition = transform.position;
    }
}
