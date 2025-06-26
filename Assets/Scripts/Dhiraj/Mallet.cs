using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mallet : MonoBehaviour
{
    //public UnityEvent executeOnRotationComplete;
    public ToolRotationInteraction tool;
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
        rotatingArrow.isAssembly = isClockwiseStep;
        ResetEverythingOnEnable();
    }

    public Collider boxCol;
    public void ResetEverythingOnEnable()
    {
        boxCol.enabled = true;
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
        transform.localEulerAngles = initialRotationTool;      
        isAttached = false;
        fullRotations = 0;
        rotatingArrow.gameObject.SetActive(false);
        attachPointGFX.SetActive(true);
        if (Outlinable) Outlinable.enabled = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Impact Wrench"))
        {
            if (!tool.isAttached) return;
            boxCol.enabled = false;
            tool.RotateLockedItem();
        }
    }
}
