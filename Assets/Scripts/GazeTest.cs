using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeTest : MonoBehaviour
{
    public void GazeOn()
    {
        showInfoRoutine = StartCoroutine(ShowinfoRoutine());
    }
    public void GazeOff()
    {
        StopCoroutine(showInfoRoutine);
    }

    Coroutine showInfoRoutine;

    IEnumerator ShowinfoRoutine()
    {
        yield return new WaitForSeconds(2f);

        Debug.Log("정보표시");
        Destroy(gameObject);
    }
}
