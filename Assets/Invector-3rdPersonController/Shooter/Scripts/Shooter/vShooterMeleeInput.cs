using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Invector;
using Invector.CharacterController;
using Invector.IK;
using System;

public class vShooterMeleeInput : vMeleeCombatInput
{

    #region Shooter Inputs

    [Header("Shooter Input")]
    public GenericInput aimInput = new GenericInput("Mouse1", false, "LT", true, "LT", false);
    public GenericInput shotInput = new GenericInput("Mouse0", false, "RT", true, "RT", false);
    public GenericInput reloadInput = new GenericInput("R", "LB", "LB");
    public GenericInput switchCameraSideInput = new GenericInput("Tab", "RightStickClick", "RightStickClick");
    public GenericInput switchScopeViewInput = new GenericInput("Z", "RB", "RB");

    #endregion

    #region Shooter Variables       

    [HideInInspector]
    public vShooterManager shooterManager;
    [HideInInspector]
    public bool isAiming;
    [HideInInspector]
    public bool canEquip;
    [HideInInspector]
    public bool isReloading;
    [HideInInspector]
    public Transform leftHand, rightHand, rightUpperArm;

    protected int onlyArmsLayer;
    protected bool allowAttack;
    protected bool aimConditions;
    protected bool isUsingScopeView;
    protected bool isCameraRightSwitched;
    protected bool updateIK;
    protected float onlyArmsLayerWeight;
    protected float lIKWeight;
    protected float rightRotationWeight;
    protected float aimWeight;
    protected float lastAimDistance;
    protected Quaternion handRotation, upperArmRotation;
    protected vIKSolver leftIK;
    protected Vector3 aimPosition;
    protected vHeadTrack headTrack;    
    private vControlAimCanvas _controlAimCanvas;

    public vControlAimCanvas controlAimCanvas
    {
        get
        {
            if (!_controlAimCanvas)
                _controlAimCanvas = FindObjectOfType<vControlAimCanvas>();
            return _controlAimCanvas;
        }
    }

    public Animator animator
    {
        get
        {
            if (cc == null) cc = GetComponent<vThirdPersonController>();
            if (cc.animator == null) return GetComponent<Animator>();
            return cc.animator;
        }
    }

    #endregion
    
    protected override void Start()
    {
        shooterManager = GetComponent<vShooterManager>();

        base.Start();

        leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        onlyArmsLayer = animator.GetLayerIndex("OnlyArms");
        headTrack = GetComponent<vHeadTrack>();
        if (headTrack)
            headTrack.onFinishUpdate.AddListener(UpdateAimBehaviour);
        if (!controlAimCanvas)
        {
            Debug.LogWarning("Don't exist a AimCanvas in current scene", gameObject);
        }
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
    }

    protected override void FixedUpdate()
    {        
        base.FixedUpdate();
        UpdateMeleeAnimations();
        updateIK = true;
    }

    #region Shooter Inputs    

    protected override void InputHandle()
    {       
        if (cc == null) return;

        #region MeleeInput

        if (MeleeAttackConditions && !isAiming && !isReloading && !lockInputByItemManager)
        {
            MeleeWeakAttackInput();
            MeleeStrongAttackInput();
            BlockingInput();
        }
        else
            isBlocking = false;

        #endregion

        #region BasicInput

        if (!isAttacking)
        {
            ExitGameInput();

            if (!cc.lockMovement && !cc.ragdolled)
            {
                MoveCharacter();
                SprintInput();
                CrouchInput();
                StrafeInput();
                JumpInput();
                RollInput();
                ActionInput();
                EnterLadderInput();
                ExitLadderInput();
                if (shooterManager.useLockOn && shooterManager.rWeapon != null || shooterManager.useLockOnMeleeOnly && shooterManager.rWeapon == null)
                    LockOnInput();
                else if (shooterManager.rWeapon != null)
                {
                    isLockingOn = false;
                    tpCamera.UpdateLockOn(false);
                }
            }

            UpdateMeleeAnimations();
        }
        else
            cc.input = Vector2.zero;

        #endregion

        #region ShooterInput

        if (shooterManager.rWeapon == null || lockInputByItemManager)
        {
            isAiming = false;
            controlAimCanvas.SetActiveAim(false);
            controlAimCanvas.SetActiveScopeCamera(false);
            return;
        }
        AimInput();       
        ReloadInput();
        SwitchCameraSide();
        SwitchScopeViewInput();

        #endregion
    }

    public override bool lockInventory
    {
        get
        {
            return base.lockInventory || isReloading;
        }
    }

    void AimInput()
    {
        if (cc.locomotionType == vThirdPersonMotor.LocomotionType.OnlyFree)
        {
            Debug.LogWarning("Shooter behaviour needs to be OnlyStrafe or Free with Strafe. \n Please change the Locomotion Type.");
            return;
        }

        if (!shooterManager || shooterManager.rWeapon == null)
        {
            if (controlAimCanvas)
            {
                controlAimCanvas.SetActiveAim(false);
                controlAimCanvas.SetActiveScopeCamera(false);
            }
            isAiming = false;
            return;
        }

        isAiming = !isReloading && (aimInput.GetButton() || (shooterManager.alwaysAiming)) && !cc.actions && !cc.doingCustomAction || (cc.actions && cc.isJumping);

        if (cc.locomotionType == vThirdPersonMotor.LocomotionType.FreeWithStrafe)
        {
            if (isAiming && !cc.isStrafing)
            {
                cc.Strafe();
            }
            else if (!isAiming && cc.isStrafing)
            {
                cc.Strafe();
            }
        }

        if (controlAimCanvas)
        {
            if (isAiming && !controlAimCanvas.isAimActive)
                controlAimCanvas.SetActiveAim(true);
            if (!isAiming && controlAimCanvas.isAimActive)
                controlAimCanvas.SetActiveAim(false);
        }

        shooterManager.rWeapon.SetActiveAim(isAiming && aimConditions);
        shooterManager.rWeapon.SetActiveScope(isAiming && isUsingScopeView);
    }

    void ShotInput()
    {
        if (!shooterManager || shooterManager.rWeapon == null) return;
        if (isAiming && !shooterManager.canAttack && aimConditions && !isReloading && !isAttacking)
        {
            if (shooterManager.rWeapon.automaticWeapon ? shotInput.GetButton() : shotInput.GetButtonDown())
                shooterManager.Shoot(aimPosition);

            else if (shotInput.GetButtonDown())
            {
                if (allowAttack == false)
                {
                    shooterManager.Shoot(aimPosition);
                    allowAttack = true;
                }
            }
            else allowAttack = false;
        }
    }

    void ReloadInput()
    {
        if (!shooterManager || shooterManager.rWeapon == null) return;
        if (reloadInput.GetButtonDown() && !cc.actions && !cc.ragdolled)
            shooterManager.ReloadWeapon();
    }

    void SwitchCameraSide()
    {
        if (switchCameraSideInput.GetButtonDown())
        {
            isCameraRightSwitched = !isCameraRightSwitched;
            tpCamera.SwitchRight(isCameraRightSwitched);
        }
    }

    void SwitchScopeViewInput()
    {
        if (!shooterManager || shooterManager.rWeapon == null) return;
        if (isAiming && aimConditions && switchScopeViewInput.GetButtonDown())
        {
            if (controlAimCanvas && shooterManager.rWeapon.scopeTarget)
            {
                isUsingScopeView = !isUsingScopeView;
                controlAimCanvas.SetActiveScopeCamera(isUsingScopeView, shooterManager.rWeapon.useUI);
            }
        }
        else if (controlAimCanvas && !isAiming || !aimConditions)
        {
            isUsingScopeView = false;
            controlAimCanvas.SetActiveScopeCamera(false);
        }
    }

    protected override void BlockingInput()
    {
        if (shooterManager == null || shooterManager.rWeapon == null)
            base.BlockingInput();
    }

    protected override void ExitGameInput()
    {
        // just a example to quit the application 
        if (Input.GetKeyDown(KeyCode.Escape) && !lockInventory)
        {           
            Application.Quit();
        }
    }  

    protected override void RotateWithCamera(Transform cameraTransform)
    {
        if (cc.isStrafing && !cc.actions && !cc.lockMovement)
        {
            // smooth align character with aim position
            if (tpCamera != null && tpCamera.lockTarget)
            {
                cc.RotateToTarget(tpCamera.lockTarget);
            }
            // rotate the camera around the character and align with when the char move
            else if (cc.input != Vector2.zero || isAiming)
            {
                cc.RotateWithAnotherTransform(cameraTransform);
            }
        }
    }

    #endregion

    #region Update Animations

    protected override void UpdateMeleeAnimations()
    {
        // disable the onlyarms layer and run the melee methods if the character is not using any shooter weapon
        if (!cc.animator) return;

        // disable strafe input while holding a shooter weapon, so you can use the SwitchCameraSide input instead
        strafeInput.useInput = shooterManager.rWeapon == null;    
        
        if (shooterManager == null || shooterManager.rWeapon == null)
            base.UpdateMeleeAnimations();

        UpdateShooterAnimations();
    }

    protected virtual void UpdateShooterAnimations()
    {
        if (shooterManager == null) return;

        if (!isAiming && meleeManager)
        {
            // set attack id from the melee weapon (trigger fullbody atk animations)
            cc.animator.SetInteger("AttackID", meleeManager.GetAttackID());
        }
        else
        {           
            // set attack id from the shooter weapon (trigger shot layer animations)
            cc.animator.SetInteger("AttackID", shooterManager.GetAttackID());
        }
        // turn on the onlyarms layer to aim 
        onlyArmsLayerWeight = Mathf.Lerp(onlyArmsLayerWeight, isAiming ? 0f : shooterManager.rWeapon ? 1f : 0f, 6f * Time.deltaTime);
        animator.SetLayerWeight(onlyArmsLayer, onlyArmsLayerWeight);

        if (shooterManager.rWeapon != null && isAiming)
        {            
            // set the move set id (base layer) 
            cc.animator.SetFloat("MoveSet_ID", shooterManager.GetMoveSetID(), .2f, Time.deltaTime);         
        }
        else if (shooterManager.rWeapon != null)
        {
            // set the move set id (base layer) 
            cc.animator.SetFloat("MoveSet_ID", 0, .2f, Time.deltaTime);
        }
       
        // set the uppbody id (armsonly layer)
        cc.animator.SetFloat("UpperBody_ID", shooterManager.GetUpperBodyID(), .2f, Time.deltaTime);
        // set if the character can aim or not (upperbody layer)
        cc.animator.SetBool("CanAim", aimConditions);
        // character is aiming
        cc.animator.SetBool("IsAiming", isAiming && !isAttacking);
        // find states with the Reload tag
        isReloading = cc.IsAnimatorTag("Reload");
    }   

    protected override void UpdateCameraStates()
    {
        // CAMERA STATE - you can change the CameraState here, the bool means if you want lerp of not, make sure to use the same CameraState String that you named on TPCameraListData

        if (tpCamera == null)
        {
            tpCamera = FindObjectOfType<vThirdPersonCamera>();
            if (tpCamera == null)
                return;
            if (tpCamera)
            {
                tpCamera.SetMainTarget(this.transform);
                tpCamera.Init();
            }
        }

        if (changeCameraState && !cc.isStrafing)
            tpCamera.ChangeState(customCameraState, customlookAtPoint, smoothCameraState);
        else if (cc.isCrouching)
            tpCamera.ChangeState("Crouch", true);
        else if (cc.isStrafing && shooterManager.rWeapon == null)
            tpCamera.ChangeState("Strafing", true);
        else if (isAiming && shooterManager.rWeapon != null)
            tpCamera.ChangeState("Aiming", true);
        else
            tpCamera.ChangeState("Default", true);
    }

    #endregion

    #region Update Aim

    protected virtual void UpdateAimPosition()
    {
        if (!shooterManager || shooterManager.rWeapon == null) return;

        var camT = isUsingScopeView && controlAimCanvas && controlAimCanvas.scopeCamera ? //Check if is using canvas scope view
                  shooterManager.rWeapon.zoomScopeCamera ? /* if true, check if weapon has a zoomScopeCamera, 
                  if true...*/
                  shooterManager.rWeapon.zoomScopeCamera.transform : controlAimCanvas.scopeCamera.transform :
                  /*else*/Camera.main.transform;

        var origin1 = camT.position;
        if (!(controlAimCanvas && controlAimCanvas.isScopeCameraActive && controlAimCanvas.scopeCamera))
            origin1 = camT.position;

        var vOrigin = origin1;
        vOrigin += controlAimCanvas && controlAimCanvas.isScopeCameraActive && controlAimCanvas.scopeCamera ? camT.forward : Vector3.zero;
        aimPosition = camT.position + camT.forward * 100f;
        if (!isUsingScopeView) lastAimDistance = 100f;

        if (shooterManager.raycastAimTarget && shooterManager.rWeapon.raycastAimTarget)
        {
            RaycastHit hit;
            Ray ray = new Ray(vOrigin, camT.forward);

            if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane, shooterManager.aimTargetLayer))
            {                
                aimPosition = hit.point;
                if (!isUsingScopeView)
                    lastAimDistance = Vector3.Distance(camT.position, hit.point);
            }
            if (shooterManager.showCheckAimGizmos)
            {
                Debug.DrawLine(ray.origin, aimPosition);
            }
        }
    }

    protected virtual Vector2 AimDirection()
    {
        if (shooterManager.rWeapon == null) return Vector2.zero;

        var dir = aimPosition - (rightUpperArm.position);
        var angle = Quaternion.LookRotation(dir, transform.up);
        var euler = angle.eulerAngles - transform.eulerAngles;
        var x = euler.NormalizeAngle().x;
        var y = euler.NormalizeAngle().y;
        Vector2 aimDirection = new Vector2(x, y);
        return aimDirection;
    }

    #endregion

    #region IK behaviour

    void OnDrawGizmos()
    {
        if (!shooterManager || shooterManager.rWeapon == null) return;

        var _ray = new Ray(rightUpperArm.position, Camera.main.transform.forward);
        Gizmos.DrawRay(_ray.origin, _ray.direction * shooterManager.minDistanceToAim);
        var color = Gizmos.color;
        color = aimConditions ? Color.green : Color.red;
        color.a = 0.2f;
        Gizmos.color = color;
        Gizmos.DrawSphere(_ray.GetPoint(shooterManager.minDistanceToAim), shooterManager.checkAimRadius);
        Gizmos.DrawSphere(aimPosition, shooterManager.checkAimRadius);
    }

    protected virtual void UpdateAimBehaviour()
    {
        if (!updateIK && animator.updateMode == AnimatorUpdateMode.AnimatePhysics) return;
        updateIK = false;
        SetHeadTrackOffSet();
        RotateRightArm();
        RotateRightHand();
        CheckAimCondiction();
        UpdateLeftIK();
        UpdateAimPosition();
        UpdateAimHud();
        ShotInput();
    }

    protected virtual void SetHeadTrackOffSet()
    {
        if (!shooterManager || !shooterManager.rWeapon || !headTrack)
        {
            if (headTrack) headTrack.offsetSpine = Vector2.Lerp(headTrack.offsetSpine, Vector2.zero, headTrack.smooth * Time.deltaTime);
            return;
        }
        if (isAiming)
        {
            var offset = cc.isCrouching ? shooterManager.rWeapon.headTrackOffsetCrouch : shooterManager.rWeapon.headTrackOffset;
            headTrack.offsetSpine = Vector2.Lerp(headTrack.offsetSpine, offset, headTrack.smooth * Time.deltaTime);
        }
        else
            headTrack.offsetSpine = Vector2.Lerp(headTrack.offsetSpine, Vector2.zero, headTrack.smooth * Time.deltaTime);
    }

    protected virtual void UpdateLeftIK()
    {
        if (!shooterManager || !shooterManager.rWeapon || !shooterManager.useLeftIK) return;
        bool useIkOnIdle = cc.input.magnitude < 0.1f ? shooterManager.rWeapon.useIkOnIdle : true;
        bool useIkStrafeMoving = new Vector2(cc.animator.GetFloat("InputVertical"), cc.animator.GetFloat("InputHorizontal")).magnitude > 0.1f && cc.isStrafing ? shooterManager.rWeapon.useIkOnStrafe : true;
        bool useIkFreeMoving = cc.animator.GetFloat("InputVertical") > 0.1f && !cc.isStrafing ? shooterManager.rWeapon.useIkOnFree : true;
        bool useIkAttacking = isAttacking ? shooterManager.rWeapon.useIkAttacking : true;
        bool useIkConditions = !(!useIkOnIdle || !useIkStrafeMoving || !useIkFreeMoving || !useIkAttacking);

        // create left arm ik solver if equal null
        if (leftIK == null) leftIK = new vIKSolver(animator, AvatarIKGoal.LeftHand);
        if (leftIK != null)
        {
            // control weight of ik
            if (shooterManager.rWeapon && shooterManager.rWeapon.handIKTarget && Time.timeScale > 0 && !isReloading && !cc.actions && !cc.doingCustomAction && (cc.isGrounded || isAiming) && !cc.lockMovement && useIkConditions)
                lIKWeight = Mathf.Lerp(lIKWeight, 1, 10f * Time.deltaTime);
            else
                lIKWeight = Mathf.Lerp(lIKWeight, 0, 10f * Time.deltaTime);

            if (lIKWeight <= 0) return;
            // update IK
            leftIK.SetIKWeight(lIKWeight);
            if (shooterManager && shooterManager.rWeapon && shooterManager.rWeapon.handIKTarget)
            {
                var _offset = (shooterManager.rWeapon.handIKTarget.forward * shooterManager.ikPositionOffset.z) + (shooterManager.rWeapon.handIKTarget.right * shooterManager.ikPositionOffset.x) + (shooterManager.rWeapon.handIKTarget.up * shooterManager.ikPositionOffset.y);
                leftIK.SetIKPosition(shooterManager.rWeapon.handIKTarget.position + _offset);
                var _rotation = Quaternion.Euler(shooterManager.ikRotationOffset);
                leftIK.SetIKRotation(shooterManager.rWeapon.handIKTarget.rotation * _rotation);
            }
        }
    }

    protected virtual void CheckAimCondiction()
    {
        if (!shooterManager || shooterManager.rWeapon == null) return;

        var _ray = new Ray(rightUpperArm.position, Camera.main.transform.forward);

        if (Physics.SphereCast(_ray, shooterManager.checkAimRadius, shooterManager.minDistanceToAim, shooterManager.blockAimLayer))
        {
            aimConditions = false;
        }
        else
            aimConditions = true;
        aimWeight = Mathf.Lerp(aimWeight, aimConditions ? 1 : 0, 1 * Time.deltaTime);
    }

    protected virtual void RotateRightArm()
    {
        if (shooterManager.rWeapon && isAiming && aimConditions && shooterManager.rWeapon.alignRightUpperArmToAim)
        {
            var aimPoint = isUsingScopeView && controlAimCanvas.scopeCamera ? Camera.main.transform.position + Camera.main.transform.forward * lastAimDistance : aimPosition;
            Vector3 v = aimPoint - rightUpperArm.position;
            Vector3 v2 = Quaternion.AngleAxis(-shooterManager.rWeapon.recoilUp, shooterManager.rWeapon.aimReference.right) * v;
            var orientation = shooterManager.rWeapon.aimReference.forward;

            rightRotationWeight = Mathf.Lerp(rightRotationWeight, !shooterManager.canAttack || shooterManager.rWeapon.ammoCount <= 0 ? 1f * aimWeight : 0f, 10f * Time.deltaTime);

            var r = Quaternion.FromToRotation(orientation, v) * rightUpperArm.rotation;
            var r2 = Quaternion.FromToRotation(orientation, v2) * rightUpperArm.rotation;
            Quaternion rot = Quaternion.Lerp(r2, r, rightRotationWeight);

            var camT = controlAimCanvas && controlAimCanvas.isScopeCameraActive && controlAimCanvas.scopeCamera ? controlAimCanvas.scopeCamera.transform : Camera.main.transform;
            var angle = Vector3.Angle(aimPosition - shooterManager.rWeapon.muzzle.position, camT.forward);

            if ((!(angle > shooterManager.maxHandAngle || angle < -shooterManager.maxHandAngle)) || controlAimCanvas.isScopeCameraActive)
                upperArmRotation = Quaternion.Slerp(upperArmRotation, rot, shooterManager.smoothHandRotation * Time.deltaTime);

            if (!float.IsNaN(handRotation.x) && !float.IsNaN(handRotation.y) && !float.IsNaN(handRotation.z))
                rightUpperArm.rotation = upperArmRotation;
        }
    }

    protected virtual void RotateRightHand()
    {
        if (shooterManager.rWeapon && shooterManager.rWeapon.alignRightHandToAim && isAiming && aimConditions)
        {
            var aimPoint = isUsingScopeView && controlAimCanvas.scopeCamera ?
                      Camera.main.transform.position + Camera.main.transform.forward * lastAimDistance : aimPosition;
            Vector3 v = aimPoint - shooterManager.rWeapon.aimReference.position;
            Vector3 v2 = Quaternion.AngleAxis(-shooterManager.rWeapon.recoilUp, shooterManager.rWeapon.aimReference.right) * v;
            var orientation = shooterManager.rWeapon.aimReference.forward;
            if (!shooterManager.rWeapon.alignRightUpperArmToAim)
                rightRotationWeight = Mathf.Lerp(rightRotationWeight, !shooterManager.canAttack || shooterManager.rWeapon.ammoCount <= 0 ? 1f * aimWeight : 0f, 10f * Time.deltaTime);

            var r = Quaternion.FromToRotation(orientation, v) * rightHand.rotation;
            var r2 = Quaternion.FromToRotation(orientation, v2) * rightHand.rotation;
            Quaternion rot = Quaternion.Lerp(r2, r, rightRotationWeight);
            var camT = controlAimCanvas && controlAimCanvas.isScopeCameraActive && controlAimCanvas.scopeCamera ? controlAimCanvas.scopeCamera.transform : Camera.main.transform;
            var angle = Vector3.Angle(aimPosition - shooterManager.rWeapon.muzzle.position, camT.forward);
            if ((!(angle > shooterManager.maxHandAngle || angle < -shooterManager.maxHandAngle)) || (controlAimCanvas && controlAimCanvas.isScopeCameraActive))
                handRotation = Quaternion.Slerp(handRotation, rot, shooterManager.smoothHandRotation * Time.deltaTime);

            if (!float.IsNaN(handRotation.x) && !float.IsNaN(handRotation.y) && !float.IsNaN(handRotation.z))
                rightHand.rotation = handRotation;
            shooterManager.rWeapon.SetScopeLookTarget(aimPoint);
        }
    }

    protected virtual void UpdateAimHud()
    {
        if (!shooterManager || shooterManager.rWeapon == null || !controlAimCanvas) return;

        controlAimCanvas.SetAimCanvasID(shooterManager.rWeapon.scopeID);
        if (controlAimCanvas.scopeCamera && controlAimCanvas.scopeCamera.gameObject.activeSelf)
        {
            controlAimCanvas.SetAimToCenter(aimConditions);
        }
        else if (isAiming && aimConditions)
        {
            RaycastHit hit;
            if (Physics.Linecast(shooterManager.rWeapon.muzzle.position, aimPosition, out hit, shooterManager.blockAimLayer))
            {

                controlAimCanvas.SetWordPosition(cc,hit.point);
            }
            else
            {
                controlAimCanvas.SetWordPosition(cc,aimPosition);
            }
        }
        else
            controlAimCanvas.SetAimToCenter(false);

        if (shooterManager.rWeapon.scopeTarget )
        {
            var lookPoint = Camera.main.transform.position + (Camera.main.transform.forward * (isUsingScopeView ? lastAimDistance : 100f));
            controlAimCanvas.UpdateScopeCamera(shooterManager.rWeapon.scopeTarget.position, lookPoint, shooterManager.rWeapon.zoomScopeCamera ? 0 : shooterManager.rWeapon.scopeZoom);
        }
    }

    #endregion

}