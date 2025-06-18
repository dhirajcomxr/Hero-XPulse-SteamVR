using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolRotationInteraction : MonoBehaviour
{
    public UnityEvent executeOnRotationComplete;
    public XRGrabInteractable xRGrabInteractable;
    public Transform xRController;
    public bool isAttached = false;
    public bool isToolInteracting = false;

    public Transform t_HandleGfx;

    public Vector3 controllerRotation;
    public float rotationSpeed = 50f;   // Adjust rotation sensitivity
    public int requireRotation = 3;
    public bool isClockwiseStep = true;

    private float previousYaw;          // Previous Yaw angle of controller
    private bool isFirstFrame = true;

    private float totalZRotation = 0f;  // Accumulated rotation in degrees
    private int fullRotations = 0;       // Full 360-degree turns
    private float lastZ;                // Last Z angle of handle

    void Start()
    {
        lastZ = t_HandleGfx.localEulerAngles.z;
    }

    void Update()
    {
        CheckRotation();
        CountRotations();
    }

    public void AttachToComponent(bool isTrue)
    {
        if (isTrue)
        {
            LeanTween.delayedCall(0.1f, () =>
            {
                xRGrabInteractable.enabled = false;
                isAttached = true;
            });
        }
        else
        {
            isAttached = false;
        }
    }

    public void CheckRotation()
    {
        if (!isAttached && !isToolInteracting) return;

        if (xRController)
        {
            float currentYaw = -xRController.localEulerAngles.z;

            if (isFirstFrame)
            {
                previousYaw = currentYaw;
                isFirstFrame = false;
                return;
            }

            float deltaYaw = Mathf.DeltaAngle(previousYaw, currentYaw);

            if ((isClockwiseStep && deltaYaw > 0) || (!isClockwiseStep && deltaYaw < 0))
            {
                float rotationAmount = -deltaYaw * Time.deltaTime * rotationSpeed;

                // ✅ Rotate on Z-axis
                t_HandleGfx.Rotate(0f, 0f, rotationAmount);

                // ✅ Move on Z-axis (local space)
                float moveDirection = -Mathf.Sign(rotationAmount); // +1 or -1
                float moveAmount = moveDirection * Time.deltaTime * 0.01f; // 🔧 adjust 0.01f as needed
                t_HandleGfx.Translate(0f, 0f, moveAmount, Space.Self);
            }

            previousYaw = currentYaw;
        }
    }

    private void CountRotations()
    {
        float currentZ = t_HandleGfx.localEulerAngles.z;
        float deltaZ = Mathf.DeltaAngle(lastZ, currentZ);

        totalZRotation += deltaZ;
        lastZ = currentZ;

        // Count full directional rotations
        fullRotations = Mathf.Abs(Mathf.FloorToInt(totalZRotation / 360f));
        if (fullRotations >= requireRotation)
        {
            //executeOnRotationComplete?.Invoke();
            Steps steps = FindObjectOfType<Steps>();

            if (steps && steps.gameObject.activeInHierarchy)
            {
                steps.userToolsInteraction();
            }
            // Just to test
            requireRotation = 9999;
            //
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("HandController") && isAttached)
        {
            isToolInteracting = true;
            xRController = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        isToolInteracting = false;
        xRController = null;
        isFirstFrame = true; // Reset to prevent large jump when re-entering
    }

    public void DebugMassage(string data)
    {
        Debug.Log(data);
    }
}
