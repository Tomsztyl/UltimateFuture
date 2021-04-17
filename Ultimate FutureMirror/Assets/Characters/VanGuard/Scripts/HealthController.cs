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

    [Header("Mechanism Death Character")]
    [SerializeField] private CharacterKind characterKind = CharacterKind.None;
    [SerializeField] private bool death = false;
    [SerializeField] private LayerMask layerMaskDeath;
    [SerializeField] private InventoryManager inventoryManager = null;

    [Header("Bar Health Character")]
    [SerializeField] private GameObject barHealthCharacter = null;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI = null;
    [SerializeField] private float fillTarget = .5f;
    [SerializeField] private float delta = 0f;
    [SerializeField] private float dampening = 5f;
    private Material barMaterial = null;
    private UIManager uIManager = new UIManager();

    private void Awake()
    {
        healtdef = healt;
    }
    private void Start()
    {
        FindBarHealth();
    }

    private void Update()
    {
        if (!death)
        UpdateBarHealth();

        if (healt <= 0 && death==false)
        {
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
    #region Mechanism Health Bar Over Character
    private void FindBarHealth()
    {
        UIManager uIManager = new UIManager();
        barMaterial = uIManager.FindObjectMateria(barHealthCharacter);
    }
    private void UpdateBarHealth()
    {
        if (barMaterial!=null)
        {
            ControllBar(healt, healtdef, barMaterial, textMeshProUGUI);
        }
    }
    private void ControllBar(float value, float defValue, Material materialBar, TextMeshProUGUI textProBar)
    {

        delta -= fillTarget - uIManager.PercentToValue(1, uIManager.ValueToPercent(value, defValue));
        fillTarget = uIManager.PercentToValue(1, uIManager.ValueToPercent(value, defValue));


        if (textProBar != null) { textProBar.text = uIManager.ValueToPercent(value, defValue) + "%"; }

        delta = Mathf.Lerp(delta, 0, Time.deltaTime * dampening);


        materialBar.SetFloat("_Delta", delta);
        materialBar.SetFloat("_Fill", fillTarget);
    }
    #endregion

    #region Controller Dying Character
    private void CheckCharacterKind()
    {
        if (characterKind == CharacterKind.None)
            return;
        else if (characterKind==CharacterKind.Player)
        {
            DeathCharacterObject();
        }
        else if (characterKind==CharacterKind.Enemy)
        {

        }
    }
    private void DeathCharacterObject()
    {
        StartCoroutine(PlayAnimationDeathOnlyOnce("isDying"));
        death = true;
        UpdateBarHealth();
        GetComponent<BoneAimingController>().enabled = false;
        GetComponent<PlayerController>().enabled = false;
        GetComponent<PlayerMirrorController>().enabled = false;
        GetComponent<FightController>().enabled = false;
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
