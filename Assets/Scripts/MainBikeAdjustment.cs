using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BikePart
{
    public GameObject part;
    public Vector3 partPos;
}
public class MainBikeAdjustment : MonoBehaviour
{

    public List<BikePart> bikeParts = new List<BikePart>();

    public void GetTheList()
    {

    }

    public void Start()
    {

    }
    //public void AddGameObject(GameObject obj, Vector3 position)
    //{
    //    if (!BikePart.ContainsKey(obj))
    //    {
    //        BikePart.Add(obj, position);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("The game object is already added to the dictionary.");
    //    }
    //}
    //public Vector3 GetPosition(GameObject obj)
    //{
    //    Vector3 position = Vector3.zero;
    //    if (BikePart.ContainsKey(obj))
    //    {
    //        position = BikePart[obj];
    //    }
    //    else
    //    {
    //        Debug.LogWarning("The game object is not found in the dictionary.");
    //    }
    //    return position;
    //}

    public void UpdatePosition(GameObject obj, Vector3 newPosition)
    {
        // a foreach loop is required

    }

    //public void RemoveGameObject(GameObject obj)
    //{
    //    if (BikePart.ContainsKey(obj))
    //    {
    //        BikePart.Remove(obj);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("The game object is not found in the dictionary.");
    //    }
    //}
}
