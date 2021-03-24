using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneAimingController : NetworkBehaviour
{
    [Header("This is a bone who move to a pointer")]
    [SerializeField] private Transform spineBone = null;
    [SerializeField] private Transform spine1Bone = null;
    [SerializeField] private Transform spine2Bone = null;

    [Header("This is a poninter to Move Look At ")]
    [SerializeField] private GameObject pointerAim=null;
    [SerializeField] private GameObject pointer = null;
    [SerializeField] private GameObject leftHandBone=null;
    [SerializeField] private bool isAiming = false;

    [Header("Variable to Adjust Pointer Aim")]
    [SerializeField]private Vector3 adjustCameraPostionIdle = new Vector3(1.5f, -3f, -1f);
    [SerializeField]private Vector3 adjustCameraRotationWalking = new Vector3(1f, -3f, 0f);

    private CameraControllerPlayer cameraControllerPlayer = null;
    private Animator animator;


    private void Start()
    {
        if (animator==null)
        {
            animator = GetComponent<Animator>();
        }

        if (cameraControllerPlayer==null)
        {
            cameraControllerPlayer = GetComponent<CameraControllerPlayer>();
        }
    }
    private void PointerMove()
    {
        Vector3 temp = Input.mousePosition;
        temp.z = 10f; // Set this to be the distance you want the object to be placed in front of the camera.

        Camera camera = cameraControllerPlayer.GetChooseCamera().GetComponent<Camera>();

        pointer.transform.position = camera.ScreenToWorldPoint(temp);
        GetComponent<PlayerMirrorController>().CheckIsClientSendToServerComand(pointer.transform.position);
    }
    private void LateUpdate()
    {
        if (cameraControllerPlayer != null && isLocalPlayer)
        {
            PointerMove();
        }

        if (isAiming)
        {
            BoneLookAtAimClients();
            GetComponent<PlayerController>().TurnCharacterToCamera();
        }

    }
    private void BoneLookAtAimClients()
    {
        if (GetComponent<PlayerController>().GetIsWalkingDef())
        {
            pointerAim.transform.localPosition = adjustCameraRotationWalking;
            BoneMovement();
        }
        else
        {
            pointerAim.transform.localPosition = adjustCameraPostionIdle;
            BoneMovement();
        }
    }
    private void BoneMovement()
    {
        spineBone.transform.LookAt(pointerAim.transform);
        spine1Bone.transform.LookAt(pointerAim.transform);
        spine2Bone.transform.LookAt(pointerAim.transform);
        Debug.DrawLine(leftHandBone.transform.position, pointerAim.transform.position, Color.magenta);
    }


    #region Switch Aiming Player
    [Command]
    public void SwitchAdmingPlayerServer(bool aimingStatus)
    {
        SwitchAdmingPlayer(aimingStatus);
        SwitchAdmingPlayerClient(aimingStatus);
    }
    public void SwitchAdmingPlayer(bool aimingStatus)
    {
        isAiming = aimingStatus;
    }
    [ClientRpc]
    public void SwitchAdmingPlayerClient(bool aimingStatus)
    {
        SwitchAdmingPlayer(aimingStatus);
    }
    #endregion


}
