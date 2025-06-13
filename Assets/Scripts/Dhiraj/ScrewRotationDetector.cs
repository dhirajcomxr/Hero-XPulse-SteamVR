using UnityEngine;

public class ScrewRotationDetector : MonoBehaviour
{
    public Transform toolTransform;  // screwdriver or handle
    public float rotationThreshold = 720f; // Degrees for 2 full turns
    public bool isFullyRotated = false;

    private float totalRotation = 0f;
    private float lastAngle = 0f;
    private bool isToolAttached = false;

    void Update()
    {
        if (!isToolAttached || isFullyRotated) return;

        float currentAngle = toolTransform.localEulerAngles.z;

        // Handle angle wrapping (360 -> 0)
        float deltaAngle = Mathf.DeltaAngle(lastAngle, currentAngle);
        totalRotation += Mathf.Abs(deltaAngle);
        lastAngle = currentAngle;

        if (totalRotation >= rotationThreshold)
        {
            isFullyRotated = true;
            Debug.Log("Screw fully rotated!");
        }
    }

    public void AttachTool(Transform tool)
    {
        toolTransform = tool;
        lastAngle = tool.localEulerAngles.z;
        totalRotation = 0f;
        isFullyRotated = false;
        isToolAttached = true;
    }

    public void DetachTool()
    {
        isToolAttached = false;
    }
}
