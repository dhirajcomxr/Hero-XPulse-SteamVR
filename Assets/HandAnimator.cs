using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimator : MonoBehaviour
{
    [SerializeField] private InputActionProperty triggerAction;
    [SerializeField] private InputActionProperty gripAction;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        float trigVal = triggerAction.action.ReadValue<float>();
        float gripVal = gripAction.action.ReadValue<float>();

        anim.SetFloat("Trigger", trigVal);
        anim.SetFloat("Grip", gripVal);
    }

}
