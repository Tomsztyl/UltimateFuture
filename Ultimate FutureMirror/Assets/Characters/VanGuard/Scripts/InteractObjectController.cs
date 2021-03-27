using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObjectController : NetworkBehaviour
{
    [Header("This is a button who player interact with object")]
    [Tooltip("This button trigger with object to interact [Default set Key Code {F}")]
    [SerializeField] private KeyCode _interactKey = KeyCode.F;
    [SerializeField] private float interactionRange = 10f;
    [SerializeField] private LayerMask layerMaskIngoreRay;
    private PlayerMirrorController _playerMirrorController = null;
    private UIManager _uiManagerPlayer = null;

    private BoxController boxControllerInteract = null;


    private void Start()
    {
        if (_playerMirrorController == null)
        {
            _playerMirrorController = GetComponent<PlayerMirrorController>();
            if (_uiManagerPlayer == null && _playerMirrorController != null)
                _uiManagerPlayer = _playerMirrorController.canvasHUD.GetComponent<UIManager>();
        }
    }
    private void Update()
    {
        //check is press button key
        if (isLocalPlayer)
            RayCastInteraction();
    }

    private void RayCastInteraction()
    {
        RaycastHit hit;

        if (Physics.Raycast(GetComponent<CameraControllerPlayer>().GetChooseCamera().transform.position, GetComponent<CameraControllerPlayer>().GetChooseCamera().transform.forward, out hit, interactionRange, layerMaskIngoreRay))
        {
            //Send To Server to Clinets Point Hit
            InteractionHitBoxPandora(hit);
        }
    }
    private void InteractionHitBoxPandora(RaycastHit hit)
    {
        BoxController boxController = hit.transform.GetComponent<BoxController>();

        if (boxController != null)
        {
            StartInteractObjectBoxPandora();
            CheckPressInteractButton(boxController);
        }
        else
        {
            ExitInteractObjectBoxPandora();
        }
    }
    private void IdNetworkkBoxPandoraInServer(BoxController boxController)
    {
        if (boxController!=null)ExtractBoxPandoraServer(boxController.GetComponent<NetworkIdentity>().netId);
    }
    [Command]
    private void ExtractBoxPandoraServer(uint netIdBoxPandora)
    {
        if (netIdBoxPandora!=0)
        {
            NetworkIdentity.spawned[netIdBoxPandora].GetComponent<BoxController>().ExtractBoxPandora();
        }
    }
    #region Press Button Interact
    private void CheckPressInteractButton(BoxController boxController)
    {
        if (Input.GetKeyDown(_interactKey))
        {
            IdNetworkkBoxPandoraInServer(boxController);
            //boxController.ExtractBoxPandora();
        }
    }
    #endregion
    #region Interact with BoxPandora
    private void StartInteractObjectBoxPandora()
    {
        //send Message Print to player
        SendTriggerToUIPlayer("- Press " + _interactKey + " to break -", true);
    }
    private void ExitInteractObjectBoxPandora()
    {
        //send Message Print to player
        SendTriggerToUIPlayer(string.Empty, false);
    }
    private void SendTriggerToUIPlayer(string textToMessagePlayer, bool isColider)
    {
        if (_uiManagerPlayer != null)
        {
            _uiManagerPlayer.ManagerMessagePanel(textToMessagePlayer, isColider);
        }
    }
    #endregion
}