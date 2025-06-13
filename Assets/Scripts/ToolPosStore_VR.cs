using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolPosStore_VR : MonoBehaviour
{
    public GameObject toolpos;

    public Transform toolposdef;
    public Transform parentPos_def;

    private void Awake()
    {
        toolposdef = this.toolpos.transform;
        parentPos_def = this.gameObject.transform;
    }
    public void resetPos()
    {
      /*  this.gameObject.transform.position = parentPos_def.position;
        this.gameObject.transform.localRotation = parentPos_def.localRotation;*/


        this.toolpos.transform.position = toolposdef.transform.position;
        this.toolpos.transform.localRotation = toolposdef.transform.localRotation;
    }
}
