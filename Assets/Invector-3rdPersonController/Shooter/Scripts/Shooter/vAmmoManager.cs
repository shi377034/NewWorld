using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Invector.ItemManager;
using System;
using Invector;

public class vAmmoManager : MonoBehaviour, vIAmmoManager
{    
    public vAmmoListData ammoListData;
    
    [HideInInspector]
    public vShooterManager shooterManager;
    [HideInInspector]
    public vItemManager itemManager;
    [HideInInspector]
    public List<vItem> ammoItems;
    [HideInInspector]
    public List<Ammo> ammos = new List<Ammo>();

    void Start()
    {       
	    shooterManager = GetComponent<vShooterManager>();	    
        if (shooterManager)
        {
            shooterManager.onReloadWeapon.AddListener(ReloadWeapon);
            shooterManager.totalAmmoHandler = new vShooterManager.TotalAmmoHandler(TotalAmmo);
        }
	    
        itemManager = GetComponent<vItemManager>();
        if (itemManager)
        {
            itemManager.onUseItem.AddListener(UseAmmo);
            itemManager.onAddItem.AddListener(AddAmmo);
            itemManager.onDropItem.AddListener(DropAmmo);
            itemManager.onLeaveItem.AddListener(LeaveAmmo);
            ammoItems = itemManager.items.FindAll(item => item.type == vItemType.Ammo);
        }

       if (ammoListData)
        {
            ammos.Clear();
            for(int i = 0; i < ammoListData.ammos.Count; i++)
            {
                var ammo = new Ammo(ammoListData.ammos[i]);
                ammos.Add(ammo);
            }            
        }            
    }

    public bool CanReload(vShooterWeapon weapon)
    {
        throw new NotImplementedException();
    }

    public void ReloadWeapon(vShooterWeapon weapon)
    {
        if (weapon.ammoCount >= weapon.clipSize) return;
        var ammoItem = ammoItems.FindAll(item => item.id == weapon.ammoID);
        for (int i = 0; i < ammoItem.Count; i++)
        {
            if (weapon.ammoCount >= weapon.clipSize) break;
            var needAmmo = weapon.clipSize - weapon.ammoCount;
            if (ammoItem[i].amount < needAmmo)
                needAmmo = ammoItem[i].amount;

            weapon.AddAmmo(needAmmo);
            if (itemManager)
            {
                itemManager.LeaveItem(ammoItem[i], needAmmo);
            }
        }
    }

    public void UseAmmo(vItem item)
    {
        if (shooterManager && item.type == vItemType.Ammo)
        {
            var startingAmmo = ammos.Find(a => a.ammoID == item.id);
            if (startingAmmo != null)
            {
                var ammoAttribute = item.attributes.Find(a => a.name == Invector.ItemManager.vItemAttributes.AmmoCount);
                if (ammoAttribute != null)
                {
                    startingAmmo.count += ammoAttribute.value;
                    ammoAttribute.value = 0;
                }
                else
                {
                    startingAmmo.count++;
                }
            }
        }
    }

    public void AddAmmo(vItem item)
    {
        if (item.type == vItemType.Ammo)
        {
            ammoItems.Add(item);           
            UpdateTotalAmmo();
        }
    }

    public void LeaveAmmo(vItem item, int amount)
    {
        if (item.type == vItemType.Ammo)
        {
            if ((item.amount - amount) <= 0 && ammoItems.Contains(item))
                ammoItems.Remove(item);
        }
    }

    public void DropAmmo(vItem item, int amount)
    {
        if (item.type == vItemType.Ammo)
        {
            if ((item.amount - amount) <= 0 && ammoItems.Contains(item))
                ammoItems.Remove(item);
        }
    }

    public void TotalAmmo(int ammoID, ref int startingAmmo)
    {
        var ammoItem = ammoItems.FindAll(item => item.id == ammoID);
        for (int i = 0; i < ammoItem.Count; i++)
        {
            startingAmmo += ammoItem[i].amount;
        }
    }

    public void UpdateTotalAmmo()
    {
        if (shooterManager.rWeapon != null)
            shooterManager.UpdateTotalAmmo();
    }
   
}


