using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Devdog.InventoryPro;

namespace NodeCanvas.Tasks.InventoryPro {

	[Category("InventoryPro")]
	[Icon("InventoryPro", true)]
	public class EnableDisableCraftingBlueprint : ActionTask {

		public BBParameter<int> blueprintID;
		public BBParameter<bool> enable;

		protected override string info {
			get { return string.Format("BlueprintID {0} learned = {1}", blueprintID, enable); }
		}

		protected override void OnExecute() {

			foreach (CraftingCategory category in ItemManager.database.craftingCategories) {
				foreach (CraftingBlueprint bp in category.blueprints) {
					if (bp.ID == (uint)blueprintID.value) {
						bp.playerLearnedBlueprint = enable.value;
						EndAction(true);
						return;
					}
				}
			}

			UnityEngine.Debug.LogWarning("Error, can't set blueprint with ID " + blueprintID.value);
			EndAction(false);
		}
	}
}