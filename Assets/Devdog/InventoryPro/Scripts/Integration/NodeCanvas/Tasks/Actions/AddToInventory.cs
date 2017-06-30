using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Devdog.InventoryPro;

namespace NodeCanvas.Tasks.InventoryPro {

	[Category("InventoryPro")]
	[Icon("InventoryPro", true)]
	public class AddToInventory : ActionTask {

		[RequiredField]
		public BBParameter<InventoryItemBase> item;
		public BBParameter<int> amount = 1;

		protected override string info {
			get { return string.Format("Add {0}x {1} to Inventory", amount, item); }
		}

		protected override void OnExecute() {
			item.value.currentStackSize = (uint)amount.value;
			InventoryManager.AddItem(item.value);
			EndAction(true);
		}
	}
}