using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleUI : MonoBehaviour
{
    public Image completed;
    public Image current;


    public void IsOn(bool isComplete = false)
    {
        if (!isComplete)
        {
            
            completed.enabled = false;
            current.enabled = true;
        }
        else
        {
            
            completed.enabled = true;
            current.enabled = false;
        }

    }
}
