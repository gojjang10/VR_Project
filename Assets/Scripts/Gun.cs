using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform muzzlePoint;
    [SerializeField] float bulletSpeed;

    [SerializeField] InputActionReference trigger;
    [SerializeField] InputActionReference grip;
    [SerializeField] InputActionReference aButton;

    [SerializeField] ObjectPool bulletPool;
    [SerializeField] int curBullet = 20;
    [SerializeField] int maxBullet = 20;

    [SerializeField] GameObject gazeInteractor;
    [SerializeField] GameObject gazeStabilized;

    [SerializeField] GameObject scanUI;
    [SerializeField] GameObject scanDetectionRange;

    [SerializeField] Animator animator;

    [SerializeField] AudioClip shootSound;

    [SerializeField] Image curBulletUI;

    private void OnEnable()
    {
        trigger.action.Enable();
        grip.action.Enable();
        aButton.action.Enable();
    }

    private void Start()
    {
        scanDetectionRange.SetActive(false);
        scanUI.SetActive(false);
    }

    private void Update()
    {
        curBulletUI.fillAmount = curBullet / 20f;

        if(trigger.action.WasPressedThisFrame())
        {
            if(curBullet != 0)
            {
                Fire();
                Debug.Log("발사");
            }
        }
        else if(trigger.action.WasReleasedThisFrame())
        {
            return;
        }

        if (grip.action.WasPressedThisFrame())
        {
            ScanOn();
            Debug.Log("스캔모드 On");
        }
        else if (grip.action.WasReleasedThisFrame())
        {
            ScanOff();
            Debug.Log("스캔모드 Off");
        }

        if (aButton.action.WasPressedThisFrame())
        {
            Debug.Log("A버튼 누름");
            if(reloadCoroutine == null)
            {
               reloadCoroutine = StartCoroutine(reload());
            }
        }
        else if(aButton.action.WasReleasedThisFrame())
        {
            if(reloadCoroutine != null)
            {
                StopCoroutine(reloadCoroutine);
            }    
        }
    }

    private void OnDisable()
    {
        trigger.action.Disable();
        grip.action.Disable();
        aButton?.action.Disable();
    }

    #region 발사 관련
    public void Fire()
    {
        PooledObject instance = bulletPool.GetPool(muzzlePoint.position, muzzlePoint.rotation);

        if(instance == null)
        {
            Debug.Log("총알이 풀에서 다 나왔습니다");
            Debug.Log("재장전이 필요합니다.");
            return;
        }

        if (instance.TryGetComponent(out Rigidbody rb) && curBullet != 0)
        {
            SetForce(rb);
            curBullet--;
            animator.SetTrigger("Shoot");
            SoundManager.Instance.PlaySFX(shootSound);
        }
    }

    void SetForce(Rigidbody rb)
    {
        rb.velocity = muzzlePoint.forward * bulletSpeed;
    }
    #endregion

    #region 스캔 관련
    public void ScanOn()
    {
        gazeInteractor.SetActive(true);
        gazeStabilized.SetActive(true);
        scanDetectionRange.SetActive(true);
        scanUI.SetActive(true);
    }

    public void ScanOff()
    {
        gazeInteractor.SetActive(false);
        gazeStabilized.SetActive(false);
        scanDetectionRange.SetActive(false);
        scanUI.SetActive(false);
    }
    #endregion

    Coroutine reloadCoroutine;

    #region 재장전
    private IEnumerator reload()
    {
        WaitForSeconds delay = new WaitForSeconds(2);
        Debug.Log("장전중...");
        yield return delay;

        curBullet = maxBullet;

        Debug.Log("장전완료");
        reloadCoroutine = null;
    }
    #endregion
}
