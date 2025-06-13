using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class StepsManager : MonoBehaviour {

    #region PUBLIC_VARS
    private static StepsManager _instance;
    public static StepsManager Instance { get { return _instance; } }

    public Text stepDesc;
    public Text StepNumber;
    [ColorUsage(true,true)]
    public Color highlightColor;
  /*  public GameObject fixedCamera;
    public GameObject mainCamera;
    public GameObject freeCamPivot;*/
    public GameObject caution_popup;
    public Text Caution_Text;
    public GameObject cautionHeader;
    public GameObject cautionLine;
   
    public GameObject toolPopup;
    public Text toolName_txt;
    //public Sprite defaultToolSprite;
    public Image toolImage;
    public GameObject torquePopup;
    public Text torqueValueTxt;
    public GameObject oneTimeUse;
    public GameObject infoPanel;
    public GameObject nextBtn;
    public GameObject prevBtn;

    public GameObject restartBtnContainer;
    public Steps existingStepScripts;
    public GameObject completeProccessTask;
    public GameObject VR_UI;

    public string stepCSV_filePath="";
    public GameObject divideingLineForStepList;
    public GameObject loadingScreen;
    public Material modelTransparent;
    public Material modelTrackHighLighted;
    public List<GameObject> NewLineRopes = new List<GameObject>();
    float NewLineRopesRadius;
    public GameObject[] vr_Tools;
    public AudioSource audioSource;
    public Transform toolSpwanPos;
    #endregion

    #region EVENTS
    public delegate void OnNextClick();
    public static event OnNextClick NextStep;

    public delegate void OnPrevClick();
    public static event OnPrevClick PreviousStep;

    public delegate void OnReplayClick();
    public static event OnReplayClick ReplayStep;
    #endregion

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        }
        else {
            _instance = this;
        }
        
    }
    bool SidePanelAnimState;
    public Animator CanvasAnim;
    public GameObject sidepanel;


   /* public void LineRopeInSteps()
    {
        foreach (Step st in existingStepScripts.steps)
        {
            if (st.curvedLineObjs.Length > 0)
            {
                for (int i = 0; i < st.curvedLineObjs.Length; i++)
                {
                    if (st.curvedLineObjs[i] != null)
                    {
                        GameObject newLineRope = new GameObject();
                        newLineRope.transform.localScale = Vector3.one;
                        newLineRope.AddComponent<CurvedLine3D>();
                        newLineRope.GetComponent<CurvedLine3D>().paths = st.curvedLineObjs[i].GetComponent<CurvedLine3D>().paths;
                        newLineRope.GetComponent<CurvedLine3D>().material = st.curvedLineObjs[i].GetComponent<CurvedLine3D>().material;
                        newLineRope.GetComponent<CurvedLine3D>().shape = st.curvedLineObjs[i].GetComponent<CurvedLine3D>().shape;
                        newLineRope.GetComponent<CurvedLine3D>().precision = st.curvedLineObjs[i].GetComponent<CurvedLine3D>().precision;
                        newLineRope.GetComponent<CurvedLine3D>().radius = st.curvedLineObjs[i].GetComponent<CurvedLine3D>().radius;
                        newLineRope.GetComponent<CurvedLine3D>().refresh_rate = 1;
                        newLineRope.GetComponent<CurvedLine3D>().NewMesh();
                       // newLineRope.transform.position = st.curvedLineObjs[i].transform.localPosition;
                     //   newLineRope.transform.rotation = Quaternion.Euler(st.curvedLineObjs[i].transform.rotation.x, st.curvedLineObjs[i].transform.rotation.y, st.curvedLineObjs[i].transform.rotation.z);
                        st.curvedLineObjs[i].GetComponent<MeshRenderer>().enabled = false;
                        NewLineRopes.Add(newLineRope);
                        NewLineRopesRadius = newLineRope.GetComponent<CurvedLine3D>().radius;
                    }
                }
            }
        }
    }*/

    public void clickOnNextOrPreviousButton()
    {
        if (Caution_Text.text == "")
        {
            if (sidepanel.GetComponent<RectTransform>().position.x== 0)
            {
                CanvasAnim.SetTrigger("RnRClose");
            }
        }
        else
        {
            if (sidepanel.GetComponent<RectTransform>().position.x != 0)
            {
                CanvasAnim.SetTrigger("RnROpen");
            }
        }
    }
    private void Update()
    {
        existingStepScripts = FindObjectOfType<Steps>();
        if (existingStepScripts!=null)
        {
          //  Debug.Log("Locate:" + existingStepScripts.currentStep + "Total  steps Count " + existingStepScripts.steps.Count);
        /*
            if (existingStepScripts.currentStep == existingStepScripts.steps.Count-1)
            {
               completeProccessTask.SetActive(true);
            }
            else
            {
                completeProccessTask.SetActive(false);
            }*/
        }
       
   /*     if(NewLineRopes.Count>0 && sceneObject.SelectedMode != SceneObjectOnOff.modes.ModelTracking)
        {
            float sizePercent = sceneObject.Bike.transform.localScale.x / 1;

           // NewLineRopesRadius = NewLineRopes[0].GetComponent<CurvedLine3D>().radius;

            foreach (GameObject item in NewLineRopes)
            {
                item.GetComponent<CurvedLine3D>().radius = NewLineRopesRadius * sizePercent;
            }


        }*/

    }
    public void PanRight()
    {
        //if (mainCamEx)
          //  mainCamEx.PanRight();
    }
    public Text description { get { return (stepDesc); } }

    public void OnNext() {
        NextStep?.Invoke();
    }

    public void OnReplay()
    {
        ReplayStep?.Invoke();
    }

    public void OnPrevious() {
        PreviousStep?.Invoke();
    }
 /*   public void resumeLaststep()
    {
        partInfoDIYsearchList partInfosearch = FindObjectOfType<partInfoDIYsearchList>();
        if (partInfosearch.isResumeModule==true)
        {
            Debug.Log(" Module Resume from last step   ");
            existingStepScripts.resume(PlayerPrefs.GetInt(partInfosearch.SelectedModule.moduleName));
        }
        else
        {
            Debug.Log(" Module start from beginning   ");
        }
        
    }*/
  /*  public void resumeLastPrerideStep()
    {
        HotspotModuleLoader hotspotModuleLoader = FindObjectOfType<HotspotModuleLoader>();
        if (hotspotModuleLoader.isResumeModule == true)
        {
            Debug.Log(" Module Resume from last step   ");
            existingStepScripts.resume(PlayerPrefs.GetInt(hotspotModuleLoader.SelectedModule.moduleName));
        }
        else
        {
            Debug.Log(" Module start from beginning   ");
        }

    }*/
/*    private void OnValidate()
    {
        if (mainCamEx == null)
            mainCamEx = FindObjectOfType<ExteriorCam>();
    }*/

    public void DownloadStepText()
    {
        Debug.LogError("Start Download StepText");
        StartCoroutine(DownloadStepTextCoroutine(("https://hero-native.s3.ap-south-1.amazonaws.com/Step_Text/" + stepCSV_filePath.Replace(" ", "+") + ".csv"), ReadSteps));
    }
    public IEnumerator DownloadStepTextCoroutine(string url, System.Action<string> callback)
    {
        Debug.LogError(" StepTExt Url" + url);
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SendWebRequest();

            while (!www.isDone)
            {
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                //Debug.Log(" data " + www.downloadHandler.text);
                callback(www.downloadHandler.text);
            }
            else if (www.isNetworkError || www.isHttpError || www.isNetworkError)
            {
                Debug.LogError(" Step text not download ");
                existingStepScripts.addProcessCompleteStep();
                existingStepScripts.LoadStepProgress(existingStepScripts.steps);
            }
        }
    }

    public void ReadSteps(string stepText)
    {
        Debug.Log("Step text" + stepText);
        if (stepText == "")
        {
            existingStepScripts.addProcessCompleteStep();
            existingStepScripts.LoadStepProgress(existingStepScripts.steps);
            return;
        }

        string[] lines = stepText.Split('\n');//File.ReadAllLines(filePath);
        Debug.Log("Line Lenght--"+lines.Length);
        for (int i = 1; i < lines.Length-2; i++)
        {
            // skip the 1st line
            //Debug.Log("");
            string[] s = Regex.Split(lines[i], ",");
            Debug.Log(" Index  "+s[0]);
            int index = int.Parse(s[0]);
            string locateTxt = s[1];
            string instr = s[2];
            string toolName = s[4];
            string torque = s[5];
            string cautionNotes = s[6];
            Debug.Log(" index " + index + " locateTxt " + locateTxt + " instr" + instr + " tool name " + toolName + " torque" + torque + " caution Note" + cautionNotes);
            existingStepScripts.steps[index].locateObjectText = locateTxt;
            existingStepScripts.steps[index].stepInstructions = instr;
            existingStepScripts.steps[index].specialToolName = toolName;
            existingStepScripts.steps[index].torque = torque;
            existingStepScripts.steps[index].cautionNotes = cautionNotes.Replace("~", "\n");
        }
        existingStepScripts.addProcessCompleteStep();
        existingStepScripts.LoadStepProgress(existingStepScripts.steps);
    }
}
