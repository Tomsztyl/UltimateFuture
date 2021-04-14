using Mirror;
using Mirror.Examples.NetworkRoom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Cameras;

public enum CharacterSelectPerks
{
    Vanguard,
    SwatGuard,
}

public class PlayerMirrorController : NetworkBehaviour
{
    [Header("This is a Persks Character")]
    [Tooltip("Define Who Character Choose ")]
    [SerializeField] CharacterSelectPerks characterSelectPerks;

    [Header("This is Control Player Start")]
    public bool isPlayer = false;
    public PlayerController PlayerController;
    public CameraControllerPlayer CameraControllerPlayer;
    public GameObject Cameras = null;
    [Header("This is HUD Player")]
    public GameObject canvasHUD = null;
    public Canvas CanvasHUD = null;
    public UIManager UIManagerHUD = null;
    public GameObject EventSystem = null;
    public RepairControllerMouse repairControllerMouse=null;


    [Header("This is a Equipment Player")]
    public GameObject ToolBar = null;
    public List<GameObject> SlorControllerToolBar = new List<GameObject>();
    public GameObject EquipmentBox = null;
    public List<GameObject> SlorControllerEquipmentBox = new List<GameObject>();
    public GameObject InventoryManager = null;

    [Header("Select Object Hand Player ")]
    public GameObject rightHand;
    public GameObject leftHand;

    [Header("This is variable to trigger PingBar")]
    public GameObject pingBar = null;

    [Header("Variable Trigger Pointer ")]
    public GameObject pointer = null;
    [SyncVar]
    public Vector3 pointerMovePosition;
    public bool isPointer = false;

    [Header("This is a Display Character Text up Head")]
    [Tooltip("This is a Display Character Text up Head")]
    [SerializeField] private Transform displayTextUpCharacter = null;


    void OnValidate()
    {
        TriggerHudPlayer();
        TriggerEquipmentPlayer();
    }

    void Start()
    {
        TriggerPlayerHUDStart();
        if (isLocalPlayer) { SelectCharacterLayer(); FindAllSlotControllerServer(); CreateCameraToPlayer(); }
        isPlayer = isLocalPlayer;
        repairControllerMouse.enabled = isLocalPlayer;
        CameraControllerPlayer.enabled = isLocalPlayer;
        GetComponent<InteractObjectController>().enabled = isLocalPlayer;
    }
    void SelectCharacterLayer()
    {
        this.gameObject.layer = 8;
    }
    private void Update()
    {
        if (isLocalPlayer) 
        { 
            StartChecker();
        }
    }
    public void TriggerPlayerHUDStart()
    {
        PlayerController.enabled = isLocalPlayer;
        CanvasHUD.enabled = isLocalPlayer;
        UIManagerHUD.enabled = isLocalPlayer;
        EventSystem.SetActive(isLocalPlayer);
        pingBar.SetActive(isLocalPlayer);
    }
    #region Walidation Controls Player
    public void TriggerHudPlayer()
    {

        if (PlayerController == null)
            PlayerController = GetComponent<PlayerController>();

        if (CameraControllerPlayer == null)
            CameraControllerPlayer = GetComponent<CameraControllerPlayer>();

        if (CanvasHUD == null)
            CanvasHUD = GameObject.Find(this.name + "/Canvas").GetComponent<Canvas>();

        if (UIManagerHUD == null)
            UIManagerHUD = GameObject.Find(this.name + "/Canvas").GetComponent<UIManager>();

        if (EventSystem == null)
            EventSystem = GameObject.Find(this.name + "/Canvas/EventSystem").GetComponent<GameObject>();

        if (canvasHUD == null)
            canvasHUD = GameObject.Find(this.name + "/Canvas/EventSystem").GetComponent<GameObject>();

        if (pingBar == null)
            pingBar = GameObject.Find(this.name + "/Canvas/PingBar").GetComponent<GameObject>();

        if (pointer == null)
            pointer = GameObject.Find(this.name + "/Canvas/Pointer").GetComponent<GameObject>();
    }
    public void TriggerEquipmentPlayer()
    {
        if (ToolBar == null)
            ToolBar = GameObject.Find(this.name + "/Canvas/EventSystem").GetComponent<GameObject>();

        if (EquipmentBox == null)
            EquipmentBox = GameObject.Find(this.name + "/Canvas/EventSystem").GetComponent<GameObject>();

        if (InventoryManager == null)
            InventoryManager = GameObject.Find(this.name + "/Canvas/EventSystem").GetComponent<GameObject>();
    }
    #endregion

    #region Create Local Third Camera for Player
    public void CreateCameraToPlayer()
    {
        var cameraInstantiate = Instantiate(Cameras, transform.position, Quaternion.identity);

        FreeLookCam freeLookCam = cameraInstantiate.transform.Find("FreeLookCameraRig").GetComponent<FreeLookCam>();
        GetComponent<PlayerController>().SetCurrentCameraTranform(freeLookCam.GetComponent<Transform>());
        freeLookCam.SetTarget(this.gameObject.transform);


        Transform ThirdCameraObj = GameObject.FindWithTag("ThirdCamera").GetComponent<Transform>();
        GameObject objectgoot;
        objectgoot = ThirdCameraObj.gameObject;
        CameraControllerPlayer.SetThirdCamera(objectgoot);
    }
    #endregion

    #region Find All Slot From EQ
    //Check Trigger Key
    [Command]
    public void FindAllSlotControllerServer()
    {
        FindAllSlotController();
        FindAllSlotControllerServerClients();
    }
    public void FindAllSlotController()
    {
        foreach (Transform child in ToolBar.transform)
        {
            SlorControllerToolBar.Add(child.gameObject);
        }
        foreach (Transform child in EquipmentBox.transform)
        {
            SlorControllerEquipmentBox.Add(child.gameObject);
        }
    }
    [ClientRpc]
    private void FindAllSlotControllerServerClients()
    {
        FindAllSlotController();
    }
    #endregion

    #region Trigger Input Key Slot
    public void StartChecker()
    {
        CheckTheTriggerKey();
    }
    public void CheckTheTriggerKey()
    {
        foreach (GameObject slotControllerObj in SlorControllerToolBar)
        {
            SlotController slotControllerCheck = slotControllerObj.GetComponent<SlotController>();
            if (slotControllerCheck.ReturnKeyCodeObject() == KeyCode.None)
            {
                return;
            }
            else
            {
                if (Input.GetKeyDown(slotControllerCheck.ReturnKeyCodeObject()))
                {
                    foreach (var scriptableObject in ((NetworkRoomManagerExt)NetworkManager.singleton).scriptableObjectToMirror.Select(((value, index) => new { value, index })))
                    {
                        if (slotControllerCheck.ReturnPrefab() == null) { DestoryAllObjectInHandServer(); PlayerController.SetBasicSpeed(); PlayerController.SetDefaultSpeedToVariableCalculate(); return; }
                        else if (scriptableObject.value.name == slotControllerCheck.ReturnPrefab().name)
                        {
                            DestoryAllObjectInHandServer();
                            SendSlotControllerToObjectServer(scriptableObject.index);
                            continue;
                        }
                    }
                }
            }
        }
    }
#endregion

    #region Select Object From Scriptable Object And Send In Hand And Send Properties object/weapone
    [Command]
    public void SendSlotControllerToObjectServer(int indexSpanObject)
    {
        SendSlotControllerToObject(indexSpanObject);
        SendSlotControllerToObjectClients(indexSpanObject);
    }
    public void SendSlotControllerToObject(int indexSpanObject)
    {
        var objectInstantiate = Instantiate(((NetworkRoomManagerExt)NetworkManager.singleton).objectHand, rightHand.transform);
        if (NetworkServer.active) { NetworkServer.Spawn(objectInstantiate); }
        FightController fightController = objectInstantiate.transform.root.GetComponent<FightController>();



        if (((NetworkRoomManagerExt)NetworkManager.singleton).scriptableObjectToMirror[indexSpanObject] is WeaponeObjectController)
        {
            WeaponeObjectController weaponeObjectController = (WeaponeObjectController)((NetworkRoomManagerExt)NetworkManager.singleton).scriptableObjectToMirror[indexSpanObject];
            var objectWeaponeInstantiate = Instantiate(weaponeObjectController.ReturnWeaponeHand(), objectInstantiate.transform);
            if (NetworkServer.active) { NetworkServer.Spawn(objectWeaponeInstantiate); }

            //Set Speed Player with gun
            // PlayerController.RestetCalculateVariable();
            PlayerController.SetSpeedGunAimProperties(weaponeObjectController.ReturnSpeedWalikingPlayer(), weaponeObjectController.ReturnSpeedRotationPlayer());
            PlayerController.SetDefaultSpeedToVariableCalculate();

            //Set Properties Weapone
            objectWeaponeInstantiate.transform.localPosition = weaponeObjectController.ReturnPositionInstantiate(characterSelectPerks);
            objectWeaponeInstantiate.transform.localEulerAngles = weaponeObjectController.ReturnRotationInstantiate(characterSelectPerks);
            objectWeaponeInstantiate.transform.localScale = weaponeObjectController.ReturnScaleInstantiate(characterSelectPerks);

            //Set ScriptableObject In Script Fight
            fightController.SetHandObjectController(null);
            fightController.SetWeaponeObjectController(weaponeObjectController);

            //Set ObjectHand and ObjectInHandChild
            fightController.SetObjcetHandChild(objectWeaponeInstantiate);
            fightController.SetObjectHand(objectInstantiate);
        }
        else if (((NetworkRoomManagerExt)NetworkManager.singleton).scriptableObjectToMirror[indexSpanObject] is HandObjectController)
        {
            HandObjectController handObjectController = (HandObjectController)((NetworkRoomManagerExt)NetworkManager.singleton).scriptableObjectToMirror[indexSpanObject];
            var objectHandInstantiate = Instantiate(handObjectController.ReturnObjectInHandPref(), objectInstantiate.transform);
            if (NetworkServer.active) { NetworkServer.Spawn(objectHandInstantiate); }


            //Set Speed Player With object in hand          
            PlayerController.SetSpeedDefProperties(handObjectController.ReturnSpeedWalikingPlayer(), handObjectController.ReturnSpeedRotationPlayer());
            PlayerController.SetDefaultSpeedToVariableCalculate();

            //Set Properties Object
            objectHandInstantiate.transform.localPosition = handObjectController.ReturnPositionInstantiate(characterSelectPerks);
            objectHandInstantiate.transform.localEulerAngles = handObjectController.ReturnRotationInstantiate(characterSelectPerks);
            objectHandInstantiate.transform.localScale = handObjectController.ReturnScaleInstantiate(characterSelectPerks);

            //Set ScriptableObject In Script Fight
            fightController.SetWeaponeObjectController(null);
            fightController.SetHandObjectController(handObjectController);

            //Set ObjectHand and ObjectInHandChild
            fightController.SetObjcetHandChild(objectHandInstantiate);
            fightController.SetObjectHand(objectInstantiate);
        }
    }
    [ClientRpc]
    public void SendSlotControllerToObjectClients(int indexSpanObject)
    {
        SendSlotControllerToObject(indexSpanObject);
    }
    #endregion

    #region Check If Object In Hand is Destory Before Instantiate next Object In Hand
    [Command]
    public void DestoryAllObjectInHandServer()
    {
        DestoryAllObjectInHand();
        DestoryAllObjectInHandClients();
    }
    public void DestoryAllObjectInHand()
    {
        foreach (Transform objectInHand in rightHand.transform)
        {
            if (objectInHand.name == "ObjectHand(Clone)")
            {
                Destroy(objectInHand.gameObject);
            }
        }
    }
    [ClientRpc]
    public void DestoryAllObjectInHandClients()
    {
        DestoryAllObjectInHand();
    }
    #endregion

    #region Send Tranfrom Pointer
    public void CheckIsClientSendToServerComand(Vector3 postionPointer)
    {
        isPointer = true;
        UpdatePointerPostionServer(postionPointer);
    }
    [Command]
    public void UpdatePointerPostionServer(Vector3 postionPointer)
    {
        UpdatePointerPostion(postionPointer);
        UpdatePointerPostionClients(postionPointer);
    }
    public void UpdatePointerPostion(Vector3 postionPointer)
    {
        if (isPointer == false)
        {
            pointerMovePosition = postionPointer;
            pointer.transform.position = pointerMovePosition;
        }
        else
        {
            isPointer = false;
        }
    }
    [ClientRpc]
    public void UpdatePointerPostionClients(Vector3 postionPointer)
    {
        UpdatePointerPostion(postionPointer);
    }
    #endregion
    #region Mechansm Drop Object
    public void DropObjectServer(string nameDropObjectScriptable, Vector3 instantiateObjectDrop, float valueDrop)
    {
        DropObject(nameDropObjectScriptable, instantiateObjectDrop, valueDrop);
        DropObjectClient(nameDropObjectScriptable, instantiateObjectDrop, valueDrop);
    }
    public void DropObject(string nameDropObjectScriptable,Vector3 instantiateObjectDrop, float valueDrop)
    {
        //var InstantiateDropObject=Instantiate()
        foreach (var scriptableObject in ((NetworkRoomManagerExt)NetworkManager.singleton).scriptableObjectToMirror.Select(((value, index) => new { value, index })))
        {          
            if (scriptableObject.value.name == nameDropObjectScriptable)
            {
                if (((NetworkRoomManagerExt)NetworkManager.singleton).scriptableObjectToMirror[scriptableObject.index] is WeaponeObjectController)
                {
                    WeaponeObjectController weaponeObjectController = (WeaponeObjectController)((NetworkRoomManagerExt)NetworkManager.singleton).scriptableObjectToMirror[scriptableObject.index];
                    var InstantiateObject = Instantiate(weaponeObjectController.RetrunObjectWeaponeDropPreafabGround(), instantiateObjectDrop,Quaternion.identity);
                    if (NetworkServer.active) { NetworkServer.Spawn(InstantiateObject); }

                    PickUpMaterial pickUpMaterial = InstantiateObject.GetComponent<PickUpMaterial>();

                    if (pickUpMaterial!=null)
                    {
                        pickUpMaterial.SetCountObject(valueDrop);
                    }
                    continue;
                }
                else if(((NetworkRoomManagerExt)NetworkManager.singleton).scriptableObjectToMirror[scriptableObject.index] is HandObjectController)
                {
                    HandObjectController handObjectController = (HandObjectController)((NetworkRoomManagerExt)NetworkManager.singleton).scriptableObjectToMirror[scriptableObject.index];
                    var InstantiateObjectWeapone = Instantiate(handObjectController.RetrunObjectDropPreafabGround(), instantiateObjectDrop, Quaternion.identity);
                    if (NetworkServer.active) { NetworkServer.Spawn(InstantiateObjectWeapone); }

                    PickUpMaterial pickUpMaterial = InstantiateObjectWeapone.GetComponent<PickUpMaterial>();

                    if (pickUpMaterial != null)
                    {
                        pickUpMaterial.SetCountObject(valueDrop);
                    }
                    continue;
                }              
            }
        }
    }
    public void DropObjectClient(string nameDropObjectScriptable, Vector3 instantiateObjectDrop, float valueDrop)
    {
        DropObject(nameDropObjectScriptable, instantiateObjectDrop, valueDrop);
    }
    #endregion


}