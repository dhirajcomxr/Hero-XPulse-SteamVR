using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetToolPositions : MonoBehaviour
{
    public Transform parant;
    public Vector3 initialPos, initialRot;
    private void Awake()
    {
    }
   
    public void ResetTool()
    {
        transform.parent = parant;
        transform.localPosition = initialPos;
        transform.localEulerAngles = initialRot;
    }
}
