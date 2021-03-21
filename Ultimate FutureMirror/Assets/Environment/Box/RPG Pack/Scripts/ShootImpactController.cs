using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootImpactController : NetworkBehaviour
{
    [Header("Variable responsible for ricochet Impact")]
    [SerializeField] private bool isEnableRicocher = false;

    private GameObject objectFromShoot = null;
    private GunKind gunKind = GunKind.None;
    private float damageGet;
    private HealthController healthControllerTarget=null;


    public void ShootImpactControllerProperties(GameObject objectFromShoot, GunKind gunKind, float damageGet)
    {
        this.objectFromShoot = objectFromShoot;
        this.gunKind = gunKind;
        this.damageGet = damageGet;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other!=null&&other.transform.GetComponent<HealthController>()!=null)
        {
            if (isEnableRicocher)
            {
                TakeDamageMechanic(other);
            }
            else
            {
                if (other.transform.GetComponent<HealthController>() == objectFromShoot.GetComponent<HealthController>())
                    return;
                else
                {
                    TakeDamageMechanic(other);
                }
            }
        }
    }
    private void TakeDamageMechanic(Collider other)
    {
        HealthController healthControllerTarget = other.transform.GetComponent<HealthController>();
        healthControllerTarget.TakeDamage(damageGet);
        Destroy(this.gameObject);
    }



}
