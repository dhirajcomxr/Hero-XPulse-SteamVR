using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public TextMeshProUGUI cautionNote;
    public Button previousButton;
    public Button nextButton;
    public Button retryButton;



    [Space(10)]
    public UnityEvent previousButtonAction;
    public UnityEvent nextButtonAction;
    public UnityEvent retryButtonAction;

    private void Start()
    {
        previousButton.onClick.AddListener(() => { previousButtonAction.Invoke(); });
        nextButton.onClick.AddListener(() => { nextButtonAction.Invoke(); });
        retryButton.onClick.AddListener(() => { retryButtonAction.Invoke(); });
    }

    public void SetCautionNote(string text,Color col)
    {
        cautionNote.text = $"<color={col}>{text}</color>";
    }

    public void PrintMsg(string t)
    {
        Debug.Log(t);
    }
}
