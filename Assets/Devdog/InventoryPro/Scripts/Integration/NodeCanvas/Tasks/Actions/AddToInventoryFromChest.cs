using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Devdog.InventoryPro;

namespace NodeCanvas.Tasks.InventoryPro {

	[Category("InventoryPro")]
	[Icon("InventoryPro", true)]
	public class AddToInventoryFromChest : ActionTask {

		[RequiredField]
		public BBParameter<LootableObject> chest;

		protected override string info {
			get { return string.Format("Add from {0} to Inventory", chest); }
		}

		protected override void OnExecute() {
			var c = chest.value;
			for (var i = c.items.Length - 1; i > -1; --i) {
				InventoryManager.AddItem(c.items[i]);
				c.items[i] = null;
			}
			EndAction(true);
		}
	}
}