using UnityEngine;

public class ObjectMoverInteraction : MonoBehaviour
{
    public Transform xRController;            // XR Controller reference
    public float moveSpeed = 0.05f;           // How fast the object moves
    public bool isPositiveDirection = true;   // Allow movement in +X (true) or -X (false)
    public float targetLocalX = 1f;           // Target local X position
    public Transform gfx;                     // The transform to rotate on Z-axis

    private bool isInteracting = false;
    private bool hasReachedTarget = false;

    public Vector3 currentControllerPos;
    private Vector3 previousControllerPosition;

    void Update()
    {
        if (isInteracting && xRController)
        {
            currentControllerPos = xRController.localPosition;

            if (!hasReachedTarget)
            {
                DetectAndMoveByControllerMotion();
                CheckTargetReached();
            }

            UpdateRotation(); // Always update rotation based on position
        }
    }

    private void DetectAndMoveByControllerMotion()
    {
        Vector3 currentControllerPosition = xRController.localPosition;
        float deltaX = currentControllerPosition.x - previousControllerPosition.x;

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
        else
        {
            hasReachedTarget = false;
        }
    }

    /*private void UpdateRotation()
    {
        if (!gfx) return;

        float currentX = transform.localPosition.x;
        float t;

        if (isPositiveDirection)
        {
            t = Mathf.InverseLerp(0f, targetLocalX, currentX); // Moving 0 → target
            t = Mathf.Clamp01(t);
            gfx.localEulerAngles = new Vector3(
                gfx.localEulerAngles.x,
                gfx.localEulerAngles.y,
                Mathf.Lerp(0f, -30f, t) // Rotate 0 → -30
            );
        }
        else
        {
            t = Mathf.InverseLerp(targetLocalX, 0f, currentX); // Moving target → 0
            t = Mathf.Clamp01(t);
            gfx.localEulerAngles = new Vector3(
                gfx.localEulerAngles.x,
                gfx.localEulerAngles.y,
                Mathf.Lerp(-30f, 0f, t) // Rotate -30 → 0
            );
        }
    }*/

    private void UpdateRotation()
    {
        if (!gfx) return;

        float currentX = transform.localPosition.x;
        float t;

        if (isPositiveDirection)
        {
            t = Mathf.InverseLerp(0f, targetLocalX, currentX);
            t = Mathf.Clamp01(t);

            Quaternion fromRotation = Quaternion.Euler(gfx.localEulerAngles.x, gfx.localEulerAngles.y, 0f);
            Quaternion toRotation = Quaternion.Euler(gfx.localEulerAngles.x, gfx.localEulerAngles.y, -30f);
            gfx.localRotation = Quaternion.Lerp(fromRotation, toRotation, t);
        }
        else
        {
            t = Mathf.InverseLerp(targetLocalX, 0f, currentX);
            t = Mathf.Clamp01(t);

            Quaternion fromRotation = Quaternion.Euler(gfx.localEulerAngles.x, gfx.localEulerAngles.y, -30f);
            Quaternion toRotation = Quaternion.Euler(gfx.localEulerAngles.x, gfx.localEulerAngles.y, 0f);
            gfx.localRotation = Quaternion.Lerp(fromRotation, toRotation, t);
        }

        Debug.Log($"[Rotation] isPositive: {isPositiveDirection}, currentX: {currentX}, t: {t}, Z: {gfx.localEulerAngles.z}");
    }






    public bool isInteractingLeft = false;
    public bool isInteractingRight = false;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("HandController1"))
        {
            isInteractingLeft = true;
        }

        if (other.CompareTag("HandController") && isInteractingLeft)
        {
            xRController = other.transform;
            isInteracting = isInteractingRight = true;
            previousControllerPosition = xRController.localPosition;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HandController"))
        {
            isInteractingRight = false;
            isInteracting = false;
            xRController = null;
        }
        if (other.CompareTag("HandController1"))
        {
            isInteractingLeft = false;
            isInteracting = false;
            xRController = null;
        }

    }
}
