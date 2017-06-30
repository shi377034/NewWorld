using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Invector.CharacterController;

namespace NodeCanvas.Tasks.Actions{
    [Name("Sprint")]
	[Category("CharacterController")]
	public class ControllerSprint : ActionTask<vThirdPersonController>{
        public BBParameter<bool> value;
		protected override string OnInit(){
			return null;
		}

		protected override void OnExecute(){
            agent.Sprint(value.value);
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