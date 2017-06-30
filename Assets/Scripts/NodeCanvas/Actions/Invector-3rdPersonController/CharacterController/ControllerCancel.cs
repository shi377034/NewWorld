using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Invector.CharacterController;

namespace NodeCanvas.Tasks.Actions{

	[Category("CharacterController")]
	public class ControllerCancel : ActionTask<vThirdPersonController>{

		protected override string OnInit(){
			return null;
		}

		protected override void OnExecute(){

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