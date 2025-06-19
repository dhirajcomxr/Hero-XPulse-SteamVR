using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ToolAndAttachPoint
{
    public string moduleName;
    public List<GameObject> AttachPoints = new List<GameObject>();
    public List<ToolRotationInteraction> Tools = new List<ToolRotationInteraction>();
}
public class LoadModule : MonoBehaviour
{
    public GameObject mainBike;
    public GameObject[] engineModule;
    public GameObject[] wheelModule;

    private GameObject currentModule;
    public ToolAndAttachPoint[] toolAndAttachPoints;

    int toolModuleIndex = 0;
    GameObject[] moduleToLoad;
    public void LoadBikeModule(int moduleIndex)
    {
        // Unload the previous module if one is already loaded
        toolModuleIndex = moduleIndex;
        UnloadBikeModule();

        moduleToLoad = null;

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
            currentModule = Instantiate(moduleToLoad[0]);

            // Set it as a child of the bike (optional: for organization)
            currentModule.transform.parent = transform;

            currentModule.transform.localScale = Vector3.one; // Reset position to avoid offset issues
            currentModule.transform.localPosition = Vector3.zero; // Reset position to avoid offset issues
            currentModule.transform.localRotation = Quaternion.identity; // Reset rotation to avoid offset issues
            // Make sure the module is active
            currentModule.SetActive(true);
            moduleToLoad[1].SetActive(true);
            mainBike.SetActive(false);
        }
    }

    public void UnloadBikeModule()
    {
        if (currentModule != null)
        {
            DestroyImmediate(currentModule);
            moduleToLoad[1].SetActive(false);
            currentModule = null;
            for (int i = 0; i < toolAndAttachPoints[toolModuleIndex].AttachPoints.Count - 1; i++)
            {
                toolAndAttachPoints[toolModuleIndex].AttachPoints[i].SetActive(false);
                toolAndAttachPoints[toolModuleIndex].Tools[i].ResetEverythingOnEnable();
                toolAndAttachPoints[toolModuleIndex].Tools[i].gameObject.SetActive(false);
            }
            mainBike.SetActive(true);
        }
    }
}
