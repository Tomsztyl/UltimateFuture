using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum CharacterKind
{
    None,
    Player,
    Enemy,
}

public class HealthController : NetworkBehaviour
{
    [Header("Set Current Healt")]
    [Tooltip("Default Healt is 100")]
    [SyncVar]
    public float healt = 100;
    [SyncVar]
    public float healtdef;
    public TextMeshPro textHealt;

    [Header("Mechanism Death Character")]
    [SerializeField] private CharacterKind characterKind = CharacterKind.None;
    private bool isDying = false;
    [SerializeField] private bool death = false;
    [SerializeField] private LayerMask layerMaskDeath;
    [SerializeField] private InventoryManager inventoryManager = null;

    [Header("Bar Health Character")]
    [SerializeField] private GameObject barHealthCharacter = null;

    private void Awake()
    {
        healtdef = healt;
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            DisplayTextServer();
        }
        if (healt <= 0 && death==false)
        {
            isDying = true;
            CheckCharacterKind();
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (healt <= 0) 
            return;
        else
        {
            healt = healt - damage;
        }

    }

    [Command]
    public void DisplayTextServer()
    {
        DisplayText();
        DisplayTextClients();
    }
    public void DisplayText()
    {
        textHealt.text = "" + healt;
    }
    [ClientRpc]
    public void DisplayTextClients()
    {
        DisplayText();
    }
    #region Display Bar Health
    #region Mechanism Calculate Percent
    private int ValueToPercent(float value, float defValue)
    {
        return (int)((int)value * 100 / defValue);
    }
    private float PercentToValue(float value, int percent)
    {
        return (float)((float)value * percent * 0.01);
    }
    #endregion
    #endregion
    #region Controller Dying Character
    private void CheckCharacterKind()
    {
        if (characterKind == CharacterKind.None)
            return;
        else if (characterKind==CharacterKind.Player)
        {
            StartCoroutine(PlayAnimationDeathOnlyOnce("isDying"));
            death = true;
            GetComponent<BoneAimingController>().enabled = false;
            GetComponent<PlayerController>().enabled = false;
            GetComponent<PlayerMirrorController>().enabled = false;
            GetComponent<FightController>().enabled = false;
            DeathCharacterObject();
        }
        else if (characterKind==CharacterKind.Enemy)
        {

        }
    }
    private void DeathCharacterObject()
    {
        GetComponent<CapsuleCollider>().isTrigger = true;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        this.gameObject.layer = layerMaskDeath;
        if (inventoryManager!=null)
        {
            inventoryManager.DropAllObjectFromEq();
        }
    }
    private IEnumerator PlayAnimationDeathOnlyOnce(string paramName)
    {
        GetComponent<Animator>().SetBool(paramName, true);
        yield return null;
        GetComponent<Animator>().SetBool(paramName, false);
    }
    #endregion
}
