using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadModule : MonoBehaviour
{
    public GameObject mainBike;
    public GameObject engineModule;
    public GameObject wheelModule;

    private GameObject currentModule;

    public void LoadBikeModule(int moduleIndex)
    {
        // Unload the previous module if one is already loaded
        UnloadBikeModule();

        GameObject moduleToLoad = null;

        if (moduleIndex == 0)
        {
            moduleToLoad = engineModule;
        }
        else if (moduleIndex == 1)
        {
            moduleToLoad = wheelModule;
        }

        if (moduleToLoad != null)
        {
            // Duplicate the module (instantiate a new instance)
            currentModule = Instantiate(moduleToLoad);

            // Set it as a child of the bike (optional: for organization)
            currentModule.transform.parent = transform;

            currentModule.transform.localScale = Vector3.one; // Reset position to avoid offset issues
            currentModule.transform.localPosition = Vector3.zero; // Reset position to avoid offset issues
            currentModule.transform.localRotation = Quaternion.identity; // Reset rotation to avoid offset issues
            // Make sure the module is active
            currentModule.SetActive(true);
            mainBike.SetActive(false);
        }
    }

    public void UnloadBikeModule()
    {
        if (currentModule != null)
        {
            Destroy(currentModule);
            currentModule = null;
            mainBike.SetActive(true);
        }
    }
}
