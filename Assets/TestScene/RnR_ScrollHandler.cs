/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollHandler : MonoBehaviour
{
    public RectTransform content;
    public Scrollbar scrollBar;
    public int numberOfItems;
    public int currentStep;
    [SerializeField]private Steps stepScript;

    // Start is called before the first frame update
    void OnEnable()
    {
        stepScript = FindObjectOfType<Steps>();
        currentStep = 0;
        currentContentPosition = Vector3.zero;
        currentContentPosition = content.localPosition;
    }

    private void OnDisable()
    {
        currentContentPosition = Vector3.zero;
        content.localPosition = currentContentPosition;
    }

    [SerializeField]Vector3 currentContentPosition;
    public void SetScrollPosNext() {

        currentStep = stepScript.currentStep;
        numberOfItems = stepScript.steps.Count;
        if (currentStep > 0)
        {
            currentContentPosition += new Vector3(0, GetContentSizetoscroll(currentStep, 20), 0);
            content.localPosition = currentContentPosition;
        }
    }

    float GetContentSizetoscroll(int stepNumber, int offset)
    {
        float scrollValue = 0;
        scrollValue = stepScript.loadedStepsInfo[currentStep].GetComponent<RectTransform>().sizeDelta.y;
        Debug.Log("Scroll Up position -- " + scrollValue);
        return scrollValue + offset;
    }

    public void SetScrollPosPrev() {

        currentStep = stepScript.currentStep;
        numberOfItems = stepScript.steps.Count;
        if (currentStep > 0)
        {
            currentContentPosition -= new Vector3(0, GetContentSizetoscroll(currentStep, 20), 0);

        //712 -= 224 = 488
            content.localPosition = currentContentPosition;
        }
    }
    float GetContentSizetoscroll2(int stepNumber, int offset)
    {
        float scrollValue = 0;
        scrollValue = stepScript.loadedStepsInfo[currentStep].GetComponent<RectTransform>().sizeDelta.y;
        Debug.Log("Scroll Dp position -- " + scrollValue + offset);
        return scrollValue + offset;
    }


}*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RnR_ScrollHandler : MonoBehaviour
{
    public RectTransform content;
    public Scrollbar scrollBar;
    public int numberOfItems;
    public int currentStep;
    [SerializeField] private Steps stepScript;
    // Start is called before the first frame update
    void OnEnable()
    {
        stepScript = FindObjectOfType<Steps>();
        currentStep = 0;
    }
    public void SetScrollPosNext()
    {
        currentStep = stepScript.currentStep;
        numberOfItems = stepScript.steps.Count;
        if (currentStep > 0)
        {
            content.localPosition = new Vector3(content.localPosition.x, GetContentSizetoscroll(currentStep), content.localPosition.z);
        }
    }
    float GetContentSizetoscroll(int stepNumber)
    {
        float scrollValue = 0;
       // Debug.LogError(" abs " + Mathf.Abs(stepScript.loadedStepsInfo[currentStep].GetComponent<RectTransform>().localPosition.y));
        scrollValue = Mathf.Abs(stepScript.loadedStepsInfo[currentStep].GetComponent<RectTransform>().localPosition.y) - stepScript.loadedStepsInfo[currentStep].GetComponent<RectTransform>().sizeDelta.y;
        Debug.Log("Scroll Up position -- " + scrollValue);
        return scrollValue;
    }
    public void SetScrollPosPrev()
    {
        if (!stepScript)
        {
            return;
        }
        currentStep = stepScript.currentStep;
        numberOfItems = stepScript.steps.Count;
        if (currentStep == 0 || currentStep == -1)
        {
            content.localPosition = new Vector3(content.localPosition.x, 0, content.localPosition.z);
        }
        if (currentStep > 0)
        {
            content.localPosition = new Vector3(content.localPosition.x, GetContentSizetoscroll(currentStep), content.localPosition.z);
        }
    }
}