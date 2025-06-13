using EPOOutline;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
//using UnityEngine.XR.Interaction.Toolkit.Interactables;
#if UNITY_EDITOR
#endif
[System.Serializable]
public class Step {
    public string locateObjectText;
    public string stepInstructions;
    public string specialToolName;
    public Animator animatedObject;
    public string animTriggerName;
    public int debugStepNum;
    public Transform lookAtPoint;
    public Transform overrideCameraPosition;
    public bool isLocked = false;
    public GameObject[] objsToHighlight;
    public GameObject[] objsToDisable;
    public GameObject[] objsToEnable;
    public GameObject[] objsToE_D;
    public GameObject[] curvedLineObjs;
    [TextArea]
    public string cautionNotes="";
    public AudioClip voiceOver;
    public string torque;
    public Sprite[] toolSprite;
    [Space(20)]
    [Header("For VR")]
    public bool isUserinteraction;
    public GameObject vr_Interactable;
    [Space(10)]
    public bool isToolAttached;
    public GameObject[] vr_toolsAttch;
    public GameObject[] objDisableForVR;
    
    // public StepsEximProcessor.StepData data;
}

public class Steps : MonoBehaviour {
    #region Variables
    public string assemblyStepsCSV = "";
    public string dismantlingStepsCSV = "";

    public enum Process { Dismantling, Assembly };
    [Space(20)]
    public Process currentProcess;
    public int currentStep = -1;
   // [SerializeField] PartHighlighter highlighter;
    [Space(20)]
    public bool auto = false;
    [Range(1, 5)]
    public int animSpeed = 1;
    [Range(0.2f, 3f)]
    public float textDuration = 1f;

    [Header("---------- DISMANTLING PROCESS ----------")]
    public List<Step> steps;
    [Header("---------- ASSEMBLY PROCESS -------------")]
    public List<Step> assemblySteps;
    Text stepDesc;
    Text toolName;
    Text torqueValueTxt;
    Image[] toolImage;
    public delegate void StepUpdate(int s);
    public event StepUpdate stepUpdated;
    public StepsManager stepsMgr;
    //  [SerializeField]
    public bool partLocated = false;
    bool triggeredNext = false;
    public Animator currentAnim;
    GameObject replayButton;
    #endregion

    [SerializeField] bool keepHighlightForAnimation = false;
    bool enableDebug = true;
    [SerializeField] Step _currentStepData;
    //   [SerializeField]
    int totalSteps = 0;
    bool useNewCamera = true;
    public bool useStepPlayer = false;
    bool initialised = false;
    int lastStep = 0;
    CanvasGroup cautionCanvas;

    public GameObject progress;
    public Transform progressGroup;

    [Header("Enable for Model Tracking only")]
    public bool useAnimatorGroup = false;
    public GameObject animatorGrp;
    public GameObject UI;
    public RnR_ScrollHandler progressBar;
   // public RnRSidePanal sidePanal;
    private void Awake()
    {
        stepsMgr = StepsManager.Instance;
        progressBar = FindObjectOfType<RnR_ScrollHandler>(true);
        progressGroup = progressBar.gameObject.transform.GetChild(0).gameObject.transform.GetChild(0);
    }
    // Start is called before the first frame update
    void Start()
    {
        offAllHighlightObject();
        stepsMgr = FindObjectOfType<StepsManager>();
        stepsMgr.existingStepScripts = this;
   /*     stepsMgr.NewLineRopes.Clear();
        stepsMgr.NewLineRopes = new List<GameObject>();
        stepsMgr.LineRopeInSteps();*/
        //Debug.Log("stepsMgr.stepCSV_filePath -- "+ "https://hero-native.s3.ap-south-1.amazonaws.com/Step_Text/" + stepsMgr.stepCSV_filePath.Replace(" ", "+") + ".csv");
      //  StartCoroutine(DownloadStepTextCoroutine(("https://hero-native.s3.ap-south-1.amazonaws.com/Step_Text/" + stepsMgr.stepCSV_filePath.Replace(" ","+") + ".csv"), ReadSteps));
        addProcessCompleteStep();
        LoadStepProgress(steps);
       
        //  forModelTracking_Mat();
        // ReadSteps(stepsMgr.stepCSV_filePath);

        /* stepDesc = stepsMgr.stepDesc;
         toolName = stepsMgr.toolName_txt;
         //   toolImage = stepsMgr.toolImage;

         torqueValueTxt = stepsMgr.torqueValueTxt;
         if (highlighter == null)
             highlighter = FindObjectOfType<PartHighlighter>();
         //  if(currentProcess==Process.Assembly)
         if (useStepPlayer)
         {
             SetProcess(currentProcess);
             Previous();
         }
         else
         {
             useNewCamera = false;
         }*/

    }

    public IEnumerator DownloadStepTextCoroutine(string url, System.Action<string> callback)
    {
        Debug.LogError(" StepTExt Url"+url);
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SendWebRequest();

            while (!www.isDone)
            {
                yield return null;
            }
        
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(" data "+www.downloadHandler.text);
                callback(www.downloadHandler.text);
            }
            else if(www.isNetworkError || www.isHttpError|| www.isNetworkError|| www.isNetworkError)
            {
                Debug.LogError(" Step text not download ");
                addProcessCompleteStep();
                LoadStepProgress(steps);
            }
        }
    }
        public void ReadSteps(string stepText)
    {
        Debug.Log("Step text" + stepText);
        if (stepText == "")
        {
            addProcessCompleteStep();
            LoadStepProgress(steps);
            return;
        }

        string[] lines = stepText.Split('\n');//File.ReadAllLines(filePath);
        Debug.Log(lines.Length);
        for (int i = 1; i < lines.Length-1; i++)
        { 
            // skip the 1st line
            string[] s = Regex.Split(lines[i], ",");
            int index = int.Parse(s[0])-1;
            string locateTxt = s[1];
            string instr = s[2];
            string toolName = s[4];
            string torque = s[5];
            string cautionNotes = s[6];
            Debug.Log(" index "+index+" locateTxt "+locateTxt+" instr"+instr+" tool name "+toolName+" torque"+torque+" caution Note"+cautionNotes);
                steps[index].locateObjectText = locateTxt;
                steps[index].stepInstructions = instr;
                steps[index].specialToolName = toolName;
                steps[index].torque = torque;
                steps[index].cautionNotes = cautionNotes.Replace("~", "\n");
            
        }
        addProcessCompleteStep();
        LoadStepProgress(steps);
        Debug.LogError("StepTExt Load");
    }
        public void addProcessCompleteStep()
    {
        Step processCompleteStep = new Step();
        processCompleteStep.locateObjectText = "Process Complete";
        processCompleteStep.stepInstructions = "Process Complete";
        processCompleteStep.debugStepNum = steps.Count;
        steps.Add(processCompleteStep);
    }
    

        void HighLightCurrentStepObject(GameObject [] obj)
    {
        foreach (Step st in steps)
        {
          //  Debug.Log("  Steps Number  " + st.stepInstructions);
            if (st.stepInstructions != "Process Complete")
            {
                if (st.objsToHighlight.Length > 0)
                {
                    for (int i = 0; i < st.objsToHighlight.Length; i++)
                    {
                        if (st.objsToHighlight[i] != null)
                        {
                            st.objsToHighlight[i].GetComponent<Outlinable>().enabled = false;
                        }
                    }
                }
            }
        }

        if (obj.Length>0)
        {
            for (int i = 0; i < obj.Length; i++)
            {
                obj[i].GetComponent<Outlinable>().enabled=true;
            }
        }
    }
        void offAllHighlightObject()
        {
          //  List<GameObject> highlightedObj = new List<GameObject>();
            foreach (Step st in steps)
            {
               // Debug.Log("  Steps Number  " +st.locateObjectText);
                if (st.objsToHighlight.Length > 0)
                {
                    for (int i = 0; i < st.objsToHighlight.Length; i++)
                    {
                        if (st.objsToHighlight[i]!=null)
                        {
                            if (st.objsToHighlight[i].GetComponent<Outlinable>() == null)
                            {
                                st.objsToHighlight[i].AddComponent<Outlinable>().enabled = false;
                            }
                            else
                            {
                                st.objsToHighlight[i].GetComponent<Outlinable>().enabled = false;
                            }

                        }

                        /*  highlightedObj.Add(st.objsToHighlight[i]);*/

                    }
                }

           /* if (st.isUserinteraction == true)
            {
                if (st.vr_Interactable != null)
                {
                    st.vr_Interactable.AddComponent<XRSimpleInteractable>();
                    st.vr_Interactable.GetComponent<XRSimpleInteractable>().hoverEntered
                .AddListener(userInteraction);
                }

            }*/
        }
          /*  foreach (GameObject g in highlightedObj)
            {
                g.GetComponent<Outlinable>().enabled = false;
            }*/
        }

   /* void forModelTracking_Mat()
    {
        //  List<GameObject> highlightedObj = new List<GameObject>();

        foreach (Step st in steps)
        {
            // Debug.Log("  Steps Number  " +st.locateObjectText);
            if (st.objsToHighlight.Length > 0)
            {
                for (int i = 0; i < st.objsToHighlight.Length; i++)
                {
                    if (st.objsToHighlight[i] != null)
                    {
                        if (st.objsToHighlight[i].GetComponent<MaterialSwitch>() == null)
                        {
                            st.objsToHighlight[i].AddComponent<MaterialSwitch>().enabled = true;
                            st.objsToHighlight[i].GetComponent<MaterialSwitch>().GetMehsesMaterials();
                            st.objsToHighlight[i].GetComponent<MaterialSwitch>().transperMat = stepsMgr.modelTransparent;
                        }
                        else
                        {
                            st.objsToHighlight[i].GetComponent<MaterialSwitch>().enabled = true;
                            st.objsToHighlight[i].GetComponent<MaterialSwitch>().GetMehsesMaterials();
                            st.objsToHighlight[i].GetComponent<MaterialSwitch>().transperMat =stepsMgr.modelTransparent;
                        }
                    }

                }
            }
        }
    }*/

    public List<GameObject> loadedStepsInfo;
    Text stepinfo;
    
   public void LoadStepProgress(List<Step> selectedSteps)
    {
        Debug.Log(" Load Steps Progress ...... add steps  ");
        loadedStepsInfo = new List<GameObject>();
        for (int i = 0; i < selectedSteps.Count; i++)
        {
            //GameObject currentProgress = Instantiate(progress, progressGroup);
            GameObject currentProgress = Instantiate(Resources.Load("Toggle", typeof(GameObject)),progressGroup) as GameObject;
            currentProgress.transform.GetChild(1).GetComponent<Text>().text = selectedSteps[i].stepInstructions;
            currentProgress.transform.GetChild(1).GetComponent<Text>().color = Color.grey;
            currentProgress.GetComponent<SwitchStepState>().IsOn(false);
            loadedStepsInfo.Add(currentProgress);
          //  GameObject currentProgressLine = Instantiate(stepsMgr.divideingLineForStepList, progressGroup);
        }
        //     progressBar.content.localPosition = new Vector3(0, 0, 0);

        //stepDesc = stepsMgr.stepDesc;
      //  toolName = stepsMgr.toolName_txt;
        //toolImage = stepsMgr.toolImage;

      //  torqueValueTxt = stepsMgr.torqueValueTxt;
   /*     if (highlighter == null)
            highlighter = FindObjectOfType<PartHighlighter>();*/
        //  if(currentProcess==Process.Assembly)
        if (useStepPlayer)
        {
         //  SetProcess(Process.Dismantling);
           // Previous();
         //   Previous_New();
            progressBar.SetScrollPosPrev();
        }
        else
        {
            useNewCamera = false;
        }
     //   stepsMgr.loadingScreen.SetActive(false);
    }

    private void Update() {

        //Debug.LogError(" current step   "+ currentStep);
        if (currentAnim != null) {
            AnimatorStateInfo info = currentAnim.GetCurrentAnimatorStateInfo(0);
            float time = info.normalizedTime % 1;
            if (time > 0.995f && !triggeredNext) {
                StartCoroutine(HighlightNextBtn());
                triggeredNext = true;
                currentAnim = null;
             //StartCoroutine(SkipToNext(0.5f));
            }
        }
        /*if(!UI.activeInHierarchy)
        UI.SetActive(true);*/
    }

    private void OnEnable() {
        stepsMgr = StepsManager.Instance;
        if (useStepPlayer) {
            StepsManager.NextStep += Next;
            //StepsManager.PreviousStep += Previous;
            StepsManager.PreviousStep += Previous_New;
            StepsManager.ReplayStep += Replay;
        } else {
            StepsManager.NextStep += NextAnim;
            StepsManager.PreviousStep += PreviousAnim;
        }

    }
    private void OnDisable() {

        if (useStepPlayer) {
            StepsManager.NextStep -= Next;
            //StepsManager.PreviousStep -= Previous;
            StepsManager.PreviousStep -= Previous_New;
            StepsManager.ReplayStep -= Replay;
        } else {
            StepsManager.NextStep -= NextAnim;
            StepsManager.PreviousStep -= PreviousAnim;
        }

    }
    #region Updated
    public void Assembly() => CheckAndSetProcess(Process.Assembly);
    public void Assembly(int stepNum) {
        CheckAndSetProcess(Process.Assembly);
        FastForwardToStep(stepNum);
        LocatePartv2();
        partLocated = true;
    }
    public void Dismantling() => CheckAndSetProcess(Process.Dismantling);
    public void Dismantling(int stepNum) {
        CheckAndSetProcess(Process.Dismantling);
        FastForwardToStep(stepNum);
        LocatePartv2();
        partLocated = true;
    }

    void CheckAndSetProcess(Process proc) {
        if (currentStep > 0)
            FastForwardToStep(0);
        if (proc != currentProcess) {
            SetProcess(proc);
            Previous();
        }
    }
    void SetProcess(Process proc) {
        DebugLog("SET PROCESS:" + proc.ToString());
        partLocated = false;
        currentProcess = Process.Dismantling;
        if (proc == Process.Assembly) {
            DebugLog("Starting ASSEMBLY Process");
            // Fast-Forward all Dismantling steps
            currentStep = 0;
            int targetSt = steps.Count - 1;
            FastForwardToStep(targetSt);

        } else {
            DebugLog("Starting Dismantling Process");
            // Fast-Rewind All Dismantling Steps
            currentStep = steps.Count - 1;
            FastForwardToStep(0);
        }
        currentProcess = proc;
        totalSteps = GetTotalSteps();
        currentStep = 0;
        initialised = true;
        //   Previous();
        //  partLocated = true;
    }

    IEnumerator HighlightNextBtn(float delay = 0f) {
        yield return new WaitForSeconds(delay);
        iTween.PunchScale(stepsMgr.nextBtn, iTween.Hash("amount", Vector3.one * 0.2f, "time", 0.5f, "looptype", "loop"));
        yield return new WaitForSeconds(5f);
        if (stepsMgr.nextBtn.GetComponent<iTween>() != null) {
            DestroyImmediate(stepsMgr.nextBtn.GetComponent<iTween>());
            stepsMgr.nextBtn.transform.localScale = Vector3.one;
        }
    }

    public Step GetStepAt(int s) {
        if (s < 0)
            s = 0;
        if (s >= GetTotalSteps())
            s = GetTotalSteps() - 1;
        return currentProcess == Process.Dismantling ? steps[s] : assemblySteps[s];
    }
    public void UpdateCurrentStepData() {
        _currentStepData = GetStepAt(currentStep);
    }
    public void Previous() {
    
        Debug.Log("PREVIOUS:" + currentStep);
        if (currentStep >= totalSteps)
            currentStep = totalSteps - 1;
        Step curStep = GetCurrentStep();
   /*     stepsMgr.cautionHeader.SetActive(false);
       // stepsMgr.cautionLine.SetActive(false);
        *//*      if (curStep.cautionNotes == "") {
                  stepsMgr.cautionHeader.SetActive(false);
                  stepsMgr.cautionLine.SetActive(false);
              } else {
                  stepsMgr.cautionHeader.SetActive(true);
                  stepsMgr.cautionLine.SetActive(true);
              }*//*
        stepsMgr.Caution_Text.text = curStep.cautionNotes.ToString();*/

        stepsMgr.cautionHeader.SetActive(false);

        stepsMgr.Caution_Text.text = curStep.cautionNotes.ToString();


        stepinfo = loadedStepsInfo[currentStep].transform.GetChild(1).GetComponent<Text>();
        stepinfo.fontStyle = FontStyle.Normal;
        stepinfo.color = Color.green;
        stepinfo.GetComponentInParent<SwitchStepState>().IsOn(false);


        progressBar.SetScrollPosPrev();
        //REVERSE CURRENT STEP
        partLocated = false;
      /*  if (currentStep >= 0) {
            if (!GetStepAt(currentStep).isLocked) {
                AnimRewind(GetCurrentStep());
               FastTraversalV2(GetCurrentStep(), 0);
            }
        }*/
        // REVERSE HALF OF PREV STEP
        currentStep--;
        if (currentStep < 0)
            currentStep = 0;

        Step c = GetCurrentStep();
        if (c.isLocked) {
            DebugLog(currentStep + " : is Locked, skipping step");
            if (currentStep > 0) {
                while (GetCurrentStep().isLocked) {
                    currentStep--;
                }
            }
            if (currentStep <= 0) {
                if (GetCurrentStep().isLocked) {
                    while (GetCurrentStep().isLocked) {
                        currentStep++;
                    }
                }
            }
        }
        Step d = GetCurrentStep();

        //  FastTraversalV2(d, 0);
        // Run
        // s(GetCurrentStep(), false);
          // FastTraversalV2(GetCurrentStep(), 0);
        // highlighter.RemoveHighLight();
        // LocatePartv2();
        //  partLocated = true;
        //progressBar.SetScrollPosPrev();

        if (curStep.isLocked)
        {
            Debug.LogError("CURRENT STEP IS LOCKED?");
        }
        else
        {
            playVoiceOver(curStep.voiceOver);
            // check and stop highlighting the button
            if (stepsMgr.nextBtn.GetComponent<iTween>() != null)
            {
                DestroyImmediate(stepsMgr.nextBtn.GetComponent<iTween>());
                stepsMgr.nextBtn.transform.localScale = Vector3.one;
            }

            HighLightCurrentStepObject(curStep.objsToHighlight);
            //ToggleHighlight(curStep.objsToHighlight,true);
            ToggleObjects(curStep.objsToEnable, true);

            Debug.LogError("Part Not Locate");
            Debug.Log(" Switch Caution note ");
            if (curStep.cautionNotes == "")
            {
                SwitchCautionNote(false);
            }
            else
            {
                SwitchCautionNote(true);
            }
            partLocated = false;
            CompleteV2();

        }
        _updateStep();

        if (curStep.isUserinteraction == false && curStep.isToolAttached == false)
        {
            //   currentStep++;
            stepsMgr.nextBtn.GetComponent<Button>().interactable = true;
            // stepsMgr.nextBtn.SetActive(true);
        }
     

    }
 
    public void Replay() {
        if (currentStep >= 0 && currentStep < totalSteps) {
            if (partLocated)
                LocatePartv2();
            else {
                Step curStep = GetCurrentStep();
                if (curStep.cautionNotes == "") {
                    stepsMgr.cautionHeader.SetActive(false);
                 //   stepsMgr.cautionLine.SetActive(false);
                } else {
                    stepsMgr.cautionHeader.SetActive(true);
                  //  stepsMgr.cautionLine.SetActive(true);
                }
                //Reset Camera Pos
                SetCameraPositionV2(curStep);
                // Play VO
                if (curStep.voiceOver != null && GetComponent<AudioSource>() != null) {
                    GetComponent<AudioSource>().clip = curStep.voiceOver;
                    GetComponent<AudioSource>().Play();
                }
                // start animation
                if (curStep.animatedObject != null) {
                    curStep.animatedObject.speed = auto ? animSpeed : 1f;
                    curStep.animatedObject.SetTrigger(curStep.animTriggerName);
                    currentAnim = curStep.animatedObject;

                }
            }

           // stepsMgr.mainCamEx.enableLookAt = true;
        }
    }
    bool LocateStepEmpty = false;
    Text previousStepInfo;
    Text nextStepInfo;

    void playVoiceOver(AudioClip audioClip)
    {
        stepsMgr.audioSource.clip = audioClip;
        stepsMgr.audioSource.PlayDelayed(1.5f);
    }

    public void Previous_New()
    {
        currentStep--;
      
     
        ToggleObjects(steps[currentStep].objsToDisable, true);
        ToggleObjects(steps[currentStep + 1].objsToEnable, false);
        ToggleObjects(steps[currentStep + 1].objsToE_D, false);
        AnimRewind(steps[currentStep + 1]);
        if (steps[currentStep + 1].isUserinteraction == true)
        {
            Transform[] findarrow = steps[currentStep + 1].vr_Interactable.GetComponentsInChildren<Transform>(true);

            foreach (Transform item in findarrow)
            {
                if (item.gameObject.name.Contains("Arrow"))
                {
                    item.gameObject.SetActive(false);
                }

            }
        }

        if (steps[currentStep + 1].isToolAttached == true)
        {
            ToggleObjects(steps[currentStep + 1].objDisableForVR, false);
            for (int i = 0; i < steps[currentStep + 1].vr_toolsAttch.Length; i++)
            {

                if (steps[currentStep + 1].vr_toolsAttch[i].tag == "tool")
                {
                    steps[currentStep + 1].vr_toolsAttch[i].transform.position = stepsMgr.toolSpwanPos.position;
                    steps[currentStep + 1].vr_toolsAttch[i].transform.localRotation = stepsMgr.toolSpwanPos.localRotation;
                 //   steps[currentStep + 1].vr_toolsAttch[i].transform.GetComponent<ToolPosStore_VR>().resetPos();
                }

                steps[currentStep + 1].vr_toolsAttch[i].SetActive(false);

            }
        }

        Debug.LogError(" step num " + currentStep);
       /* if (currentStep < 0)
        {
            Debug.LogError(" step num --1");
          
            AnimRewind(steps[currentStep]);
           // ToggleObjects(steps[currentStep].objsToDisable, true);

        }*/
        Debug.LogError(" step num --2");
          stepsMgr.nextBtn.GetComponent<Button>().interactable = false;
            totalSteps = GetTotalSteps();

        if (currentStep < 0)
            currentStep = 0;
        if (currentStep >= totalSteps)
            currentStep = totalSteps - 1;
        while (GetCurrentStep().isLocked && currentStep < totalSteps - 1)
        {
            currentStep--;
        }
        Debug.LogError(" step num --3");
        Step curStep = GetCurrentStep();
     
        stepsMgr.cautionHeader.SetActive(false);
    
        stepsMgr.Caution_Text.text = curStep.cautionNotes.ToString();

        stepinfo = loadedStepsInfo[currentStep].transform.GetChild(1).GetComponent<Text>();
        stepinfo.fontStyle = FontStyle.Normal;
        stepinfo.color = Color.white;
        stepinfo.GetComponentInParent<SwitchStepState>().IsOn(true);
        Debug.LogError(" step num --4");
        /*  if (curStep.animatedObject != null)
          {
              currentAnim = curStep.animatedObject;
              currentAnim.Update(0f);
          }*/
        /*    if (curStep.animatedObject!=null)
            {
                CheckReversal(curStep.animatedObject.gameObject);
            }*/


        if (currentStep > 0)
        {
            nextStepInfo = loadedStepsInfo[currentStep + 1].transform.GetChild(1).GetComponent<Text>();
            if (nextStepInfo != null)
            {
                nextStepInfo.fontStyle = FontStyle.Normal;
                nextStepInfo.color = Color.grey;
                nextStepInfo.GetComponentInParent<SwitchStepState>().IsOn(false);
            }
        }
        progressBar.SetScrollPosPrev();

        if (curStep.locateObjectText == "")
        {
            Debug.Log(" Locate Text Empty " + partLocated);
            partLocated = false;
        }

        if (curStep.isLocked)
        {
            Debug.LogError("CURRENT STEP IS LOCKED?");
        }
        else
        {
            AnimRewind(GetCurrentStep());
            playVoiceOver(curStep.voiceOver);
            // check and stop highlighting the button
            /*  if (stepsMgr.nextBtn.GetComponent<iTween>() != null)
              {
                  DestroyImmediate(stepsMgr.nextBtn.GetComponent<iTween>());
                  stepsMgr.nextBtn.transform.localScale = Vector3.one;
              }*/

            if (curStep.objsToHighlight.Length>0)
            {
                HighLightCurrentStepObject(curStep.objsToHighlight);
            }
           
            //ToggleHighlight(curStep.objsToHighlight,true);
            ToggleObjects(curStep.objsToEnable, true);
            ToggleObjects(curStep.objsToE_D, true);
            ToggleObjects(curStep.objDisableForVR, true);
            //Debug.LogError("Part Not Locate");
            Debug.Log(" Switch Caution note ");
            if (curStep.cautionNotes == "")
            {
                SwitchCautionNote(false);
            }
            else
            {
                SwitchCautionNote(true);
            }
            partLocated = false;
            CompleteV2();
            //    }

        }

        _updateStep();
        if (curStep.isUserinteraction == false && curStep.isToolAttached == false)
        {
         //   currentStep++;
            stepsMgr.nextBtn.GetComponent<Button>().interactable = true;
            // stepsMgr.nextBtn.SetActive(true);
        }
    }
    public void Next()
    {
        currentStep++;
        Debug.LogError(" step num " + currentStep);
        if (currentStep>0)
        {
            ToggleObjects(steps[currentStep - 1].objsToDisable, false);
            ToggleObjects(steps[currentStep - 1].objsToE_D, false);
            AnimForward(steps[currentStep - 1]);

            if (steps[currentStep - 1].isUserinteraction == true)
            {
                Transform[] findarrow = steps[currentStep - 1].vr_Interactable.GetComponentsInChildren<Transform>(true);

                foreach (Transform item in findarrow)
                {
                    if (item.gameObject.name.Contains("Arrow"))
                    {
                        item.gameObject.SetActive(false);
                    }

                }
            }

            if (steps[currentStep - 1].isToolAttached == true)
            {
                ToggleObjects(steps[currentStep - 1].objDisableForVR, false);
                for (int i = 0; i < steps[currentStep + 1].vr_toolsAttch.Length; i++)
                {

                    if (steps[currentStep + 1].vr_toolsAttch[i].tag == "tool")
                    {
                        steps[currentStep + 1].vr_toolsAttch[i].transform.position = stepsMgr.toolSpwanPos.position;
                        steps[currentStep + 1].vr_toolsAttch[i].transform.localRotation = stepsMgr.toolSpwanPos.localRotation;
                    }

                    steps[currentStep + 1].vr_toolsAttch[i].SetActive(false);

                }
            }
        }
       
        //stepsMgr.StepNumber.text =currentStep.ToString();
        //Debug.Log("Total Step...." + totalSteps);
       // stepsMgr.nextBtn.SetActive(false);
        stepsMgr.nextBtn.GetComponent<Button>().interactable=false;
        totalSteps = GetTotalSteps();
        if (currentStep < 0)
            currentStep = 0;
        if (currentStep >= totalSteps)
            currentStep = totalSteps - 1;
        while (GetCurrentStep().isLocked && currentStep < totalSteps - 1)
        {
            currentStep++;
        }
        Step curStep = GetCurrentStep();

       
        stepsMgr.cautionHeader.SetActive(false);
       // stepsMgr.cautionLine.SetActive(false);
        // Debug.LogError("Current Step "+ curStep.cautionNotes.ToString());
        stepsMgr.Caution_Text.text = curStep.cautionNotes.ToString();
        // stepsMgr.clickOnNextOrPreviousButton();
        
        stepinfo = loadedStepsInfo[currentStep].transform.GetChild(1).GetComponent<Text>();
        stepinfo.fontStyle = FontStyle.Normal;
        stepinfo.color = Color.white;
        stepinfo.GetComponentInParent<SwitchStepState>().IsOn(true);


        if (currentStep > 0)
        {
            previousStepInfo = loadedStepsInfo[currentStep - 1].transform.GetChild(1).GetComponent<Text>();
            if (previousStepInfo != null)
            {
                previousStepInfo.fontStyle = FontStyle.Normal;
                previousStepInfo.color = Color.grey;
                previousStepInfo.GetComponentInParent<SwitchStepState>().IsOn(false);
            }
        }
        progressBar.SetScrollPosNext();

        if (curStep.locateObjectText == "")
        {
            Debug.Log(" Locate Text Empty " + partLocated);
            partLocated = false;
        }

        if (curStep.isLocked)
        {
            Debug.LogError("CURRENT STEP IS LOCKED?");
        }
        else
        {
            AnimRewind(GetCurrentStep());
            playVoiceOver(curStep.voiceOver);
            // check and stop highlighting the button
            if (stepsMgr.nextBtn.GetComponent<iTween>() != null)
            {
                DestroyImmediate(stepsMgr.nextBtn.GetComponent<iTween>());
                stepsMgr.nextBtn.transform.localScale = Vector3.one;
            }

            if (curStep.objsToHighlight.Length > 0)
            {
                HighLightCurrentStepObject(curStep.objsToHighlight);
            }
            //ToggleHighlight(curStep.objsToHighlight,true);
            ToggleObjects(curStep.objsToEnable, true);
            ToggleObjects(curStep.objsToE_D, true);
            ToggleObjects(curStep.objDisableForVR, true);
            if (currentStep > 0)
            {
                ToggleObjects(steps[currentStep - 1].objsToDisable, false);
            }

            // Debug.LogError("Part Not Locate");
            Debug.Log(" Switch Caution note ");
                if (curStep.cautionNotes == "")
                {
                    SwitchCautionNote(false);
                }
                else
                {
                    SwitchCautionNote(true);
                }
                partLocated = false;
                CompleteV2();
        //    }

        }

        _updateStep();
        if (curStep.isUserinteraction==false&&curStep.isToolAttached==false)
        {
          //  currentStep++;
           // stepsMgr.nextBtn.SetActive(true);
            stepsMgr.nextBtn.GetComponent<Button>().interactable = true;
        }
       

    /*    if (curStep.locateObjectText == "")
        {
            //  LocateStepEmpty = true;
            LocatePartv2();
        }*/

    }
    public void SwitchCautionNote(bool state)
    {
            Debug.Log(" Switch Caution note " +state);
            stepsMgr.cautionHeader.SetActive(state);
          //  stepsMgr.cautionLine.SetActive(state);
    }

    public void resume(int stepNumber)
    {
        FastForwardTo(stepNumber);
        UpdateCurrentStepData();
    }
    void ResolveSpecialTool() {

    }
    public void FastForwardTo(int ffstep) {
        DebugLog("FF to :" + ffstep);
        if (ffstep >= GetTotalSteps()) {
            Debug.Log("Fast Fwd to Assembly!");
            Assembly(ffstep - totalSteps);

        } else
            FastForwardToStep(ffstep);
        LocatePartv2();
        partLocated = true;
    }
    void FastForwardToStep(int ffStep) {
        //  ffStep = ffStep >= GetTotalSteps() ? GetTotalSteps() - 1 : ffStep;

        if (ffStep > currentStep) {
            while (ffStep > currentStep) {
                FastTraversalV2(GetCurrentStep(), 1);
                currentStep++;
            }
        } else {
            AnimRewind(GetCurrentStep());
            while (ffStep < currentStep) {
                FastTraversalV2(GetCurrentStep(), 0);

                currentStep--;
            }
        }
      //  highlighter.RemoveAllHighlights();
        _updateStep();
    }

    void FastTraversal(int stepId) {
        Step s = currentProcess == Process.Dismantling ? steps[stepId] : assemblySteps[stepId];
        ToggleObjects(s.objsToEnable, true);
        //  ToggleObjects(s.objsToEnable, fwd);
        if (s.animatedObject != null) {
            DebugLog("FT");
            Animator curAO = s.animatedObject;

            bool curState = s.animatedObject.gameObject.activeInHierarchy;
            s.animatedObject.gameObject.SetActive(true);
            s.animatedObject.SetTrigger(s.animTriggerName);
            s.animatedObject.speed = 1000f;
            s.animatedObject.Update(10f);
            //un-comment following 
            //AnimatorStateInfo info = s.animatedObject.GetCurrentAnimatorStateInfo(0);
            //var has = s.animatedObject.runtimeAnimatorController.animationClips[0].GetHashCode();
            //Debug.Log(stepId + "INFO HASH:" + s.animatedObject.runtimeAnimatorController.animationClips[0].name);
            // s.animatedObject.Play(has, 0, 1f);
            s.animatedObject.Update(10f);
            s.animatedObject.gameObject.SetActive(curState);
            currentAnim = s.animatedObject;
            //  curAO.speed = 0;

        }
        ToggleObjects(s.objsToDisable, false);
        //  ToggleObjects(s.objsToDisable, !fwd);
    }
    void FastTraversalV2(Step s, int fwd) {
        bool state = fwd >= 1 ? true : false;

        if (fwd == 1)   //Fast Forward to Next
            ToggleObjects(s.objsToEnable, true);
        else
            ToggleObjects(s.objsToDisable, true);
        //DebugLog("Fast Forwarded");
        //if (s.stepInstructions.Contains("Remove the intake manifold"))
        //    Debug.Log("INTAKE BEFORE:"+s.objsToDisable[0].activeSelf);
        if (!s.isLocked)
            if (s.animatedObject != null) {

                Animator curAO = s.animatedObject;

                bool curState = s.animatedObject.gameObject.activeInHierarchy;
                if (!curState)
                    CheckReversal(s.animatedObject.gameObject);
                s.animatedObject.gameObject.SetActive(true);
                s.animatedObject.SetTrigger(s.animTriggerName);

                s.animatedObject.speed = fwd >= 1 ? 1000f : 0;
                s.animatedObject.Update(10f);

                //can be added twice to compensate for error 
                s.animatedObject.Update(10f);
                s.animatedObject.gameObject.SetActive(curState);
                s.animatedObject.enabled = false;
                currentAnim = s.animatedObject;

            }

        if (fwd >= 1)
            ToggleObjects(s.objsToDisable, false);
        else
            ToggleObjects(s.objsToEnable, false);
        //if (s.stepInstructions.Contains("Remove the intake manifold"))
        //    Debug.Log("INTAKE AFTER:" + s.objsToDisable[0].activeSelf);
    }

    public Step GetCurrentStep() {
        Step result;
        //**********************************************************************
        //if (currentStep < 0)
        //    currentStep = 0;
        //*********************************************************************
        if (currentProcess == Process.Dismantling) {
            if (currentStep >= steps.Count)
                currentStep = steps.Count - 1;
            result = steps[currentStep];
        } else {
            if (currentStep >= assemblySteps.Count)
                currentStep = assemblySteps.Count - 1;
            result = assemblySteps[currentStep];
        }

        //if (currentStep > (steps.Count - 2))
        //    Debug.LogError("Reaching!!!");
        return result;
    }

    public int GetTotalSteps() => GetCurrentStepList().Count;
    public List<Step> GetCurrentStepList() => currentProcess == Process.Dismantling ? steps : assemblySteps;
   // public PartHighlighter GetPartHighlighter() => highlighter;
    void DebugLog(string msg) {
        if (enableDebug)
            Debug.Log(msg);
    }
    /// <summary>
    /// ////
    /// </summary>
    /// <param name="s"></param>
    /// <param name="forward"></param>
    /// <param name="toggleFirst"></param>
    void RunToggles(Step s, bool forward) {

        ToggleObjects(s.objsToDisable, !forward);
        // disable the objects which were enabled for this step
        ToggleObjects(s.objsToEnable, forward);
        //   ToggleHighlight(s.objsToHighlight, forward);

    }
    void CheckReversal(GameObject g) {

        if (g.activeSelf) {
            if (!g.activeInHierarchy) {
                GameObject par = g.transform.parent.gameObject;
                while (!par.activeSelf) {
                    Debug.Log("This", par);
                    par.SetActive(true);
                    par = par.transform.parent.gameObject;
                }
            }
        }
    }

    public void EnableAnimatedObject(Animator anim) {
        // this is only required for model tracking and the bool will be OFF for surface tracking.
        if (useAnimatorGroup) {
            for (int i = 0; i < animatorGrp.transform.childCount; i++) {
                // only enable the object if the animator matches with the current step. all others will be disabled.
                animatorGrp.transform.GetChild(i).gameObject.SetActive(animatorGrp.transform.GetChild(i).GetComponent<Animator>() == anim);
                
                Debug.LogWarning(animatorGrp.transform.GetChild(i).gameObject.name + " " + (animatorGrp.transform.GetChild(i).GetComponent<Animator>() == anim));
            }
        }
    }


    void LocatePartv2() {
        Debug.Log("Locate V2:" + currentStep+"Total  steps Count "+steps.Count);

       
        if (currentStep > -1) {
            Step lpv2 = GetCurrentStep();
            
            //stepsMgr.toolList.transform.localPosition = new Vector3(-(Screen.width / 2f) - 280f,
            // stepsMgr.toolList.transform.localPosition.y, stepsMgr.toolList.transform.localPosition.z);

            if (currentStep < totalSteps) {
                //      lpv2 = GetCurrentStep();

                EnableAnimatedObject(lpv2.animatedObject);
                ToggleObjects(lpv2.objsToEnable, true);
                //*****************************************************************************
                //   if (lpv2.animatedObject)

                //*****************************************************************************
                if (lpv2 != null) {
                    if (lpv2.animatedObject != null) {
                        lpv2.animatedObject.enabled = true;
                        CheckReversal(lpv2.animatedObject.gameObject);
                    } else
                        Debug.Log("<color=red>No Animator found at step:</color>" + currentStep);
                } else
                    Debug.LogError("Cannot Find:" + lpv2.animatedObject.name, lpv2.animatedObject.gameObject);
                //   ToggleObjects(lpv2.objsToDisable, true);
                // start at first frame
                if (lpv2.animatedObject != null) {
                    AnimRewind(lpv2);
                }
                //  FastTraversalV2(lpv2, 0);

                // enable highlight
                ToggleHighlight(lpv2.objsToHighlight, true);
                if (currentProcess == Process.Dismantling) {
                    ToggleCurvedLineDelay(steps[currentStep].curvedLineObjs, false);
                    if (currentStep != 0)
                        ToggleCurvedLineDelay(steps[lastStep].curvedLineObjs, false);
                } else {
                    ToggleCurvedLineDelay(assemblySteps[currentStep].curvedLineObjs, false);
                    if (currentStep != 0)
                        ToggleCurvedLineDelay(assemblySteps[lastStep].curvedLineObjs, false);
                }
                // set the cam position
                SetCameraPositionV2(lpv2);
                // show description
                // set the description
                //    stepsMgr.stepDesc.text = s.locateObjectText;
                ShowStaticContent(lpv2);

            }
           
        }
        /*if (currentStep == steps.Count - 1)
        {
            
            Debug.Log($"Dhiraj : Process Completed : {loadedStepsInfo[currentStep].transform.GetChild(1).GetComponent<Text>().text}");

            stepsMgr.completeProccessTask.SetActive(true);
            //  currentStep = 0;
        }
        else
        {
            
            stepsMgr.completeProccessTask.SetActive(false);
        }*/
    }
    void CompleteV2() {

        Step cmpStep = GetCurrentStep();

        if (LocateStepEmpty == true)
        {
            Debug.LogError(" Locate Text Empty");
            Debug.LogError(cmpStep.cautionNotes);
            
            if (cmpStep.cautionNotes == "")
            {
                SwitchCautionNote(false);
            }
            else
            {
                stepsMgr.Caution_Text.text = cmpStep.cautionNotes;
                SwitchCautionNote(true);
            }
            LocateStepEmpty = false;
        }
        // disable highlight for current step
        /*  if (!keepHighlightForAnimation)
              highlighter.RemoveHighLight();*/
        // SetCameraPositionV2(cmpStep);
        // set the description
        //stepsMgr.stepDesc.text = cmpStep.stepInstructions;
        /*   if (currentProcess == Process.Dismantling) {
               ToggleCurvedLineDelay(steps[currentStep].curvedLineObjs, true);
               lastStep = currentStep;
           } else {
               ToggleCurvedLineDelay(assemblySteps[currentStep].curvedLineObjs, true);
               lastStep = currentStep;
           }*/

        // Play VO
        /*  if (cmpStep.voiceOver != null && GetComponent<AudioSource>() != null) {
              GetComponent<AudioSource>().clip = cmpStep.voiceOver;
              GetComponent<AudioSource>().Play();
          }*/

    /*    if (currentAnim != null)
        {
            currentAnim.Update(1000f);
            currentAnim = null;

        }
*/
        // start animation
        if (cmpStep.animatedObject != null && cmpStep.isUserinteraction==false && cmpStep.isToolAttached==false) {

            cmpStep.animatedObject.enabled = true;
            cmpStep.animatedObject.speed = auto ? animSpeed : 1f;
            cmpStep.animatedObject.SetTrigger(cmpStep.animTriggerName);
            // cmpStep.animatedObject.ResetTrigger(cmpStep.animTriggerName);
            currentAnim = cmpStep.animatedObject;

        }
        else 
        {
            if (cmpStep.isUserinteraction == true)
            {
                Transform[] findarrow = cmpStep.vr_Interactable.GetComponentsInChildren<Transform>(true);

                foreach (Transform item in findarrow)
                {
                    if (item.gameObject.name.Contains("Arrow"))
                    {
                        item.gameObject.SetActive(true);
                    }

                }
                //cmpStep.vr_Interactable.transform.GetChild(0).gameObject.SetActive(true);
                cmpStep.vr_Interactable.GetComponent<XRSimpleInteractable>().enabled=true;
            }

            if (cmpStep.isToolAttached == true)
            {
                foreach (GameObject item in stepsMgr.vr_Tools)
                {
                    if (item.gameObject.name==cmpStep.vr_toolsAttch[0].name || item.gameObject.name == cmpStep.vr_toolsAttch[1].name)
                    {
                        item.gameObject.SetActive(true);
                    }
                    else
                    {
                        item.gameObject.SetActive(false);
                    }
                }
            }
            //StartCoroutine(HighlightNextBtn(1f));
            //  StartCoroutine(SkipToNext(textDuration));
        }
        if (currentStep == steps.Count - 1)
        {
            /*previousStepInfo = loadedStepsInfo[currentStep].transform.GetChild(1).GetComponent<Text>();
            if (previousStepInfo != null)
            {
                previousStepInfo.GetComponentInParent<ToggleUI>().IsOn(false);
            }*/
            stepsMgr.completeProccessTask.SetActive(true);
            stepsMgr.VR_UI.SetActive(false);
            currentStep = -1;
            progressBar.SetScrollPosPrev();
        }
        else
        {
            stepsMgr.completeProccessTask.SetActive(false);
        }
    }

   public void RestartSteps()
    {
        SceneManager.LoadScene("AEML_RnR_Animation");
    }
    public void userInteraction(HoverEnterEventArgs args)
        {
        
            Debug.Log("  Interacted----- ");
            Step step = GetCurrentStep();

        Transform[] findarrow = step.vr_Interactable.GetComponentsInChildren<Transform>(true);

        foreach (Transform item in findarrow)
        {
            if (item.gameObject.name.Contains("Arrow"))
            {
                item.gameObject.SetActive(false);
            }
           
        }
    
        if (step.animatedObject != null)
            {
                step.animatedObject.enabled = true;
                step.animatedObject.speed = auto ? animSpeed : 1f;
                step.animatedObject.SetTrigger(step.animTriggerName);
                currentAnim = step.animatedObject;
        }



        step.vr_Interactable.GetComponent<XRSimpleInteractable>().enabled = false;

       // stepsMgr.nextBtn.SetActive(true);
        stepsMgr.nextBtn.GetComponent<Button>().interactable = true;
        for (int i = 0; i < step.objDisableForVR.Length; i++)
        {
        
            step.objDisableForVR[i].SetActive(false);
        }
        //  currentStep++;
    }
    public void userToolsInteraction(SelectEnterEventArgs args)
    {
       
        Step step = GetCurrentStep();
        Debug.Log("  Tool Placed ------------ "+ step.animTriggerName);
        if (step.animatedObject != null)
        {
            currentAnim = step.animatedObject;
            step.animatedObject.enabled = true;
            step.animatedObject.speed = auto ? animSpeed : 1f;

            step.animatedObject.SetTrigger(step.animTriggerName);
        }

        for (int i = 0; i < step.objDisableForVR.Length; i++)
        {
            /*    if (step.vr_toolsAttch[i].tag == "tool")
                {
                    step.vr_toolsAttch[i].transform.position = stepsMgr.toolSpwanPos.position;
                }*/
          
            step.objDisableForVR[i].SetActive(false);
        }

        for (int i = 0; i < step.vr_toolsAttch.Length; i++)
        {
        /*    if (step.vr_toolsAttch[i].tag == "tool")
            {
                step.vr_toolsAttch[i].transform.position = stepsMgr.toolSpwanPos.position;
            }*/
            step.vr_toolsAttch[i].SetActive(false);
        }
       
      //  stepsMgr.nextBtn.SetActive(true);
        stepsMgr.nextBtn.GetComponent<Button>().interactable = true;

       // currentStep++;
    }
    void AnimRewind(Step thisStep) => SkipAnim(thisStep, 0);
    void AnimForward(Step thisStep) => SkipAnim(thisStep, 1);

    void SkipAnim(Step thisStep, int skipTo) {
        DebugLog("SKIPANIM:" + skipTo);
        if (thisStep.animatedObject != null) {
            Animator curAO = thisStep.animatedObject;

            //   bool curAState = curAO.gameObject.activeInHierarchy;
            AnimatorStateInfo info = curAO.GetCurrentAnimatorStateInfo(0);
            if (skipTo == 0)
                thisStep.animatedObject.SetTrigger(thisStep.animTriggerName);
            //  curAO.enabled = true;
            //   curAO.Play(info.fullPathHash, 0, skipTo);
            curAO.Play(info.fullPathHash, -1, skipTo);

            currentAnim = thisStep.animatedObject;
            //   curAO.gameObject.SetActive (curAState);
            //  if (skipTo == 0)
            // curAO.speed = 0;
            curAO.speed = skipTo == 0 ? 0 : 10000f;


        }
    }
    void SetCameraPositionV2(Step s) {

        //Debug.Log(">>>>>> .....................>>>>   Set Camera");
        // stepsMgr.mainCamEx.NewPosTarget(s.overrideCameraPosition.position, s.lookAtPoint);
        if (stepsMgr == null)
            stepsMgr = StepsManager.Instance;
            //stepsMgr.mainCamEx.CamPOS = s.overrideCameraPosition;// Cam Pos set
            if (s.lookAtPoint != null && s.overrideCameraPosition != null) {
              //  Debug.Log("<color=blue>[NEW METHOD!]</color>");
               // stepsMgr.mainCamEx.NewPosTarget(s.overrideCameraPosition.position, s.lookAtPoint);
            GetAnimatedObject(s);

        } else {
               /* if (s.lookAtPoint != null) {
                    stepsMgr.freeCamPivot.transform.position = s.lookAtPoint.transform.position;
                   // stepsMgr.mainCamEx.target = s.lookAtPoint;
                    
                }*/
                if (s.overrideCameraPosition != null) {
                  //  Debug.Log("USE NEW cam");
                    if (s.lookAtPoint != null) {
                        //stepsMgr.mainCamEx.distance = Vector3.Distance(s.lookAtPoint.transform.position, s.overrideCameraPosition.transform.position);
                    }
                  //  stepsMgr.mainCamEx.NewPosRot(s.overrideCameraPosition.position, s.overrideCameraPosition.localRotation);
                }
            }

    }
    void ToggleObjects(GameObject[] objs, bool isOn) {
        if (objs != null)
            foreach (GameObject g in objs) {
                if (g != null)
                {
                    Debug.Log("  Off  Gameobject "+g.name);
                    g.SetActive(isOn);
                }
                else
                    Debug.Log("Null Element found in Step:" + currentStep + " of " + currentProcess.ToString());
            }
        else
            Debug.LogError("Null Element found in Step:" + currentStep + " of " + currentProcess.ToString() + " Inst: " + GetCurrentStep().stepInstructions);
    }

    void ToggleHighlight(GameObject[] obj, bool add) {
        if (add) {
            if (keepHighlightForAnimation)
            {
              //  highlighter.RemoveHighLight();
               // modelTrackingTransperant();
            }
               
            if (obj != null)
            {
             //   highlighter.Highlight(obj);
               //modelTrackingHighlighed(obj);
            }
            else
            {
                Debug.LogError("null highlight list found in " + currentStep + " of " + currentProcess + " inst " + GetCurrentStep().locateObjectText);
            }
               
        } else {
            if (!keepHighlightForAnimation)
            {
              //  highlighter.RemoveHighLight();
              //  modelTrackingTransperant();
            }
               
        }
    }
  /*  void modelTrackingTransperant()
    {
        if (SceneManager.GetSceneByName("ModelTracking").isLoaded)
        {
            foreach (Step st in steps)
            {
                // Debug.Log("  Steps Number  " +st.locateObjectText);
                if (st.objsToHighlight.Length > 0)
                {
                    for (int i = 0; i < st.objsToHighlight.Length; i++)
                    {
                        if (st.objsToHighlight[i] != null)
                        {
                            if (st.objsToHighlight[i].GetComponent<MaterialSwitch>() == null)
                            {
                                st.objsToHighlight[i].AddComponent<MaterialSwitch>().enabled = true;
                                st.objsToHighlight[i].GetComponent<MaterialSwitch>().transperantMat();
                            }
                            else
                            {
                                st.objsToHighlight[i].GetComponent<MaterialSwitch>().transperantMat();
                            }
                        }

                    }
                }
            }
        }
       
    }*/
/*    void modelTrackingHighlighed(GameObject[] gos)
    {
        if (SceneManager.GetSceneByName("ModelTracking").isLoaded)
        {
            foreach (GameObject item in gos)
            {
                MaterialSwitch materialSwitch = item.GetComponent<MaterialSwitch>();
                for (int i = 0; i < materialSwitch.meshWithMat.Count; i++)
                {
                    Material[] mats = new Material[materialSwitch.meshWithMat[i].meshRenderers.materials.Length];
                    for (int j = 0; j < mats.Length; i++)
                    {
                        mats[i] = stepsMgr.modelTrackHighLighted;
                    }
                    materialSwitch.meshWithMat[i].meshRenderers.materials = mats;
                }
            }
        }
      
    }*/
    void ToggleCurvedLineDelay(GameObject[] obj, bool isUsing) {
        if (obj != null)
        {
            if (isUsing)
            {
             /*   foreach (GameObject go in obj)
                {
                    go.GetComponent<IndieMarc.CurvedLine.CurvedLine3D>().refresh_rate = 0.015f;
                }*/
            }
            else
            {
              /*  foreach (GameObject go in obj)
                {
                    go.GetComponent<IndieMarc.CurvedLine.CurvedLine3D>().refresh_rate = 1f;
                }*/
            }
        }
    }

    public void ShowRestartBtn(Step s) {
        //stepsMgr.restartBtnContainer.SetActive(partLocated && s.animatedObject != null);
    }
    public bool PartLocated() => partLocated;
    void _updateStep()
        => stepUpdated?.Invoke(currentStep);

    static void RemoveSpriteByName(List<Sprite> sprites, string nameToRemove)
    {
        // Find the index of the sprite with the specified name
        int index = sprites.FindIndex(sprite => sprite.name == nameToRemove);

        // Check if the sprite was found
        if (index != -1)
        {
            // Remove the sprite at the found index
            sprites.RemoveAt(index);
        }
        else
        {
            Console.WriteLine("Sprite not found.");
        }
    }
    
    void ShowStaticContent(Step s) {
        // set the description
       // Debug.Log("SpriteShow static entered");
       // stepsMgr.stepDesc.text = s.locateObjectText;

        //stepsMgr.toolPopup.SetActive(s.specialToolName != "" && s.specialToolName != "-");
        //if (stepsMgr.toolPopup != null)
        //    if(stepsMgr.toolName_txt!=null)
        //    stepsMgr.toolName_txt.text = s.specialToolName;

        bool hasToolSprites = false;
        if (s.toolSprite != null)
            hasToolSprites = s.toolSprite.Length > 0;
        // NEW UI ANIMATION
        //iTween.MoveTo(stepsMgr.toolList.gameObject, iTween.Hash("x", hasToolSprites ? 20f : -280f, "time", 0.5f, "delay", 1f));

       // stepsMgr.toolList.gameObject.SetActive(hasToolSprites);
        if (hasToolSprites) {
            Debug.Log("Has tool sprites found");
            //stepsMgr.toolList.Load(s.toolSprite, true);
        }
        else
        {
          /*  if (stepsMgr.toolList.transform.childCount>0)
            {
                foreach (Transform child in stepsMgr.toolList.transform)
                {
                    Destroy(child.gameObject);
                }
            }*/
          
        }
        //  toolName.text = s.specialToolName;
        //  toolImage.sprite = s.toolSprite;
        //if(stepsMgr.slideShow!=null)
        //stepsMgr.slideShow.Load(s.toolSprite);

        //shafi       //stepsMgr.torquePopup.SetActive(s.torque != "");

       // iTween.MoveTo(stepsMgr.torquePopup.transform.GetChild(0).gameObject, iTween.Hash("x", s.torque != "" ? Screen.width : Screen.width + 700f, "time", 0.5f, "delay", 0.5f));
     // shafi         // stepsMgr.torqueValueTxt.text = s.torque;
        //   torqueValueTxt.text = s.torque;

        // set the caution text
        bool showCaution = false;
        s.cautionNotes = s.cautionNotes.Trim();
        if (s.cautionNotes != null) {
            //showCaution = (s.cautionNotes.Length > 0);
            if (cautionCanvas == null)
                cautionCanvas = stepsMgr.caution_popup.GetComponent<CanvasGroup>();

            if (cautionCanvas) {
                cautionCanvas.alpha = 1;
                //    stepsMgr.caution_popup.SetActive(showCaution);
               // stepsMgr.mainCamEx.PanLeft();
            }

        }
        //     iTween.MoveTo(stepsMgr.caution_popup.transform.GetChild(0).gameObject, iTween.Hash("x", showCaution ? Screen.width : Screen.width + 700f, "time", 0.5f, "delay", 0.2f));
        //iTween.MoveTo(stepsMgr.caution_popup.transform.GetChild(0).gameObject, iTween.Hash("x", showCaution ? Screen.width : Screen.width + 700f, "time", 0.5f, "delay", 0.2f));
        // stepsMgr.caution_txt.text = s.cautionNotes.Replace(".", ".\n");
        if (showCaution)
        {
            string[] cautionLines = s.cautionNotes.ToString().Split(new string[] { ":::" }, System.StringSplitOptions.None);
           // stepsMgr.caution_txt.Load(cautionLines);
        }

        if (stepsMgr.oneTimeUse != null)
            stepsMgr.oneTimeUse.SetActive(s.stepInstructions.ToUpper().Contains("RENEW"));
        if (stepsMgr.infoPanel != null)
            stepsMgr.infoPanel.SetActive(s.cautionNotes != "" || s.torque != "");
        if (s.locateObjectText.Trim() == "")
        {
            Debug.Log("Locate Step is Empty. Skipping it to animated part");
            LocateStepEmpty = true;
            CompleteV2();
        }
    }
    #endregion
    #region FallBack
    //--------------------------------------------------------------------------------------
    void PreviousAnim() {
        StartCoroutine(TriggerPrevAnim());

    }
    IEnumerator TriggerPrevAnim() {
        if (currentStep < 1) {
            yield break;
        }
        if (currentProcess == Process.Dismantling) {
            for (int i = 0; i < 2; i++) {
                // reset the objects to their original state
                // enable the objects which are supposed to be disabled
                ToggleObjects(steps[currentStep].objsToDisable, true);
                // disable the objects which were enabled for this step
                ToggleObjects(steps[currentStep].objsToEnable, false);

                // disable highlight for current step
                ToggleHighlight(steps[currentStep].objsToHighlight, false);

                // reset at first frame
                if (steps[currentStep].animatedObject != null) {
                    steps[currentStep].animatedObject.SetTrigger("idle");
                    yield return new WaitForSeconds(1f);
                }
                currentStep--;
            }
            // set the cam position
            //SetCameraPosition(steps[currentStep + 1]);

            // stop if audio is playing
            if (GetComponent<AudioSource>() != null) {
                if (GetComponent<AudioSource>().isPlaying) {
                    GetComponent<AudioSource>().Stop();
                }
            }
            partLocated = false;
            NextAnim();
        } else if (currentProcess == Process.Assembly) {
            for (int i = 0; i < 2; i++) {
                // reset the objects to their original state
                // enable the objects which are supposed to be disabled
                ToggleObjects(assemblySteps[currentStep].objsToDisable, true);
                // disable the objects which were enabled for this step
                ToggleObjects(assemblySteps[currentStep].objsToEnable, false);

                // disable highlight for current step
                ToggleHighlight(assemblySteps[currentStep].objsToHighlight, false);

                // reset at first frame
                if (assemblySteps[currentStep].animatedObject != null) {

                    assemblySteps[currentStep].animatedObject.SetTrigger(assemblySteps[currentStep].animTriggerName);
                    AnimatorStateInfo info = assemblySteps[currentStep].animatedObject.GetCurrentAnimatorStateInfo(0);

                    assemblySteps[currentStep].animatedObject.Play(info.nameHash, 0, 0f);
                    //yield return new WaitForEndOfFrame();
                    assemblySteps[currentStep].animatedObject.speed = 0f;
                    assemblySteps[currentStep].animatedObject.enabled = false;
                    yield return new WaitForSeconds(0.1f);
                }
                currentStep--;
            }

            // set the cam position
            //SetCameraPosition(assemblySteps[currentStep + 1]);

            // stop if audio is playing
            if (GetComponent<AudioSource>().isPlaying) {
                GetComponent<AudioSource>().Stop();
            }
            partLocated = false;
            NextAnim();
        }
    }
    void LocatePart() {
        stepsMgr.prevBtn.GetComponent<Button>().interactable = false;
        if (currentProcess == Process.Dismantling) {
            if (currentStep > -1) {
                // disable the previous object
                ToggleObjects(steps[currentStep].objsToDisable, false);
                ToggleObjects(steps[currentStep].objsToEnable, true);
            }

            if (currentStep >= steps.Count - 1) {
                // if reached end then start assembly process
                Debug.Log("Reached End");
                currentProcess = Process.Assembly;
                currentStep = -1;
                partLocated = false;
                return;
            }

            // set the next step as the current step
            currentStep++;

            ShowRestartBtn(steps[currentStep]);

            // check if current and the following steps are locked
            while (steps[currentStep].isLocked && currentStep < steps.Count - 1) {
                // disable objects
                ToggleObjects(steps[currentStep].objsToDisable, false);

                // go to next step
                currentStep++;
                Debug.Log("SKIPPED: " + steps[currentStep].locateObjectText);
            }

            if (currentStep < steps.Count) {
                // set the cam position
                SetCameraPosition(steps[currentStep]);

                // show description
                ShowStaticContent(steps[currentStep]);

                // enable highlight
                ToggleHighlight(steps[currentStep].objsToHighlight, true);

               // ToggleCurvedLineDelay(steps[currentStep - 1].curvedLineObjs, false);
                // start at first frame
                if (steps[currentStep].animatedObject != null) {
                    steps[currentStep].animatedObject.SetTrigger(steps[currentStep].animTriggerName);
                    steps[currentStep].animatedObject.speed = 0;
                    steps[currentStep].animatedObject.enabled = false;
                }
            }
        } else if (currentProcess == Process.Assembly) {
            // fast forward to completion if the user clicks next before the animation is complete
            if (currentStep > -1) {
                if (assemblySteps[currentStep].animatedObject != null) {
                    assemblySteps[currentStep].animatedObject.speed = 100f;
                    // assemblySteps[currentStep].animatedObject.playbackTime = 100f;
                    //      currentAnim.playbackTime = 0.998f;
                    assemblySteps[currentStep].animatedObject.StopPlayback();
                    StartCoroutine(ResetAnimSpeed(assemblySteps[currentStep].animatedObject));
                }
            }
            currentStep++;

            // check if current and the following steps are locked
            while (assemblySteps[currentStep].isLocked) {
                foreach (GameObject g in assemblySteps[currentStep].objsToEnable) {
                    g.SetActive(true);
                }
                currentStep++;
                Debug.Log("SKIPPED: " + assemblySteps[currentStep].locateObjectText);
            }

            // set the cam position
            SetCameraPosition(assemblySteps[currentStep]);

            // show description, caution, torque values
            ShowStaticContent(assemblySteps[currentStep]);

            // enable highlight
            ToggleHighlight(assemblySteps[currentStep].objsToHighlight, true);

            ToggleCurvedLineDelay(steps[currentStep - 1].curvedLineObjs, false);

            // enable the objects required for the current step
            ToggleObjects(assemblySteps[currentStep].objsToEnable, true);
            ToggleObjects(assemblySteps[currentStep].objsToDisable, false);


            // start at first frame
            if (assemblySteps[currentStep].animatedObject != null) {
                Debug.Log("Triggered: " + assemblySteps[currentStep].animTriggerName);
                assemblySteps[currentStep].animatedObject.SetTrigger(assemblySteps[currentStep].animTriggerName);
                //  assemblySteps[currentStep].animatedObject.playbackTime = 0.1f;
                assemblySteps[currentStep].animatedObject.enabled = false;
                assemblySteps[currentStep].animatedObject.speed = 0f;
            }
        }
    }
    void CompleteTheStep() {
        stepsMgr.prevBtn.GetComponent<Button>().interactable = true;
        if (currentProcess == Process.Dismantling) {
            // disable highlight for current step
            ToggleHighlight(steps[currentStep].objsToHighlight, false);

            ToggleCurvedLineDelay(steps[currentStep].curvedLineObjs, true);

            // set the description
            stepDesc.text = steps[currentStep].stepInstructions;

            // Play VO
            if (steps[currentStep].voiceOver != null && GetComponent<AudioSource>() != null) {
                GetComponent<AudioSource>().clip = steps[currentStep].voiceOver;
                GetComponent<AudioSource>().Play();
            }

            // start animation
            if (steps[currentStep].animatedObject != null) {
                steps[currentStep].animatedObject.enabled = true;
                steps[currentStep].animatedObject.speed = auto ? animSpeed : 1f;
                currentAnim = steps[currentStep].animatedObject;
            } else {
                StartCoroutine(HighlightNextBtn(1f));
                StartCoroutine(SkipToNext(textDuration));
            }
        } else if (currentProcess == Process.Assembly) {
            // disable highlight for current step
            ToggleHighlight(assemblySteps[currentStep].objsToHighlight, false);
            // set the description
            stepDesc.text = assemblySteps[currentStep].stepInstructions;

            ToggleCurvedLineDelay(steps[currentStep].curvedLineObjs, true);
            // Play VO
            if (assemblySteps[currentStep].voiceOver != null && GetComponent<AudioSource>() != null) {
                GetComponent<AudioSource>().clip = assemblySteps[currentStep].voiceOver;
                GetComponent<AudioSource>().Play();
            }
            // start animation
            if (assemblySteps[currentStep].animatedObject != null) {
                //assemblySteps[currentStep].animatedObject.SetTrigger(assemblySteps[currentStep].animTriggerName);
                assemblySteps[currentStep].animatedObject.enabled = true;
                assemblySteps[currentStep].animatedObject.speed = auto ? animSpeed : 1f;
                currentAnim = assemblySteps[currentStep].animatedObject;
            } else {
                StartCoroutine(HighlightNextBtn(1f));
                StartCoroutine(SkipToNext(textDuration));
            }
        }
    }

    IEnumerator SkipToNext(float t) {
        yield return new WaitForSeconds(t);
        if (auto) {
            NextAnim();
        }
        triggeredNext = false;
    }

    void NextAnim() {
        // check and stop highlighting the button
        if (stepsMgr.nextBtn.GetComponent<iTween>() != null) {
            DestroyImmediate(stepsMgr.nextBtn.GetComponent<iTween>());
            stepsMgr.nextBtn.transform.localScale = Vector3.one;
        }
        if (!partLocated) {
            partLocated = true;
            LocatePart();
            StartCoroutine(SkipToNext(textDuration));
        } else {
            partLocated = false;
            CompleteTheStep();
        }
    }

    IEnumerator ResetAnimSpeed(Animator anim) {
        Debug.Log("Reset was called");
        yield return new WaitForSeconds(2f);
        anim.speed = 1f;
    }

    // ---------------------------------------------------------------------------------

    void SetCameraPosition(Step s) {
        if (s.overrideCameraPosition != null) {
            // PIP camera
         /*   if (stepsMgr.fixedCamera != null) {
                stepsMgr.fixedCamera.transform.position = s.overrideCameraPosition.position;
                stepsMgr.fixedCamera.transform.localEulerAngles = s.overrideCameraPosition.localEulerAngles;
            }
            // main camera
            stepsMgr.mainCamera.transform.position = s.overrideCameraPosition.position;
            stepsMgr.mainCamera.transform.eulerAngles = s.overrideCameraPosition.localEulerAngles;*/
        }
    /*    if (s.lookAtPoint != null) {
          // stepsMgr.mainCamEx.target = s.lookAtPoint.transform;
           stepsMgr.freeCamPivot.transform.position = s.lookAtPoint.transform.position;*/

            GetAnimatedObject(s);


      //  }
    }
    // ---------------------------------------------------------------------------------



    public void GetAnimatedObject(Step s)
    {
        /*if (s.objsToHighlight.Length > 0)
        {
            foreach (GameObject item in s.objsToHighlight)
            {
                if (item.GetComponent<MeshRenderer>())
                {
                    stepsMgr.modeltrackPositions.animatedObject = item.transform;
                    break;
                }
            }
            stepsMgr.modeltrackPositions.animatedObject = s.lookAtPoint;
        }        
        else*/
           // stepsMgr.modeltrackPositions.animatedObject = s.lookAtPoint;
    }



    [ContextMenu("Export Steps to CSV")]
    void ExportToCSV() {
        List<string> dSteps = new List<string>();
        dSteps.Add("Sr.No.,Locate part instruction,Step Instruction,isLocked,Tool Name");
        for (int i = 0; i < steps.Count; i++) {
            dSteps.Add(
                (i + 1) + ","
                + steps[i].locateObjectText.Replace(",", "") + ","
                + steps[i].stepInstructions.Replace(",", "") + ","
                + steps[i].isLocked.ToString() + ","
                + steps[i].specialToolName.Replace(",", "")
                );
        }
        string dStepPath = Application.dataPath + Path.DirectorySeparatorChar + "Dump" + Path.DirectorySeparatorChar + "DismantlingSteps.csv";
        File.WriteAllLines(dStepPath, dSteps.ToArray());

        List<string> aSteps = new List<string>();
        aSteps.Add("Sr.No.,Locate part instruction,Step Instruction,isLocked,Tool Name");
        for (int i = 0; i < assemblySteps.Count; i++) {
            aSteps.Add(
                (i + 1) + ","
                + assemblySteps[i].locateObjectText.Replace(",", "") + ","
                + assemblySteps[i].stepInstructions.Replace(",", "") + ","
                + assemblySteps[i].isLocked.ToString() + ","
                + assemblySteps[i].specialToolName.Replace(",", "")
                );
        }
        string aStepPath = Application.dataPath + Path.DirectorySeparatorChar + "Dump" + Path.DirectorySeparatorChar + "Assembly.csv";
        File.WriteAllLines(aStepPath, aSteps.ToArray());
    }

    [ContextMenu("Import Assembly Steps")]
    void ImportAssemblySteps() {
        TextAsset data = Resources.Load(assemblyStepsCSV) as TextAsset;
        string[] lines = Regex.Split(data.text, System.Environment.NewLine);
        for (int i = 1; i < lines.Length; i++) {
            string srNo = Regex.Split(lines[i], ",")[0];
            string locateText = Regex.Split(lines[i], ",")[1];
            string stepInstr = Regex.Split(lines[i], ",")[2];
            string isLocked = Regex.Split(lines[i], ",")[3];
            string toolName = Regex.Split(lines[i], ",")[4];
            string renew = Regex.Split(lines[i], ",")[5];
            string torque = Regex.Split(lines[i], ",")[6];

            assemblySteps[i - 1].locateObjectText = locateText;
            assemblySteps[i - 1].stepInstructions = stepInstr;
            assemblySteps[i - 1].specialToolName = toolName;
            assemblySteps[i - 1].torque = torque;
        }
    }

    [ContextMenu("Import Dismantling Steps")]
    void ImportDismantlingSteps() {
        TextAsset data = Resources.Load(dismantlingStepsCSV) as TextAsset;
        string[] lines = Regex.Split(data.text, System.Environment.NewLine);
        for (int i = 1; i < lines.Length; i++) {
            string srNo = Regex.Split(lines[i], ",")[0];
            string locateText = Regex.Split(lines[i], ",")[1];
            string stepInstr = Regex.Split(lines[i], ",")[2];
            string isLocked = Regex.Split(lines[i], ",")[3];
            string toolName = Regex.Split(lines[i], ",")[4];
            string renew = Regex.Split(lines[i], ",")[5];
            string torque = Regex.Split(lines[i], ",")[6];

            steps[i - 1].locateObjectText = locateText;
            steps[i - 1].stepInstructions = stepInstr;
            steps[i - 1].specialToolName = toolName;
            steps[i - 1].torque = torque;
        }
    }
    #endregion
    [ContextMenu("Update Step Numbers")]
    void AddNumbers() {
        int i = 0;
        foreach (Step s in assemblySteps) {
            s.debugStepNum = i;
            i++;
        }
        i = 0;
        foreach (Step s in steps) {
            s.debugStepNum = i;
            i++;
        }
    }
}
#if UNITY_EDITOR
/*[CustomEditor(typeof(Steps))]
[CanEditMultipleObjects]
public class StepsEditor : Editor {
    int ffStepNum;
    bool showStep;
    int cur;
    SerializedProperty stepObj;
    public override void OnInspectorGUI() {


        Steps steps = (Steps)target;
        DrawDefaultInspector();
        EditorGUILayout.Space(10);

        if (steps.useStepPlayer) {
            if (EditorApplication.isPlaying) {
                EditorGUILayout.LabelField("Step Player", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Now Playing " + steps.currentProcess.ToString() + ": " + steps.currentStep, EditorStyles.largeLabel);
                if (steps.currentProcess != Steps.Process.Dismantling)
                    if (GUILayout.Button("Dismantling"))
                        steps.Dismantling();
                if (steps.currentProcess != Steps.Process.Assembly)
                    if (GUILayout.Button("Assembly"))
                        steps.Assembly();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.HelpBox("Previous, Replay ,Next", MessageType.None);
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button(" << ")) {
                    steps.Previous();
                    steps.UpdateCurrentStepData();
                }
                if (GUILayout.Button(" REP ")) {
                    steps.Replay();
                    steps.UpdateCurrentStepData();
                }
                if (GUILayout.Button(" >> ")) {
                    steps.Next();
                    steps.UpdateCurrentStepData();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(10);
                EditorGUILayout.HelpBox("Fast Forward Or Rewind to any step", MessageType.None);
                EditorGUILayout.BeginHorizontal();
                ffStepNum = EditorGUILayout.IntField(ffStepNum, GUILayout.MinWidth(30));
                if (GUILayout.Button(" GO ")) {
                    steps.FastForwardTo(ffStepNum);
                    steps.UpdateCurrentStepData();
                }
                EditorGUILayout.EndHorizontal();

            } else
                EditorGUILayout.HelpBox("Step Controls are Only Visible during Play mode", MessageType.Info);
        } else {
            EditorGUILayout.HelpBox("enable Step Player to Toggle Player", MessageType.None);
            if (GUILayout.Button("Enable Step Player"))
                steps.useStepPlayer = true;

        }
    }
    void AddOutlineEffect() {
        cakeslice.OutlineEffect outlineEffect = FindObjectOfType<cakeslice.OutlineEffect>();
        if (outlineEffect == null) {
            ExteriorCam extCam = FindObjectOfType<ExteriorCam>();
            if (extCam != null) {
                Debug.Log("Added OutlineEffect,Color Pulse", extCam.gameObject);
                extCam.gameObject.AddComponent<cakeslice.OutlineEffect>();
                extCam.gameObject.AddComponent<OutlineFillColorPulse>();

            }
        }
    }
    void AddPartHighlighter() {
        PartHighlighter hl = FindObjectOfType<PartHighlighter>();
        if (hl == null) {
            StepsManager stpMgr = FindObjectOfType<StepsManager>();
            if (stpMgr) {
                Debug.Log("Added PartHighlighter", stpMgr.gameObject);
                stpMgr.gameObject.AddComponent<PartHighlighter>();
            }
        }
    }
    private void OnEnable() {
        Steps steps = (Steps)target;

        if (steps.GetPartHighlighter() == null) {
            Debug.Log("IS Null");
            AddOutlineEffect();
            AddPartHighlighter();
        }
    }
}*/
#endif