using Invector.CharacterController;
using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions{
    [Name("Strafe")]
	[Category("CharacterController")]
	public class ControllerStrafe : ActionTask<vThirdPersonController>{

		protected override string OnInit(){
			return null;
		}

		protected override void OnExecute(){
            agent.Strafe();
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