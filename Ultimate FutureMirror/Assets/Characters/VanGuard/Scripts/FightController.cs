using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightController : NetworkBehaviour
{
    [Header("Variables who Define Shooting")]
    [Tooltip("Key set Shoot Gun")]
    [SerializeField] private KeyCode shootGun = KeyCode.Mouse0;
    [SerializeField] private bool isAiming = false;
    [SerializeField] private bool isShooting = false;   
    private float nextTimeToFire = 0f;


    [Header("This is Clone Main ObjectHand")]
    [SerializeField] private GameObject objectHand=null;
    [Header("This is a child ObjectHand [OBJECT/WEAPONE]")]
    [SerializeField] private GameObject objectHandChild=null;


    [Header("This is a ScriptableObject to weapone in hand")]
    [Tooltip("This variable set when player get weapone in hand")]
    [SerializeField] private WeaponeObjectController weaponeObjectController=null;
    [Tooltip("Tihs variable set when player get object in hand")]
    [SerializeField] private HandObjectController handObjectController=null;

    public LayerMask PlayerLayerMask;


    private void LateUpdate()
    {
       if (isLocalPlayer)StartShooting();
    }
    #region Method Change Trigger Shoot
    public void StartAiming()
    {
        isAiming = true;
    }
    public void StopAiming()
    {
        isAiming = false;
        isShooting = false;
    }
    #endregion
    #region Mechanic Start Shooting
    private void StartShooting()
    {
        if (weaponeObjectController == null) return;
        if (isAiming)
        {
            if (Input.GetKeyDown(shootGun))
            {
                //Start Shooting
                isShooting = true;              
            }
            if (Input.GetKeyUp(shootGun))
            {
                //Stop Shooting
                isShooting = false;
            }
        }

        if (isShooting && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / weaponeObjectController.GetTimeToNextRateShoot();
            ShootController();
        }
    }
    #endregion
    #region Controller Scriptable Object And Object In Hand
    public void SetWeaponeObjectController(WeaponeObjectController weaponeObjectController)
    {
        this.weaponeObjectController = weaponeObjectController;
    }
    public void SetHandObjectController(HandObjectController handObjectController)
    {
        this.handObjectController = handObjectController;
    }
    public void SetObjectHand(GameObject objectHand)
    {
        this.objectHand = objectHand;
    }
    public void SetObjcetHandChild(GameObject objectHandChild)
    {
        this.objectHandChild = objectHandChild;
    }
    #endregion
    #region Mechanic Shooting RayCast
    private void ShootController()
    {
        ShootControllerToClient();
    }
    private void ShootControllerToClient()
    {
        if (weaponeObjectController == null) { return; }
        RaycastHit hit;
        //Start Muzzzle Effect
        MuzzleEffectTriggerServer();
        ShootingSoundServer();

        if (Physics.Raycast(GetComponent<CameraControllerPlayer>().GetChooseCamera().transform.position, GetComponent<CameraControllerPlayer>().GetChooseCamera().transform.forward, out hit, weaponeObjectController.GetRangeWeapone(), PlayerLayerMask))
        {
            //Send To Server to Clinets Point Hit
            InstatniateHitPointShootServer(hit.point, hit.normal) ;

            BoxController boxController = hit.transform.GetComponent<BoxController>();
            if (boxController != null)
            {
                boxController.ExtractBox();
            }

        }
    }
    #region Effect Particle Shoot

    //Muzzle Effect Trigger Server to Clinets
    [Command]
    private void MuzzleEffectTriggerServer()
    {
        MuzzleEffectTrigger();
        MuzzleEffectTriggerClients();
    }
    private void MuzzleEffectTrigger()
    {
        ParticleSystem muzzleEfectWeapone = objectHandChild.transform.Find("MuzzleWeapone").GetComponent<ParticleSystem>();
        if (muzzleEfectWeapone != null)
        {
            muzzleEfectWeapone.Play();
        }
    }
    [ClientRpc]
    private void MuzzleEffectTriggerClients()
    {
        MuzzleEffectTrigger();
    }
    //***

    //Start Shooting Effect Server to Clinets
    [Command]
    private void InstatniateHitPointShootServer(Vector3 hitPoint,Vector3 hitNormal)
    {
        InstatniatePoinShoot(hitPoint, hitNormal);
        //InstatniateHitPointShootClients(hitPoint, hitNormal);
    }
    private void InstatniatePoinShoot(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (weaponeObjectController.GetImpactParticle() != null)
        {
            if (NetworkServer.active) 
            {
                //Make in server
                var objectInstantiateParticle = Instantiate(weaponeObjectController.GetImpactParticle(), hitPoint, Quaternion.LookRotation(hitNormal));
                NetworkServer.Spawn(objectInstantiateParticle);

                #region Set Poperties Shoot Impact
                ShootImpactController shootImpactController = objectInstantiateParticle.GetComponent<ShootImpactController>();

                if (shootImpactController != null)
                {
                    shootImpactController.ShootImpactControllerProperties(this.gameObject.GetComponent<NetworkIdentity>().netId, weaponeObjectController.GetGunKind(), weaponeObjectController.GetDamageWeapone());
                    Debug.Log(this.gameObject);
                }
                #endregion
                Destroy(objectInstantiateParticle.gameObject, 2f);
            }

        }
    }
    [ClientRpc]
    private void InstatniateHitPointShootClients(Vector3 hitPoint, Vector3 hitNormal)
    {
        //InstatniatePoinShoot(hitPoint, hitNormal); 
    }

    //****

    //Trigger Destroy Box To Shooting Server to Clinets 
    [Command]
    private void ShootingDestroyBoxServer(GameObject hitParticle)
    {
        Debug.Log(hitParticle);
       // ShootingDestoyBoxClients(hitParticle);
    }
    [ClientRpc]
    private void ShootingDestoyBoxClients(GameObject hitParticle)
    {
        //BoxController boxController = hitParticle.transform.GetComponent<BoxController>();
        //if (boxController != null)
        //{
        //    boxController.ExtractBox();
        //}
    }
    //*****


    #endregion
    #region Effect Audio Shoot
    [Command]
    private void ShootingSoundServer()
    {
        ShootingSound();
        ShootingSoundClients();
    }
    private void ShootingSound()
    {
        if (weaponeObjectController != null)
        {
            ShootingSoundRange();
            GetComponent<AudioSource>().clip = weaponeObjectController.GetAudioShootingEffect();
            GetComponent<AudioSource>().Play();
        }
    }
    [ClientRpc]
    private void ShootingSoundClients()
    {
        ShootingSound();
    }
    #region Porpoeries Sound Shooting
    private void ShootingSoundRange()
    {
        GetComponent<AudioSource>().maxDistance=weaponeObjectController.GetRangeWeapone();
    }
    #endregion
    #endregion
    #endregion
}
