using Invector.CharacterController;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	[Category("Camera")]
	public class CameraInput : ActionTask<vThirdPersonController>{
        public BBParameter<float> x;
        public BBParameter<float> y;
        public BBParameter<float> zoom;
        protected vThirdPersonCamera tpCamera;
        protected override string OnInit(){
            if (tpCamera == null)
            {
                tpCamera = Object.FindObjectOfType<vThirdPersonCamera>();
            }
            return null;
		}
		protected override void OnUpdate(){
            if (tpCamera == null)
                return;
            tpCamera.RotateCamera(x.value, y.value);
            tpCamera.Zoom(zoom.value);
            EndAction(true);
        }
	}
}