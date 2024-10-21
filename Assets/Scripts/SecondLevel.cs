using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SecondLevel : MonoBehaviour
{
    [SerializeField] UnityEvent secondLevelClear;

    private void Update()
    {
        if(GameManager.Instance.robots == 0)
        {
            secondLevelClear?.Invoke();
        }
    }
}
