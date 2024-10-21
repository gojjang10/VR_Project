using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GazeTest : MonoBehaviour
{
    // 문을 열기 위한 이벤트 작성
    [SerializeField] UnityEvent interacted;
    [SerializeField] public GameObject scanText;
    [SerializeField] public Image scanBar;

    private void Start()
    {
        scanText.SetActive(false);
    }

    // 테스트용 Gaze 상호작용 시작시
    public void GazeOn()
    {
        showInfoRoutine = StartCoroutine(ShowinfoRoutine());
    }
    // 테스트용 Gaze 상호작용 끝날시
    public void GazeOff()
    {
        StopCoroutine(showInfoRoutine);
    }

    // 코루틴 저장 변수
    Coroutine showInfoRoutine;

    // 정보표시 후 이벤트 발생 코루틴
    IEnumerator ShowinfoRoutine()
    {
        float time = 0f;
        float duration = 2f;

        scanBar.fillAmount = 0;

        while(time < duration)
        {
            time += Time.deltaTime;
            scanBar.fillAmount = Mathf.Clamp01(time / duration);
            yield return null;
        }


        //yield return new WaitForSeconds(2f);

        Debug.Log("정보표시");
        interacted?.Invoke();
        Destroy(gameObject);
    }
}
