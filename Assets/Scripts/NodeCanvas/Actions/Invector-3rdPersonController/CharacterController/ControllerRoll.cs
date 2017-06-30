using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Invector.CharacterController;

namespace NodeCanvas.Tasks.Actions{
    [Name("Roll")]
	[Category("CharacterController")]
	public class ControllerRoll : ActionTask<vThirdPersonController>{

		protected override string OnInit(){
			return null;
		}

		protected override void OnExecute(){
            agent.Roll();
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