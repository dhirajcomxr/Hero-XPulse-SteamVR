using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolRotationInteraction : MonoBehaviour
{
    //public UnityEvent executeOnRotationComplete;
    public bool isLocked = false;
    public bool isSpanner = false;
    public XRGrabInteractable xRGrabInteractable;
    public Transform xRController;
    public ObjectRotation rotatingArrow;
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

    [Space(5), Header("Reset Positions")]
    public Transform parantTransform;
    public Vector3 initialPositionTool;
    public Vector3 initialRotationTool;
    public GameObject attachPointGFX;

    public EPOOutline.Outlinable Outlinable;
    private void Awake()
    {
        Outlinable = GetComponent<EPOOutline.Outlinable>();
    }
    void OnEnable()
    {
        if (!isLocked) rotatingArrow.isAssembly = isClockwiseStep;
        ResetEverythingOnEnable();
    }
    void Start()
    {
        lastZ = t_HandleGfx.localEulerAngles.z;
    }

    void Update()
    {
        if(!isLocked) CheckRotation();
        if(!isLocked) CountRotations();

    }


    public void AttachToComponent(bool isTrue)
    {
        if (isTrue)
        {
            LeanTween.delayedCall(0.1f, () =>
            {
                xRGrabInteractable.enabled = false;
                isAttached = true;
               if(!isLocked) rotatingArrow.gameObject.SetActive(true);
            });
        }
        else
        {
            isAttached = false;
            rotatingArrow.gameObject.SetActive(false);
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
                //t_HandleGfx.Translate(0f, 0f, moveAmount, Space.Self);
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
        fullRotations = Mathf.Abs(Mathf.FloorToInt(totalZRotation / 60f));
        if (fullRotations >= requireRotation)
        {
            //executeOnRotationComplete?.Invoke();
            Steps steps = FindObjectOfType<Steps>();

            if (steps && steps.gameObject.activeInHierarchy)
            {
                steps.userToolsInteraction();
                ResetTool();
                Debug.Log("Error is here");
            }
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

    public Collider[] thisColliders;
    public void RotateLockedItem()
    {
        if (!isLocked) return;
        rotatingArrow.isAssembly = !isClockwiseStep;
        rotatingArrow.gameObject.SetActive(true);
        float rotationAngle = isClockwiseStep ? 90f : -90f; // Rotate 90 degrees either direction
        float duration = 3f;

        LeanTween.rotateAround(t_HandleGfx.gameObject, Vector3.forward, rotationAngle, duration)
                 .setEase(LeanTweenType.easeInOutSine)
                 .setOnComplete(() => {
                     Steps steps = FindObjectOfType<Steps>();

                     if (steps && steps.gameObject.activeInHierarchy)
                     {
                         steps.userToolsInteraction();
                         ResetTool();
                         rotatingArrow.gameObject.SetActive(false);
                         Debug.Log("Error is here");
                     }
                 });
    }
    public void ResetTool()
    {
        this.gameObject.SetActive(true);
        t_HandleGfx.localEulerAngles = Vector3.zero;
        isToolInteracting = false;
        xRController = null;
        transform.parent = parantTransform;
        transform.localPosition = initialPositionTool;
        transform.localEulerAngles = initialRotationTool;
        xRGrabInteractable.enabled = true;
        isAttached = false;
        if (Outlinable) Outlinable.enabled = false;
        foreach (var item in thisColliders)
        {
            item.enabled = true;
        }
    }

    public void ResetEverythingOnEnable()
    {
        previousYaw = 0;
        totalZRotation = 0;
        lastZ = 0;
        isFirstFrame = true;
        t_HandleGfx.localEulerAngles = Vector3.zero;
        isToolInteracting = false;
        xRController = null;
        transform.parent = parantTransform;
        transform.localPosition = initialPositionTool;
        transform.localEulerAngles = initialRotationTool;
        xRGrabInteractable.enabled = true;
        isAttached = false;
        fullRotations = 0;
        rotatingArrow.gameObject.SetActive(false);
        attachPointGFX.SetActive(true);
        if (Outlinable) Outlinable.enabled = true;
        foreach (var item in thisColliders)
        {
            item.enabled = true;
        }
    }
}
