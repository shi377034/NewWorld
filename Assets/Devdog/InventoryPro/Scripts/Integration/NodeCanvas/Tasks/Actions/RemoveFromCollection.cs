using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Devdog.InventoryPro;

namespace NodeCanvas.Tasks.InventoryPro {

	[Category("InventoryPro")]
	[Icon("InventoryPro", true)]
	public class RemoveFromCollection : ActionTask {

		[RequiredField]
		public BBParameter<InventoryItemBase> item;
		[RequiredField]
		public BBParameter<ItemCollectionBase> collection;

		protected override string info {
			get { return string.Format("Remove {0} from {1}", item, collection); }
		}

		protected override void OnExecute() {
			collection.value.RemoveItem(item.value);
			EndAction(true);
		}
	}
}