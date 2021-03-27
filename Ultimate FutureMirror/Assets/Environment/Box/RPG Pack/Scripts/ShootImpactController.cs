using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootImpactController : NetworkBehaviour
{
    [Header("Variable responsible for ricochet Impact")]
    [SyncVar]
    [SerializeField] private bool isEnableRicocher = false;
    [SyncVar]
    public uint netIndexObjectFromShoot;
    public HealthController objectFromShoot = null;
    [SyncVar]
    public GunKind gunKind = GunKind.None;
    [SyncVar]
    public float damageGet;


    public void ShootImpactControllerProperties(uint netIndexObjectFromShoot, GunKind gunKind, float damageGet)
    {
        this.netIndexObjectFromShoot = netIndexObjectFromShoot;
        this.gunKind = gunKind;
        this.damageGet = damageGet;

        //Set Game Object From Shot
        if (netIndexObjectFromShoot!=0)
        objectFromShoot = NetworkIdentity.spawned[netIndexObjectFromShoot].GetComponent<HealthController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        #region Take Damage Mechanism
        if (other.transform.GetComponent<HealthController>()!=null)
        {
            if (isEnableRicocher)
            {
                TakeDamageMechanic(other);
            }
            else
            {
                if (objectFromShoot!=null)
                {
                    if (other.transform.GetComponent<HealthController>() == objectFromShoot)
                        return;
                    else
                    {
                        TakeDamageMechanic(other);
                    }
                }
                
            }
        }
        #endregion

        #region Destroy Box Pandora
        if (other.transform.GetComponent<BoxController>()!=null)
        {
            other.transform.GetComponent<BoxController>().ExtractBoxPandora();
        }
        #endregion
    }
    private void TakeDamageMechanic(Collider other)
    {
        HealthController healthControllerTarget = other.transform.GetComponent<HealthController>();
        healthControllerTarget.TakeDamage(damageGet);
        Destroy(this.gameObject);
    }



}
