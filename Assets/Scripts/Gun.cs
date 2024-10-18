using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform muzzlePoint;
    [SerializeField] float bulletSpeed;

    [SerializeField] InputActionReference trigger;
    [SerializeField] InputActionReference grip;

    [SerializeField] ObjectPool bulletPool;

    [SerializeField] GameObject gazeInteractor;
    [SerializeField] GameObject gazeStabilized;

    private void OnEnable()
    {
        trigger.action.Enable();
        grip.action.Enable();
    }

    private void Update()
    {
        if(trigger.action.WasPressedThisFrame())
        {
            Fire();
            Debug.Log("발사");
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
    }

    private void OnDisable()
    {
        trigger.action.Disable();
        grip.action.Disable();
    }

    #region 발사 관련
    public void Fire()
    {
        PooledObject instance = bulletPool.GetPool(muzzlePoint.position, muzzlePoint.rotation);

        if(instance == null )
        {
            Debug.Log("총알이 풀에서 다 나왔습니다");
            return;
        }

        if (instance.TryGetComponent(out Rigidbody rb))
        {
            SetForce(rb);
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
    }

    public void ScanOff()
    {
        gazeInteractor.SetActive(false);
        gazeStabilized.SetActive(false);
    }
    #endregion
}
