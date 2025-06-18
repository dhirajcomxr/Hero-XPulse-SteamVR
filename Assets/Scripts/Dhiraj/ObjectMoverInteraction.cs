using UnityEngine;

public class ObjectMoverInteraction : MonoBehaviour
{    public Transform xRController;            // XR Controller reference
    public float moveSpeed = 0.05f;           // How fast the object moves
    public bool isPositiveDirection = true;   // Allow movement in +X (true) or -X (false)
    public float targetLocalX = 1f;           // Target local X position

    private bool isInteracting = false;
    private bool hasReachedTarget = false;

    public Vector3 currentControllerPos;
    private Vector3 previousControllerPosition;

    void Update()
    {
        if (isInteracting && xRController && !hasReachedTarget)
        {
            currentControllerPos = xRController.localPosition;
            DetectAndMoveByControllerMotion();
            CheckTargetReached();
        }
    }

    private void DetectAndMoveByControllerMotion()
    {
        Vector3 currentControllerPosition = xRController.localPosition;
        float deltaX = currentControllerPosition.x - previousControllerPosition.x;

        // Move based on direction setting
        if (isPositiveDirection && deltaX > 0.001f)
        {
            MoveObject(+1f);
        }
        else if (!isPositiveDirection && deltaX < -0.001f)
        {
            MoveObject(-1f);
        }

        previousControllerPosition = currentControllerPosition;
    }

    private void MoveObject(float direction)
    {
        Vector3 movement = new Vector3(direction * moveSpeed * Time.deltaTime, 0f, 0f);
        transform.Translate(movement, Space.Self);
    }

    private void CheckTargetReached()
    {
        float currentX = transform.localPosition.x;

        if ((isPositiveDirection && currentX >= targetLocalX) ||
            (!isPositiveDirection && currentX <= targetLocalX))
        {
            hasReachedTarget = true;
             Steps steps = FindObjectOfType<Steps>();

            if (steps && steps.gameObject.activeInHierarchy)
            {
                steps.userToolsInteraction();                
            }
            Debug.Log("🎯 Target X position reached!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("HandController"))
        {
            xRController = other.transform;
            isInteracting = true;
            previousControllerPosition = xRController.position; // Initialize reference
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HandController"))
        {
            isInteracting = false;
            xRController = null;
        }
    }
}
