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
    //[SerializeField] private KindCharacter kindCharacter;
    public TextMeshPro textHealt;

    [SerializeField] private CharacterKind characterKind = CharacterKind.None;
    private bool isDying = false;
    [SerializeField] private bool death = false;

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
        }
        else if (characterKind==CharacterKind.Enemy)
        {

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
