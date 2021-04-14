using System.Collections;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;
using System.Linq;

[CreateAssetMenu(fileName = "HandObject", menuName = "HandObject/Make New Object", order = 0)]
public class HandObjectController : ScriptableObject
{
    //Set Prefab In The Player
    [Header("Prefab In The Player")]
    [Tooltip("Set Parent ObjectHand")]
    [SerializeField] private GameObject objectHand;
    [Tooltip("Set Child objectInHandPref")]
    [SerializeField] private GameObject objectInHandPref;

    //Set Prefab Drop Properties
    [Header("Prafab In The Player Drop")]
    [Tooltip("This object is a object to drop ground from player")]
    [SerializeField] private GameObject objectDropPrefabGround = null;


    //Properties Object
    [Tooltip("Set Properties Object")]
    [Header("This is a properties Object")]
    [SerializeField] private float healPoint;
    [Header("Variable Controll Speed Player")]
    [Tooltip("This variables set speed Player if have object in hand")]
    [SerializeField] private float speedWalkingPlayer=0f;
    [Tooltip("This variable set speed rotation Player if have object in hand")]
    [SerializeField] private float speedRotationPlayer=0f;


    #region Properties Return to Instantiate Weapone Start
    public GameObject ReturnObjectInHandPref()
    {
        return objectInHandPref;
    }
    public GameObject RetrunObjectDropPreafabGround()
    {
        return objectDropPrefabGround;
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

    #region Poperties Speed Player

    public float ReturnSpeedWalikingPlayer()
    {
        return speedWalkingPlayer;
    }
    public float ReturnSpeedRotationPlayer()
    {
        return speedRotationPlayer;
    }
    #endregion
}
