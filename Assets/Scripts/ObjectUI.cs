using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectUI : MonoBehaviour
{
    [SerializeField] Camera targetCamera;

    private void Update()
    {
        Vector3 direction = (transform.position - targetCamera.transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
