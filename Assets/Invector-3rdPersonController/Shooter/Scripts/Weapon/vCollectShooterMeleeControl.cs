using UnityEngine;
using System.Collections;

public class vCollectShooterMeleeControl : vCollectMeleeControl
{
    protected vShooterManager shooterManager;

    protected override void Start()
    {
        base.Start();
        shooterManager = GetComponent<vShooterManager>();

    }

    public override void HandleCollectableInput(vCollectableStandalone collectableStandAlone)
    {
        if (shooterManager && collectableStandAlone != null && collectableStandAlone.weapon != null)
        {
            var weapon = collectableStandAlone.weapon.GetComponent<vShooterWeapon>();
            if (!weapon) return;

            var p = GetEquipPoint(rightHandler, collectableStandAlone.targetEquipPoint);
            if (!p) return;
            collectableStandAlone.weapon.transform.SetParent(p);
            collectableStandAlone.weapon.transform.localPosition = Vector3.zero;
            collectableStandAlone.weapon.transform.localEulerAngles = Vector3.zero;
            if (rightWeapon && rightWeapon != weapon.gameObject)
                RemoveRightWeapon();
            shooterManager.SetRightWeapon(weapon.gameObject);
            collectableStandAlone.OnEquip.Invoke();
            rightWeapon = weapon.gameObject;
            UpdateRightDisplay(collectableStandAlone);
        }

        base.HandleCollectableInput(collectableStandAlone);
    }

    protected override void RemoveRightWeapon()
    {
        base.RemoveRightWeapon();
        if (shooterManager)
        {
            shooterManager.rWeapon = null;
        }
    }
}
