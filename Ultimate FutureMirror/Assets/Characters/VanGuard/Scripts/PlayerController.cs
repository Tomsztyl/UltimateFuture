using Mirror;
using Mirror.Examples.NetworkRoom;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum GunKind
{
    None,
    Object,
    Pistol,
    Rifle
}

public class PlayerController : NetworkBehaviour
{
    private Animator anim;

    //Default Const Speed Player
    [Header("This is a basic Speed For The player")]
    [Tooltip("This is Variables Const basic Speed Player")]
    [SerializeField] private float basicSpeed = 10f;
    [Tooltip("This is Variable Const basic Speed Rotation Player")]
    [SerializeField] private float basicRotationSpeed = 100f;

    //Actual Speed Player
    [Header("This is Actuall Speed And Rotation Player")]
    [Tooltip("This variable set Actual Speed Player")]
    private float acctualSpeed;
    [Tooltip("This variable set Actual Speed Roration Player")]
    private float acctualSpeedRotation;

    //Default Settings Move Player
    [Header("Default Settings Move Player")]
    private float speedAnimationDef = 0f;
    private float rotationAnimationDef = 0f;
    private bool isWalkingDef = false;

    //Default Settings Move Player Aim Gun
    [Header("Default Settings Move Player Aim Gun")]
    [SerializeField] private KeyCode aimGun = KeyCode.Mouse1;
    private BoneAimingController boneAimingController;
    private CameraControllerPlayer cameraControllerPlayer;


    //Default Setting Sprint Player
    [Header("Default Setting Sprint Player")]
    [SerializeField] private bool isSprinting = false;  
    [SerializeField] private float speedSprintToAdd = 3f;
    [SerializeField] private float rotationSpeedSprintingToAdd = 10f;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [Tooltip("Player stamina time")]
    [SerializeField] private float stamineTime = 10f;
    [Tooltip("Player stamina running time")]
    [SerializeField] private float calculateStamine = 0f;
    private float speedSprintCalculate = 0f;
    private float speedRotationSprintCalculate = 0f;
    private bool CR_runningSprint = false;
    private bool isExhaustion = false;

    //Default Setting Junp Player
    [Header("Default Setting Junp Player")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private float jumpAceleration = 100f;
    [SerializeField] private LayerMask ignoreLayerGround;
    [SerializeField] private float sizeRayJump = 0.7f;
    private CapsuleCollider capsuleCollider; 
    private Rigidbody rigidbody;


    //Variables To check is gun
    [Header("Variables To check Kind Gun")]
    [Tooltip("This enum pointer check kind gun in hand player")]
    public GunKind GunKind=GunKind.None;

    [Header("Mechanic Camera Rotation Character where looking Camera")]
    [SerializeField] private bool isWalkingWhereLookingCamera = true;
    [SerializeField]private float rotationSpeed = 30f;
    [SerializeField]private float deadZoneDegrees = 15f;
    private Transform cameraCurent;
    private Vector3 cameraDirection;
    private Vector3 playerDirection;
    private Quaternion targetRotation;

    void Start()
    {
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        rigidbody = GetComponent<Rigidbody>();
        boneAimingController = GetComponent<BoneAimingController>();
        cameraControllerPlayer = GetComponent<CameraControllerPlayer>();
        SetBasicSpeed();
    }
    


    // Update is called once per frame
    void Update()
    {
        PlayerMoveSelectGunCharacter();
        PlayerJump();
    }
    #region Camera Rotation Where Player Looking
    public void SetCurrentCameraTranform(Transform camera)
    {
        cameraCurent = camera;
    }
    public void TurnCharacterToCamera()
    {
        if (cameraCurent == null) return;
        if (isWalkingWhereLookingCamera)
        { 
            cameraDirection = new Vector3(cameraCurent.forward.x, 0f, cameraCurent.forward.z);
            playerDirection = new Vector3(transform.forward.x, 0f, transform.forward.z);

            if (Vector3.Angle(cameraDirection, playerDirection) > deadZoneDegrees)
            {
                targetRotation = Quaternion.LookRotation(cameraDirection, transform.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed* Time.deltaTime);
            }
        }
    }
    #endregion
    #region Player Move Kind Character
    private void PlayerMoveSelectGunCharacter()
    {
        if (GunKind == GunKind.None)
        {
            anim.SetBool("isGun", false);
            StopAimingTrigger();
            PlayerCheckIsSprint();
            PlayerMove(acctualSpeed, acctualSpeedRotation, "speed");
            PlayerMoveAnimation(speedAnimationDef, rotationAnimationDef, isWalkingDef, "isRunBack", "isLeftTurn", "isRightTurn");
        }
        else if (GunKind==GunKind.Object)
        {
            anim.SetBool("isGun", false);
            StopAimingTrigger();
            PlayerCheckIsSprint();
            PlayerMove(acctualSpeed, acctualSpeedRotation, "speed");
            PlayerMoveAnimation(speedAnimationDef, rotationAnimationDef, isWalkingDef, "isRunBack", "isLeftTurn", "isRightTurn");
        }
        else if (GunKind == GunKind.Rifle)
        {
            anim.SetBool("isGun", true);
            PlayerAimGunStanding();
            PlayerCheckIsSprint();
            PlayerMove(acctualSpeed, acctualSpeedRotation, "speedGunAim");
            PlayerMoveAnimation(speedAnimationDef, rotationAnimationDef, isWalkingDef, "isRunBack", "isLeftTurn", "isRightTurn");
        }
        else
        {
            //Default Option
            anim.SetBool("isGun", false);
            PlayerMove(acctualSpeed, acctualSpeedRotation, "speed");
            PlayerMoveAnimation(speedAnimationDef, rotationAnimationDef, isWalkingDef, "isRunBack", "isLeftTurn", "isRightTurn");
        }
    }

    private void PlayerMove(float speed, float rotationSpeed, string speedChoiceAnimator)
    {
        float translation = Input.GetAxis("Vertical") * speed;
        speedAnimationDef = translation;
        anim.SetFloat(speedChoiceAnimator, speedAnimationDef);
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        rotationAnimationDef = rotation;
        translation *= Time.deltaTime;

        rotation *= Time.deltaTime;

        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        if (speedAnimationDef != 0)
        {
            //Is Walking
            anim.SetBool("isWalking", true);
            isWalkingDef = true;
            TurnCharacterToCamera();
        }
        else
        {
            //Is no Walking
            anim.SetBool("isWalking", false);
            isWalkingDef = false;
        }
    }
    public bool GetIsWalkingDef()
    {
        return isWalkingDef;
    }
    private void PlayerMoveAnimation(float speedAnimation, float rotationAnimation, bool isWalking, string isRunBack, string isLeftTurn, string isRightTurn)
    {

        if (speedAnimation < 0f)
        {
            anim.SetBool(isRunBack, true);
        }
        else
        {
            anim.SetBool(isRunBack, false);
        }



        if (rotationAnimation < 0f && isWalking == false)
        {
            //Animation Left
            anim.SetBool(isLeftTurn, true);
        }
        else
        {
            anim.SetBool(isLeftTurn, false);
        }

        if (rotationAnimation > 0f && isWalking == false)
        {
            //Animation Right
            anim.SetBool(isRightTurn, true);
        }
        else
        {
            anim.SetBool(isRightTurn, false);
        }
    }
    #endregion
    #region Aiming Player
    private void PlayerAimGunStanding()
    {
        if (Input.GetKeyDown(aimGun))
        {
            //Start Aiming
            StartAimingTrigger();
        }
        if (Input.GetKeyUp(aimGun))
        {
            //Stop Aiming
            StopAimingTrigger();
        }
    }
    private void StartAimingTrigger()
    {
        anim.SetBool("isAiming", true);
        boneAimingController.SwitchAdmingPlayerServer(true);
        cameraControllerPlayer.ChangePivotCameraRifle();
        GetComponent<FightController>().StartAiming();
    }
    private void StopAimingTrigger()
    {
        anim.SetBool("isAiming", false);
        boneAimingController.SwitchAdmingPlayerServer(false);
        cameraControllerPlayer.ChangePivotCameraRifleDefault();
        GetComponent<FightController>().StopAiming();
    }

    public float GetRotationAnimationDef()
    {
        return rotationAnimationDef;
    }
    #endregion
    #region Mechanic Sprint Player Calcualte
    public void RestetCalculateVariable()
    {
        speedSprintCalculate = 0f;
        speedRotationSprintCalculate = 0f;
    }
    public void SetDefaultSpeedToVariableCalculate()
    {
        speedSprintCalculate = acctualSpeed;
        speedRotationSprintCalculate = acctualSpeedRotation;
        isSprinting = false;
    }

    private void PlayerCheckIsSprint()
    {
        TriggerExhaustionCharacter();
        if (!isExhaustion)
        { 
            if (Input.GetKeyDown(sprintKey))
            {
                //Start Sprinting
                SetDefaultSpeedToVariableCalculate();
                acctualSpeed += speedSprintToAdd;
                acctualSpeedRotation += rotationSpeedSprintingToAdd;

                //Exhaustion calculation
                isSprinting = true;

            }

            if (Input.GetKeyUp(sprintKey))
            {
                //Stop Sprinting
                acctualSpeed = speedSprintCalculate;
                acctualSpeedRotation = speedRotationSprintCalculate;
                RestetCalculateVariable();

                //  Exhaustion calculation
                isSprinting = false;
            }
        }
        else
        {
            //Stop Sprinting
            acctualSpeed = speedSprintCalculate;
            acctualSpeedRotation = speedRotationSprintCalculate;

            //  Exhaustion calculation
            isSprinting = false;
        }
    }
    #region Exhaustion Character
    private void TriggerExhaustionCharacter()
    {
        if (calculateStamine >= stamineTime)
        {
            isExhaustion = true;
        }
        else
        {
            isExhaustion = false;
        }


        if (isSprinting && calculateStamine < stamineTime && !CR_runningSprint && isWalkingDef)
        {
            StartCoroutine(CorutineExhaustionAdd());
        }
        else if (!isSprinting && calculateStamine > 0 && calculateStamine <= stamineTime && !CR_runningSprint)
        {
            StartCoroutine(CorutineExhaustionGrab());
        }
        else if (isSprinting&&!isWalkingDef && !CR_runningSprint && calculateStamine <= stamineTime&& calculateStamine > 0)
        {
            StartCoroutine(CorutineExhaustionGrab());
        }
    }
    private IEnumerator CorutineExhaustionAdd()
    {
        CR_runningSprint = true;
        calculateStamine++;
        yield return new WaitForSeconds(1f);
        CR_runningSprint = false;
    }
    private IEnumerator CorutineExhaustionGrab()
    {
        CR_runningSprint = true;
        calculateStamine--;
        yield return new WaitForSeconds(1f);
        CR_runningSprint = false;
    }
    public float GetCalculateStamine()
    {
        return calculateStamine;
    }
    public float GetStamineTime()
    {
        return stamineTime;
    }
    #endregion
    #endregion
    #region Setting Speed Player Properties
    public void SetSpeedDefProperties(float speed, float speedRotation)
    {
        acctualSpeed = speed;
        acctualSpeedRotation = speedRotation;
    }
    public void SetSpeedGunAimProperties(float speed, float speedRotation)
    {
        acctualSpeed = speed;
        acctualSpeedRotation = speedRotation;
    }
    public void SetBasicSpeed()
    {
        acctualSpeed = basicSpeed;
        acctualSpeedRotation = basicRotationSpeed;
    }
    #endregion
    #region Mechanics Player Jumping
    private void PlayerJump()
    {
        if (Input.GetKeyDown(jumpKey) && RayToCheckIsGorund())
        {
           // Debug.LogWarning("Jump");
            rigidbody.AddForce(new Vector3(0, jumpAceleration, 0), ForceMode.Impulse);
            anim.SetBool("isJumping", true);
        }
        else
        {
            CheckIsGrounded();
        }


    }
    private void CheckIsGrounded()
    {
        if (RayToCheckIsGorund())
        {
            anim.SetBool("isJumping", false);
        }
    }
    private bool RayToCheckIsGorund()
    {
        RaycastHit hit;


        if (Physics.Raycast(this.transform.position, this.transform.up*-1.0f, out hit, sizeRayJump, ignoreLayerGround))
        {
            //Send To Server to Clinets Point Hit
            Debug.DrawLine(this.transform.position, hit.point,Color.green);
            return true;
        }
        return false;
    }
    #endregion
}
