using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepostionGrabbleObject : MonoBehaviour
{
    private Vector3 myPosition;
    private Vector3 myRotation;



    private void Start()
    {
        myPosition = transform.position;
        myRotation = transform.eulerAngles;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
        {
            transform.position = myPosition;
            transform.eulerAngles = myRotation;
            Debug.Log("Debug");
        }
        else
        {
            Debug.Log("Debug 1");
        }
        
    }



}
