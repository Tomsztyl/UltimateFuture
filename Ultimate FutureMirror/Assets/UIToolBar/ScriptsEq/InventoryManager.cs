using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : NetworkBehaviour
{
    [SerializeField] private GameObject toolBar;
    [SerializeField] private List<GameObject> slotToolBar = new List<GameObject>();

    [SerializeField] private GameObject equipmentBox;
    [SerializeField] private List<GameObject> slotEquipmentBox = new List<GameObject>();

    [SerializeField] private Vector3 dropObjectPostion = new Vector3(0f, 2f, 4f);
    private Vector3 tranformObjectCalcuated;



    private void Start()
    {
        FindAllSlot();
    }


    #region Find All Slot EQ
    private void FindAllSlot()
    {
        foreach (Transform child in toolBar.transform)
        {
            slotToolBar.Add(child.gameObject);
        }
        foreach (Transform child in equipmentBox.transform)
        {
            slotEquipmentBox.Add(child.gameObject);
        }
    }
    #endregion
    #region Mechanism Set Object In Eq
    private GameObject CheckObjectItExist(ScriptableObject gameObject, Sprite sprite, List<GameObject> listGameObject)
    {
        foreach (GameObject slotControllerObj in listGameObject)
        {
            SlotController slotController = slotControllerObj.GetComponent<SlotController>();
            if (slotController.ReturnPrefab() == gameObject && slotController.ReturnSprite() == sprite)
                return slotControllerObj;
        }
        return null;
    }
    private GameObject FindSortEquipObject(List<GameObject> listGameObject)
    {
        foreach (GameObject slotControllerObj in listGameObject)
        {
            SlotController slotController = slotControllerObj.GetComponent<SlotController>();
            if (slotController.ReturnPrefab() == null && slotController.ReturnSprite() == null)
                return slotControllerObj;
        }
        return null;
    }
    public bool IsExistObject(ScriptableObject scriptableObject, Sprite spriteObj)
    {
        //Check First Sloot ToolBar
        if (CheckObjectItExist(scriptableObject, spriteObj, slotToolBar) != null) { return true; }
        //Check Second Sloot Tool Box
        if (CheckObjectItExist(scriptableObject, spriteObj, slotEquipmentBox) != null) { return true; }
        return false;
    }
    public bool IsHavePlaceInEq()
    {
        //Check First Sloot ToolBar
        if (FindSortEquipObject(slotToolBar) != null) { return true; }
        //Check Second Sloot Tool Box
        if (FindSortEquipObject(slotEquipmentBox) != null) { return true; }
        return false;
    }
    public void SetObjectIsExist(ScriptableObject scriptableObject, Sprite spriteObj, float countObj)
    {
        if (CheckObjectItExist(scriptableObject, spriteObj, slotToolBar) != null)
        {
            SlotController slotController = CheckObjectItExist(scriptableObject, spriteObj, slotToolBar).GetComponent<SlotController>();
            slotController.SetCount(slotController.ReturnCountObject() + countObj);
            //SendToServerObjectToolBar(slotController, scriptableObject, spriteObj, countObj);
        }
        else if (CheckObjectItExist(scriptableObject, spriteObj, slotEquipmentBox) != null)
        {
            SlotController slotController = CheckObjectItExist(scriptableObject, spriteObj, slotEquipmentBox).GetComponent<SlotController>();
            slotController.SetCount(slotController.ReturnCountObject() + countObj);
            //SendToServerObjectToolBar(slotController, scriptableObject, spriteObj, countObj);
        }
    }
    public void SetObjectIsHavePlace(ScriptableObject scriptableObject, Sprite spriteObj, float countObj)
    {
        if (FindSortEquipObject(slotToolBar) != null)
        {
            SlotController slotController = FindSortEquipObject(slotToolBar).GetComponent<SlotController>();
            slotController.SetPrefab(scriptableObject);
            slotController.SetSprite(spriteObj,true);
            slotController.SetCount(countObj);
            //SendToServerObjectToolBar(slotController, scriptableObject, spriteObj, countObj);
        }
        else if (FindSortEquipObject(slotEquipmentBox) != null)
        {
            SlotController slotController = FindSortEquipObject(slotEquipmentBox).GetComponent<SlotController>();
            slotController.SetPrefab(scriptableObject);
            slotController.SetSprite(spriteObj,true);
            slotController.SetCount(countObj);
            //SendToServerObjectToolBar(slotController, scriptableObject,spriteObj,countObj);
        }
    }
    #endregion
    #region Drop All Object From Eq
    public void DropAllObjectFromEq()
    {
        foreach(GameObject slotControllerObj in slotToolBar)
        {
            //Drop Object From Slots ToolBar
            SlotController slotController = slotControllerObj.GetComponent<SlotController>();
            DropObjectFromSlotMirror(slotController);

        }

        foreach (GameObject slotControllerObj in slotEquipmentBox)
        {
            //Drop Object From Slots Equipment Box
            SlotController slotController = slotControllerObj.GetComponent<SlotController>();
            DropObjectFromSlotMirror(slotController);
                         
        }
    }
    private void DropObjectFromSlotMirror(SlotController slotController)
    {
        if (slotController != null)
        {
            if (slotController.ReturnPrefab() != null || slotController.ReturnSprite() != null || slotController.ReturnCountObject() > 0)
            {
                PlayerMirrorController playerMirrorController = this.gameObject.transform.root.GetComponent<PlayerMirrorController>();
                if (playerMirrorController != null)
                {
                    playerMirrorController.DropObjectServer(slotController.ReturnPrefab().name, CalculateInstantiateObjectDrop(), slotController.ReturnCountObject());
                    playerMirrorController.DestoryAllObjectInHandServer();
                }
                    

                slotController.SetCount(0);
                slotController.SetPrefab(null);
                slotController.SetSprite(null, false);
            }
        }
    }
    private Vector3 CalculateInstantiateObjectDrop()
    {
        tranformObjectCalcuated = new Vector3(transform.root.position.x, transform.root.position.y + dropObjectPostion.y, transform.root.position.z) + (transform.root.forward * dropObjectPostion.z);
        return tranformObjectCalcuated;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        CalculateInstantiateObjectDrop();
        Gizmos.DrawWireCube(tranformObjectCalcuated, new Vector3(2f, 2f, 2f));
    }
    #endregion
}
