using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolRotationInteraction : MonoBehaviour
{
    public UnityEvent RotationCompleted;
    public float requiredRotationDegrees = 720f; // 2 full turns
    public bool isFullyRotated = false;

    public XRBaseInteractor currentInteractor;
    public float totalRotation = 0f;
    public float lastZRotation = 0f;
    public bool isTracking = false;

    // private void OnEnable()
    // {
    //     var grabInteractable = GetComponent<XRGrabInteractable>();
    //     grabInteractable.selectEntered.AddListener(OnGrab);
    //     grabInteractable.selectExited.AddListener(OnRelease);
    // }

    // private void OnDisable()
    // {
    //     var grabInteractable = GetComponent<XRGrabInteractable>();
    //     grabInteractable.selectEntered.RemoveListener(OnGrab);
    //     grabInteractable.selectExited.RemoveListener(OnRelease);
    // }

    private void OnGrab(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject.transform.GetComponent<XRBaseInteractor>();
        if (currentInteractor != null)
        {
            lastZRotation = currentInteractor.attachTransform.eulerAngles.z;
            totalRotation = 0f;
            isFullyRotated = false;
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isTracking = false;
        currentInteractor = null;
    }

    void Update()
    {
        if (!isTracking || currentInteractor == null || isFullyRotated)
            return;
             
        float currentZRotation = currentInteractor.attachTransform.eulerAngles.z;
        float deltaRotation = Mathf.DeltaAngle(lastZRotation, currentZRotation);
        lastZRotation = currentZRotation;

        totalRotation += deltaRotation;

        if (!isFullyRotated)
        {
            if (totalRotation >= requiredRotationDegrees)
            {
                isFullyRotated = true;
                Debug.Log("🔧 Tool fully rotated clockwise!");
                RotationCompleted?.Invoke();
            }
            else if (totalRotation <= -requiredRotationDegrees)
            {
                isFullyRotated = true;
                Debug.Log("🔧 Tool fully rotated anticlockwise!");
                RotationCompleted?.Invoke();
            }
        }
    }


    public void ActivateTracking(bool isTrack)
    {
        isTracking = isTrack;
    }
}
