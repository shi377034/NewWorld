using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Devdog.InventoryPro;

namespace NodeCanvas.Tasks.InventoryPro {

	[Category("InventoryPro")]
	[Icon("InventoryPro", true)]
	public class OpenCloseTreasureChest : ActionTask {

		[RequiredField]
		public BBParameter<LootableObject> chest;
		public BBParameter<bool> open = true;

		protected override string info {
			get { return string.Format("Chest {0} open = {1}", chest, open); }
		}

		protected override void OnExecute() {
			if (open.value) {
				chest.value.trigger.Use();
			} else {
				chest.value.trigger.UnUse();
			}
			EndAction(true);
		}
	}
}