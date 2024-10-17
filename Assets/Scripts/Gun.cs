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
    }

    private void OnDisable()
    {
        trigger.action.Disable();
        grip.action.Disable();
    }

    public void Fire()
    {
        //GameObject instance = Instantiate(bullet, muzzlePoint.position, muzzlePoint.rotation, null);
        PooledObject instance = bulletPool.GetPool(muzzlePoint.position, muzzlePoint.rotation);

        if (instance.TryGetComponent(out Rigidbody rb))
        {
            SetForce(rb);
        }
    }

    void SetForce(Rigidbody rb)
    {
        rb.velocity = muzzlePoint.forward * bulletSpeed;
    }
}
