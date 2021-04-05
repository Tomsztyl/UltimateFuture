using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class SlotController : MonoBehaviour
{
    [Header("This Variables set object in EQ")]
    [SerializeField] private ScriptableObject prefabObject;
    [SerializeField] private Sprite spriteObject;
    [SerializeField] private float countObject;
    [SerializeField] private KeyCode keyCodeObject;
    private PlayerController playerController;
    private PlayerMirrorController playerMirrorController;

    [Header("This is Vairables set List Action Slot")]
    [SerializeField] private GameObject listActionSlot = null;

    private void Start()
    {
        playerMirrorController = this.gameObject.transform.root.GetComponent<PlayerMirrorController>();
    }
    private void Update()
    {
        SetObjectKeyTrigger();
    }
    #region Set Object In EQ Slot
    private void SetObjectKeyTrigger()
    {
        if (Input.GetKeyDown(keyCodeObject) && playerMirrorController.isPlayer)
        {
            if (prefabObject == null)
            {
                playerController = this.gameObject.transform.root.GetComponent<PlayerController>();
                playerController.GunKind = GunKind.None;
            }
            else if (prefabObject is WeaponeObjectController)
            {
                WeaponeObjectController weaponeObjectController = (WeaponeObjectController)prefabObject;
                playerController = this.gameObject.transform.root.GetComponent<PlayerController>();
                playerController.GunKind = GunKind.Rifle;
            }
            else if (prefabObject is HandObjectController)
            {
                HandObjectController handObjectController = (HandObjectController)prefabObject;
                playerController = this.gameObject.transform.root.GetComponent<PlayerController>();
                playerController.GunKind = GunKind.Object;
            }

            Button buttonSlot = gameObject.transform.Find("Border").GetComponent<Button>();
            buttonSlot.Select();
            buttonSlot.onClick.Invoke();
        }
    }
    public void SetPrefab(ScriptableObject gameObjectPrefab)
    {
        prefabObject = gameObjectPrefab;
    }
    public void SetSprite(Sprite sprite)
    {
        spriteObject = sprite;
        Image spriteimage = gameObject.transform.Find("Border/ItemImage").GetComponent<Image>();
        spriteimage.enabled = true;
        spriteimage.sprite = spriteObject;
    }
    public void SetCount(float count)
    {
        countObject = count;
        Text textCount = gameObject.transform.Find("Border/ItemImage/Count").GetComponent<Text>();
        textCount.text = "" + countObject;
    }
    #endregion
    #region Drop Object From Slot
    private void TrrigerDrop()
    {
        if (!IsEmptyCheckSlot())
        {
            //Not empty slot

        }
        else
        {
            //Empty slot
        }
    }
    private bool IsEmptyCheckSlot()
    {
        if (prefabObject == null && spriteObject == null && countObject <= 0)
        {
            return true;
        }
        else return false;
    }
    #endregion
    #region Validation Variables
    public void SetKeyCode(KeyCode keyCode)
    {
        keyCodeObject = keyCode;
    }
    public ScriptableObject ReturnPrefab()
    {
        return prefabObject;
    }
    public Sprite ReturnSprite()
    {
        return spriteObject;
    }
    public float ReturnCountObject()
    {
        return countObject;
    }
    public KeyCode ReturnKeyCodeObject()
    {
        return keyCodeObject;
    }
    #endregion
}
