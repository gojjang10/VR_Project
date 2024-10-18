using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LeftHand : MonoBehaviour
{
    [SerializeField] InputActionReference trigger;
    [SerializeField] InputActionReference grip;

    [SerializeField] Animator animator;

    private void OnEnable()
    {
        trigger.action.Enable();
        grip.action.Enable();
    }

    private void Update()
    {
        float triggerValue = trigger.action.ReadValue<float>();
        animator.SetLayerWeight(2, triggerValue);

        float gripValue = grip.action.ReadValue<float>();
        animator.SetLayerWeight(3, gripValue);
    }

    private void OnDisable()
    {
        trigger.action.Disable();
        grip.action.Disable();
    }
}
