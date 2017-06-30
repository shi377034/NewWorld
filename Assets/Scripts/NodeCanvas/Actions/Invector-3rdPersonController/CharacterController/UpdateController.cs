using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Invector.CharacterController;
using UnityEngine;
namespace NodeCanvas.Tasks.Actions{

	[Category("CharacterController")]
	public class UpdateController : ActionTask<vThirdPersonController>{
        protected vThirdPersonCamera tpCamera;
        [HideInInspector]
        public string customCameraState;                    // generic string to change the CameraState        
        [HideInInspector]
        public string customlookAtPoint;                    // generic string to change the CameraPoint of the Fixed Point Mode        
        [HideInInspector]
        public bool changeCameraState;                      // generic bool to change the CameraState        
        [HideInInspector]
        public bool smoothCameraState;                      // generic bool to know if the state will change with or without lerp  
        protected override string OnInit()
        {
            agent.Init();
            return null;
        }
        protected override void OnUpdate(){
            UpdateCameraStates();
            agent.UpdateMotor();
            agent.UpdateAnimator();
            agent.AirControl();        
            agent.UpdateTargetDirection(tpCamera != null ? tpCamera.transform : null);
            RotateWithCamera(tpCamera != null ? tpCamera.transform : null);
            EndAction(true);
        }
        void UpdateCameraStates()
        {
            if (tpCamera == null)
            {
                tpCamera = Object.FindObjectOfType<vThirdPersonCamera>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(agent.transform);
                    tpCamera.Init();
                }
            }

            if (agent.isStrafing)
                tpCamera.ChangeState(customCameraState, customlookAtPoint, smoothCameraState);
            else if (agent.isCrouching)
                tpCamera.ChangeState("Crouch", true);
            else if (agent.isStrafing)
                tpCamera.ChangeState("Strafing", true);
            else
                tpCamera.ChangeState("Default", true);
        }
        protected void RotateWithCamera(Transform cameraTransform)
        {
            if (agent.isStrafing && !agent.actions && !agent.lockMovement)
            {
                // smooth align character with aim position               
                if (tpCamera != null && tpCamera.lockTarget)
                {
                    agent.RotateToTarget(tpCamera.lockTarget);
                }
                // rotate the camera around the character and align with when the char move
                else if (agent.input != Vector2.zero)
                {
                    agent.RotateWithAnotherTransform(cameraTransform);
                }
            }
        }
    }
}