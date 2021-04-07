using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
    private bool isPonterOnSlot = false;

    private void Start()
    {
        playerMirrorController = this.gameObject.transform.root.GetComponent<PlayerMirrorController>();
    }
    private void Update()
    {
        SetObjectKeyTrigger();
        TriggerOnPointer();
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
    #region Check Trigger On Pointer
    public void OnPointerEnter(PointerEventData eventData)
    {
        isPonterOnSlot = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isPonterOnSlot = false;
    }
    private void TriggerOnPointer()
    {
        if (isPonterOnSlot)
        {
            TriggerDrop();
        }
    }
    #endregion
    #region Drop Object From Slot
    private void TriggerDrop()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            listActionSlot.SetActive(isPonterOnSlot);

            //Not empty slot
            DropController dropController = listActionSlot.GetComponentInChildren<DropController>();

            if (dropController != null)
            {
                dropController.SetMaxValueSlider(countObject);
                dropController.SetImageDrop(spriteObject);
            }
        }
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
