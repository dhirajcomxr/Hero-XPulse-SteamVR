using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPoint : MonoBehaviour
{
    public Transform attachPoint;
    public Transform attachedObject;
    
    public bool isAttached;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(AnimHash.TOOL) && !isAttached)
        {
            other.GetComponent<Rigidbody>().isKinematic = true;
            other.transform.parent = attachPoint;
            other.transform.localEulerAngles = Vector3.one;
            other.transform.localPosition = Vector3.zero;
            attachedObject = other.transform;
            isAttached = true;
        }
        if (other.CompareTag(AnimHash.TOOL) && isAttached)
        {
            if (attachedObject.localPosition.magnitude > 0)
            {
                isAttached = false;
                attachedObject = null;
            }
            
        }
    }
    


}
