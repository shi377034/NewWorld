using Invector.CharacterController;
using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions{
    [Name("Input")]
	[Category("CharacterController")]
	public class ControllerInput : ActionTask<vThirdPersonController>{
        public BBParameter<float> x;
        public BBParameter<float> y;
        protected override string info
        {
            get
            {
                return base.info+"\n("+x+","+y+")";
            }
        }
        protected override void OnUpdate(){
            agent.input.x = x.value;
            agent.input.y = y.value;
            EndAction(true);
        }
    }
}