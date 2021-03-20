using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootImpactController : NetworkBehaviour
{
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
        if (other!=null)
        {
            if (other.transform.GetComponent<HealthController>()!=null)
            {
                HealthController healthControllerTarget= other.transform.GetComponent<HealthController>();
                healthControllerTarget.TakeDamage(damageGet);
                Destroy(this.gameObject);
            }
        }
    }



}
