using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions{

	[Category("Photon Unity Networking")]
	public class CheckIsMine : ConditionTask<PhotonView>{

		protected override string OnInit(){
			return null;
		}

		protected override bool OnCheck(){
			return agent.isMine;
		}
	}
}