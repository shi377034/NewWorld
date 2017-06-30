using Invector.CharacterController;
using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions{
    [Name("Jump")]
	[Category("CharacterController")]
	public class ControllerJump : ActionTask<vThirdPersonController>{

        
		protected override string OnInit(){
			return null;
		}

		protected override void OnExecute(){
            agent.Jump();
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