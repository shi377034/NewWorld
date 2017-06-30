using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Invector;
using UnityEngine.Events;
using System;

namespace Invector.ItemManager
{
    public class vItemCollection : vTriggerAction
    {
        [Header("Weapon Options WITHOUT Inventory")]
        [Tooltip("If you're not using an inventory, you can quickly equip the collectable")]
        public bool autoEquip;
        [Tooltip("If you're not using an inventory, you can drop the current equipped weapon")]
        public bool dropCurrentEquip;
        [Tooltip("If you're not using an inventory, you can equip this into a EquipPoint inside your character")]
        public string targetEquipPoint;
        [Tooltip("If you're not using an inventory, assign the actual weapon to equip")]
        public GameObject Weapon;

        [Header("ItemList Data to use WITH Inventory")]
        public vItemListData itemListData;

        [SerializeField]
        private OnCollectItems onCollectItems;       

        [Header("---Items Filter---")]      
        public List<vItemType> itemsFilter = new List<vItemType>() { 0 };

        [HideInInspector]
        public List<ItemReference> items = new List<ItemReference>();
        public bool destroyOnCollect = true;
        public float onCollectDelay;

        public void OnCollectItems(GameObject target)
        {
            if (items.Count > 0)
            {
                items.Clear();
                StartCoroutine(OnCollect(target));
            }
        }

        IEnumerator OnCollect(GameObject target)
        {
            yield return new WaitForSeconds(onCollectDelay);

            onCollectItems.Invoke(target);
            if (destroyOnCollect) PhotonNetwork.Destroy(this.gameObject);
        }
    }
}

