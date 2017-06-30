using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Devdog.General;

namespace NodeCanvas.Tasks.InventoryPro {

	[Devdog.General.Category("InventoryPro")]
	[Icon("InventoryPro", true)]
	public class UseTrigger : ActionTask {

		[RequiredField]
		public BBParameter<Trigger> trigger;
		[RequiredField]
		public BBParameter<bool> use;

		protected override string info {
			get { return string.Format("Use {0} ({1})", trigger, use); }
		}

		protected override void OnExecute() {
			if (use.value) {
				trigger.value.Use();
			} else {
				trigger.value.UnUse();
			}
			EndAction();
		}
	}
}