using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Devdog.InventoryPro;
using System.Linq;

namespace NodeCanvas.Tasks.InventoryPro {

	[Category("InventoryPro")]
	[Icon("InventoryPro", true)]
	public class AddToChest : ActionTask {

		[RequiredField]
		public BBParameter<InventoryItemBase> item;
		public BBParameter<int> amount = 1;
		[RequiredField]
		public BBParameter<LootableObject> chest;

		protected override string info {
			get { return string.Format("Add {0}x {1} to {2}", amount, item, chest); }
		}

		protected override void OnExecute() {
			var items = chest.value.items.ToList();
			for (int i = 0; i < amount.value; i++) {
				items.Add(item.value);
			}
			chest.value.items = items.ToArray();
			EndAction(true);
		}
	}
}