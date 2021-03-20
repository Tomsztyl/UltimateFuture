using Mirror;
using RotaryHeart.Lib.SerializableDictionary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "Rifle", menuName = "HandObject/Make New Weapon", order = 1)]
public class WeaponeObjectController : ScriptableObject
{

    //Set Prefab In The Player
    [Header("Prefab In The Player")]
    [Tooltip("Set Parent RifleHand")]
    [SerializeField] private GameObject rifleHand;
    [Tooltip("Set Child WepaoneHand")]
    public GameObject weaponeHand;

    //Properties Weapone
    [Tooltip("Set Properties Weapone")]
    [Header("This is a properties Weapone")]
    [SerializeField] private float damageWeapone;
    [Tooltip("This Variable set range Shooting Weapone")]
    [SerializeField] private float rangeWeapone;
    [Tooltip("This Variable set a Time to next shoot Weapone")]
    [SerializeField] private float timeToNextRateShoot = 0f;
    [Tooltip("Set a gun Kind")]
    [SerializeField] private GunKind gunKind=GunKind.None;
    [SerializeField] private AudioClip audioShootingEffect = null;

    [Header("Variable Controll Speed Player")]
    [Tooltip("This variables set speed Player if have object in hand")]
    [SerializeField] private float speedWalkingPlayer = 0f;
    [Tooltip("This variable set speed rotation Player if have object in hand")]
    [SerializeField] private float speedRotationPlayer = 0f;


    //Propetries Particle
    [Header("This is variables to particle gun shoot")]
    [SerializeField] private bool isParticle = false;
    [SerializeField] private GameObject impactParticle=null;


    #region Properties Return to Instantiate Weapone Start
    public GameObject ReturnWeaponeHand()
    {
        return weaponeHand;
    }
    public Vector3 ReturnPositionInstantiate(CharacterSelectPerks characterSelectPerks)
    {
        return GetTrandormInstantiateCharacter(characterSelectPerks).postitionLocal;
    }
    public Vector3 ReturnRotationInstantiate(CharacterSelectPerks characterSelectPerks)
    {
       return GetTrandormInstantiateCharacter(characterSelectPerks).rotationLocal;
    }
    public Vector3 ReturnScaleInstantiate(CharacterSelectPerks characterSelectPerks)
    {
       return GetTrandormInstantiateCharacter(characterSelectPerks).scaleLocal;
    }

    #endregion
    #region Properties Particle Impact
    public bool GetIsParticle()
    {
        return isParticle;
    }
    public GameObject GetImpactParticle()
    {
        return impactParticle;
    }
    #endregion
    #region Properties Properties Weapone
    public float GetDamageWeapone()
    {
        return damageWeapone;
    }
    public float GetRangeWeapone()
    {
        return rangeWeapone;
    }
    public float GetTimeToNextRateShoot()
    {
        return timeToNextRateShoot;
    }
    #endregion

    public float ReturnSpeedWalikingPlayer()
    {
        return speedWalkingPlayer;
    }
    public float ReturnSpeedRotationPlayer()
    {
        return speedRotationPlayer;
    }
    public AudioClip GetAudioShootingEffect()
    {
        return audioShootingEffect;
    }
    public GunKind GetGunKind()
    {
        return gunKind;
    }
    #region Tranform Instantriate Object In Hand
    [SerializeField]
    public TranformObjectIsInstatniate _tranfromInstantiateObjectHand;

    [System.Serializable]
    public class TranformObjectIsInstatniate : SerializableDictionaryBase<CharacterSelectPerks, TranformInstntaite> { }

    [System.Serializable]
    public class TranformInstntaite
    {
      public Vector3 postitionLocal;
      public Vector3 rotationLocal;
      public Vector3 scaleLocal;
    }

    public TranformInstntaite GetTrandormInstantiateCharacter(CharacterSelectPerks characterSelectPerks)
    {
        return _tranfromInstantiateObjectHand.FirstOrDefault(x => x.Key == characterSelectPerks).Value;
    }
    #endregion
}