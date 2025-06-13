using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EPOOutline;

public class outlineData : MonoBehaviour
{
    public enum BikePart { FrontWheel, RearWheel, FrontBrake, RearBrake, Engine, Transmission, Suspension };
    BikePart HighlightedPart = new BikePart();
    [System.Serializable]
    public class outlineObject
    {
        public enum BikePart { FrontWheel, RearWheel, FrontBrake, RearBrake, Engine, Transmission, Suspension };
        public BikePart PartName = new BikePart();
        public GameObject[] highlightedPart;
    }
    public outlineObject[] outlineEnableObject;
  //  public GameObject[] dIYswitchObject;

    private void Start()
    {
        DisableOutline();
    }
    public void DisableOutline()
    {
        for (int i = 0; i < outlineEnableObject.Length; i++)
        {
            foreach (GameObject g in outlineEnableObject[i].highlightedPart)
            {
                Debug.LogError("Game Obejct Name " + g.name);
                if (g.GetComponent<Outlinable>().enabled)
                    g.GetComponent<Outlinable>().enabled = false;
            }
        }
    }

    public void switchHighlightedPart(BikePart part)
    {
        Debug.Log("Outlinable Part Name " + part);
        for (int i = 0; i < outlineEnableObject.Length; i++)
        {
            if (part.ToString() == outlineEnableObject[i].PartName.ToString())
            {
                foreach (GameObject g in outlineEnableObject[i].highlightedPart)
                {
                    if (g.GetComponent<Outlinable>().enabled)
                        g.GetComponent<Outlinable>().enabled = true;
                }
            }
            else
            {
                foreach (GameObject g in outlineEnableObject[i].highlightedPart)
                {
                    if (g.GetComponent<Outlinable>().enabled)
                        g.GetComponent<Outlinable>().enabled = false;
                }
            }
        }
    }
}
