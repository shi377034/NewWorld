using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Devdog.InventoryPro;

namespace NodeCanvas.Tasks.InventoryPro {

	[Category("InventoryPro")]
	[Icon("InventoryPro", true)]
	public class PickItem : ActionTask {

		[RequiredField]
		public BBParameter<InventoryItemBase> item;

		protected override string info {
			get { return "Pick " + item; }
		}

		protected override void OnExecute() {
			item.value.PickupItem();
			EndAction(true);
		}
	}
}