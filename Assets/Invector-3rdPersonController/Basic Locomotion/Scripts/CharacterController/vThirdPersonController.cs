using UnityEngine;
using System.Collections;

namespace Invector.CharacterController
{
    public class vThirdPersonController : vThirdPersonAnimator
    {
        #region Variables

        public static vThirdPersonController instance;

        #endregion
        PhotonView m_PhotonView;
        protected virtual void Awake()
        {
            m_PhotonView = GetComponent<PhotonView>();
            enabled = m_PhotonView == null ? true : m_PhotonView.isMine;
            StartCoroutine(UpdateRaycast()); // limit raycasts calls for better performance            
        }

        protected virtual void Start()
	    {
		    if (instance == null)
		    {
			    instance = this;
			    DontDestroyOnLoad(this.gameObject);
			    this.gameObject.name = gameObject.name + " Instance";
		    }
		    else
		    {
                PhotonNetwork.Destroy(this.gameObject);
			    return;
		    }		         

            #if !UNITY_EDITOR
                Cursor.visible = false;
            #endif
        }                
      
        #region Locomotion Actions
        
        public virtual void Sprint(bool value)
        {
            if(value)
            {
                if (currentStamina > 0 && input.sqrMagnitude > 0.1f)
                {
                    if (isGrounded && !isCrouching)
                        isSprinting = !isSprinting;
                }
            }
            else if (currentStamina <= 0 || input.sqrMagnitude < 0.1f || actions || isStrafing && !strafeWalkByDefault && (direction >= 0.5 || direction <= -0.5 || speed <= 0))
            {                
                isSprinting = false;
            }                
        }

        public virtual void Crouch()
        {                                    
            if (isGrounded && !actions)
            {
                if (isCrouching && CanExitCrouch())
                    isCrouching = false;
                else
                    isCrouching = true;
            }                
        }

        public virtual void Strafe()
        {
            isStrafing = !isStrafing;
        }

        public virtual void Jump()
        {
            if (animator.IsInTransition(0)) return;
                       
            // know if has enough stamina to make this action
	        bool staminaConditions = currentStamina > jumpStamina;
            // conditions to do this action
            bool jumpConditions = !isCrouching && isGrounded && !actions && staminaConditions && !isJumping;
            // return if jumpCondigions is false
            if (!jumpConditions) return;	        
	        // trigger jump behaviour
	        jumpCounter = jumpTimer;
	        isJumping = true;
            // trigger jump animations
            if (input.sqrMagnitude < 0.1f)
	            animator.CrossFadeInFixedTime("Jump", 0.1f);
            else
	            animator.CrossFadeInFixedTime("JumpMove", .2f);	        
	        // reduce stamina
            ReduceStamina(jumpStamina, false);
            currentStaminaRecoveryDelay = 1f;
        }

        public virtual void Roll()
        {
            if (animator.IsInTransition(0)) return;

	        bool staminaCondition = currentStamina > rollStamina;
            // can roll even if it's on a quickturn or quickstop animation
            bool actionsRoll = !actions || (actions && (quickStop));
            // general conditions to roll
            bool rollConditions = (input != Vector2.zero || speed > 0.25f) && actionsRoll && isGrounded && staminaCondition && !isJumping;

	        if (!rollConditions || isRolling) return;
	        
	        animator.SetTrigger("ResetState");
            animator.CrossFadeInFixedTime("Roll", 0.1f);
            ReduceStamina(rollStamina, false);
            currentStaminaRecoveryDelay = 2f;
        }      

        /// <summary>
        /// Use another transform as  reference to rotate
        /// </summary>
        /// <param name="referenceTransform"></param>
        public virtual void RotateWithAnotherTransform(Transform referenceTransform)
        {
            var newRotation = new Vector3(transform.eulerAngles.x, referenceTransform.eulerAngles.y, transform.eulerAngles.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newRotation), strafeRotationSpeed * Time.fixedDeltaTime);
            targetRotation = transform.rotation;
        }

        #endregion

        #region Check Action Triggers 
        
        /// <summary>
        /// Call this in OnTriggerEnter or OnTriggerStay to check if enter in triggerActions     
        /// </summary>
        /// <param name="other">collider trigger</param>                         
        public virtual void CheckTriggers(Collider other)
        {
            try
            {
                CheckForTriggerAction(other);
                CheckForLadderAction(other);
                CheckForAutoCrouch(other);
            }
            catch (UnityException e)
            {
                Debug.LogWarning(e.Message);
            }
        }

        /// <summary>
        /// Call this in OnTriggerExit to check if exit of triggerActions 
        /// </summary>
        /// <param name="other"></param>
        public virtual void CheckTriggerExit(Collider other)
        {
            TriggerActionExit(other);
            AutoCrouchExit(other);
        }

        #region TriggerAction  
         
        /// <summary>
        /// Execute TriggerAction
        /// </summary>
        /// <param name="triggerAction"></param>
        public virtual void TriggerAction(vTriggerAction triggerAction)
        {            
            if (!triggerAction.CanUse())
            {
                triggerAction = null;
                return;
            }            
            triggerAction.DoAction(this);
            // find the cursorObject height to match with the character animation
            matchTarget = triggerAction.target;
            // align the character rotation with the object rotation
            if (triggerAction.useTriggerRotation)
            {
                var rot = triggerAction.transform.rotation;
                transform.rotation = rot;
            }

            if (!string.IsNullOrEmpty(triggerAction.playAnimation))
            {
                // play the animation                
                animator.CrossFadeInFixedTime(triggerAction.playAnimation, 0.1f);                
            }            
        }

        private Vector3 surface;
        protected IEnumerator UpdateRaycast()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();

                AutoCrouch();
                StopMove();
            }
        }

        protected virtual void TriggerActionExit(Collider other)
        {
            if (other.CompareTag("Action"))
            {
                triggerAction = null;
                ladderAction = null;
            }
        }

        protected virtual void CheckForTriggerAction(Collider other)
        {
            if(!other.gameObject.CompareTag("Action") || triggerAction != null) return;

            var _triggerAction = other.GetComponent<vTriggerAction>();

            if (_triggerAction == null)
            {
                triggerAction = null;
                return;
            }
            var dist = Vector3.Distance(transform.forward, _triggerAction.transform.forward);           
            if(!_triggerAction.activeFromForward || dist <= 0.8f)
            {
                triggerAction = _triggerAction;
                AutoTriggerAction(triggerAction);
            }                
            else
            {
                triggerAction = null;
            }                
        }

        protected virtual void AutoTriggerAction(vTriggerAction triggerAction)
        {
            if (animator.IsInTransition(0) || actions) return;
           
            // do the action automatically if checked with autoAction
            if (triggerAction.autoAction && !actions)
            {                
                TriggerAction(triggerAction);                
            }                
        }

       
        #endregion

        #region LadderAction

        public virtual void CheckForLadderAction(Collider other)
        {
            if (!other.gameObject.CompareTag("Action") || !other.GetComponent<vTriggerLadderAction>())
                return;
            // assign the component - it will be null when he exit the trigger area
            var _ladderAction = other.GetComponent<vTriggerLadderAction>();

            // check the maxAngle too see if the character can do the action
            var dist = Vector3.Distance(transform.forward, _ladderAction.transform.forward);

            if (isUsingLadder && _ladderAction != null)
                ladderAction = _ladderAction;
            else if (dist <= 0.8f && !isUsingLadder)
            {
                ladderAction = _ladderAction;
                AutoEnterLadder(ladderAction);
            }
            else
            {
                ladderAction = null;
            }
        }

        public virtual void AutoEnterLadder(vTriggerLadderAction ladderAction)
        {
            if (isUsingLadder || animator.IsInTransition(0)) return;            
            // find the direction
            var dist = Vector3.Distance(targetDirection, ladderAction.transform.forward);
            // enter the ladder automatically if checked with autoAction
            if (ladderAction.autoAction  && dist <= 0.8f && input != Vector2.zero && !actions)
                TriggerEnterLadder(ladderAction);       
        }

        public virtual void TriggerEnterLadder(vTriggerLadderAction ladderAction)
        {           
            // find the cursorObject height to match with the character animation
            matchTarget = ladderAction.enterTarget;
            // align the character rotation with the object rotation
            var rot = ladderAction.transform.rotation;
            transform.rotation = rot;            
            // play the animation
            animator.CrossFadeInFixedTime(ladderAction.enterAnimation, 0.1f);
        }
        
        /// <summary>
        /// Exit Ladder
        /// </summary>
        /// <param name="value">ActionState value</param>
        /// <param name="immediateExit">set this to exit while using the ladder </param>
        public virtual void ExitLadder(int value, bool immediateExit = false)
        {            
            if (immediateExit)
            {
                isUsingLadder = false;
                EnableGravityAndCollision(0);
                animator.SetInteger("ActionState", 0);
            }
            else
            {
                exitingLadder = true;
                animator.SetInteger("ActionState", value);
            }
        }

        #endregion

        #region Crouch Methods

        protected virtual void AutoCrouch()
        {
            if (autoCrouch)
                isCrouching = true;

            if (autoCrouch && !inCrouchArea && CanExitCrouch())
            {
                autoCrouch = false;
                isCrouching = false;
            }
        }

        protected virtual bool CanExitCrouch()
        {
            // radius of SphereCast
            float radius = _capsuleCollider.radius * 0.9f;
            // Position of SphereCast origin stating in base of capsule
            Vector3 pos = transform.position + Vector3.up * ((colliderHeight * 0.5f) - colliderRadius);
            // ray for SphereCast
            Ray ray2 = new Ray(pos, Vector3.up);

            // sphere cast around the base of capsule for check ground distance
            if (Physics.SphereCast(ray2, radius, out groundHit, headDetect - (colliderRadius * 0.1f), autoCrouchLayer))
                return false;
            else
                return true;
        }

        protected virtual void AutoCrouchExit(Collider other)
        {
            if (other.CompareTag("AutoCrouch"))
            {
                inCrouchArea = false;
            }
        }

        protected virtual void CheckForAutoCrouch(Collider other)
        {
            if (other.gameObject.CompareTag("AutoCrouch"))
            {
                autoCrouch = true;
                inCrouchArea = true;
            }
        }

        #endregion

        #endregion
    }
}