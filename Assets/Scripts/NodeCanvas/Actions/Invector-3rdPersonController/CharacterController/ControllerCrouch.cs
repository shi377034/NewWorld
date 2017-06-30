using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Invector.CharacterController;

namespace NodeCanvas.Tasks.Actions{
    [Name("Crouch")]
	[Category("CharacterController")]
	public class ControllerCrouch : ActionTask<vThirdPersonController>{

		protected override string OnInit(){
			return null;
		}

		protected override void OnExecute(){
            agent.Crouch();
			EndAction(true);
		}

		protected override void OnUpdate(){
			
		}

		protected override void OnStop(){
			
		}

		protected override void OnPause(){
			
		}
	}
}