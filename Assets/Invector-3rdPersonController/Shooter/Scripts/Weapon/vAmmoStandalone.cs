using UnityEngine;
using System.Collections;
using Invector.ItemManager;
using Invector.CharacterController;

public class vAmmoStandalone : vTriggerAction
{
    [Header("Ammo Standalone Options")]
    [Tooltip("Use the same name as in the AmmoManager")]
    public string weaponName;
    public int ammoAmount;
    public bool destroyAfterUse = true;
    private vAmmoManager ammoManager;

    public override void DoAction(vThirdPersonController cc)
    {
        base.DoAction(cc);
        ammoManager = cc.gameObject.GetComponent<vAmmoManager>();

        var ammo = ammoManager.ammos.Find(_ammo => _ammo.weaponName.Equals(weaponName));
        ammo.count += ammoAmount;
        ammoManager.UpdateTotalAmmo();

        if (destroyAfterUse) PhotonNetwork.Destroy(gameObject);
    }
}
