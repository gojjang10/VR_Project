using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Animator animator;

    public void OpenDoor()
    {
        animator.SetBool("Open", true);
    }
}
