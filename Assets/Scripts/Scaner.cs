using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaner : MonoBehaviour
{

    private GameObject curObject;
    private void OnTriggerEnter(Collider other)
    {
       

        if(other.gameObject.CompareTag("Key"))
        {
            GazeTest instance = other.gameObject.GetComponent<GazeTest>();
            instance.scanText.SetActive(true);
            curObject = instance.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            GazeTest instance = other.gameObject.GetComponent<GazeTest>();
            instance.scanText.SetActive(false);
            curObject = null;
        }
    }

    private void OnDisable()
    {
        if(curObject != null)
        {
            GazeTest instance = curObject.GetComponent<GazeTest>();
            instance.scanBar.fillAmount = 0;
            instance.scanText.SetActive(false);
        }
    }
}
