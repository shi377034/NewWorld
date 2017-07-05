using UnityEngine;
using System.Collections;
using Invector.ItemManager;
using System.Collections.Generic;
using System;
using UnityEditor.Events;

public partial class vItemManagerUtilities
{
    partial void _InitItemManager(vItemManager itemManager)
    {
        CreateShooterPoints(itemManager, itemManager.GetComponent<vShooterManager>());
    }
    static void CreateShooterPoints(vItemManager itemManager, vShooterManager shooterManager)
    {
        if (!shooterManager) return;
        #region RightEquipPoint
        var animator = itemManager.GetComponent<Animator>();
        var equipPointR = itemManager.equipPoints.Find(p => p.equipPointName == "RightArm");
        if (equipPointR == null)
        {
            EquipPoint pointR = new EquipPoint();
            pointR.equipPointName = "RightArm";
            if (shooterManager)
            {
#if UNITY_EDITOR
                UnityEventTools.AddPersistentListener<GameObject>(pointR.onInstantiateEquiment, shooterManager.SetRightWeapon);
#else
                    pointR.onInstantiateEquiment.AddListener(manager.SetRightWeapon);
#endif
            }

            if (animator)
            {
                var defaultEquipPointR = new GameObject("defaultEquipPoint");
                var parent = animator.GetBoneTransform(HumanBodyBones.RightHand);
                defaultEquipPointR.transform.SetParent(parent);
                defaultEquipPointR.transform.localPosition = Vector3.zero;
                defaultEquipPointR.transform.forward = itemManager.transform.forward;
                defaultEquipPointR.gameObject.tag = "Ignore Ragdoll";
                pointR.handler = new vHandler();
                pointR.handler.defaultHandler = defaultEquipPointR.transform;
            }
            itemManager.equipPoints.Add(pointR);
        }
        else
        {
            if (equipPointR.handler.defaultHandler == null)
            {
                if (animator)
                {
                    var parent = animator.GetBoneTransform(HumanBodyBones.RightHand);
                    var defaultPoint = parent.Find("defaultEquipPoint");
                    if (defaultPoint) equipPointR.handler.defaultHandler = defaultPoint;
                    else
                    {
                        var _defaultPoint = new GameObject("defaultEquipPoint");
                        _defaultPoint.transform.SetParent(parent);
                        _defaultPoint.transform.localPosition = Vector3.zero;
                        _defaultPoint.transform.forward = itemManager.transform.forward;
                        _defaultPoint.gameObject.tag = "Ignore Ragdoll";
                        equipPointR.handler.defaultHandler = _defaultPoint.transform;
                    }
                }
            }

            bool containsListener = false;
            for (int i = 0; i < equipPointR.onInstantiateEquiment.GetPersistentEventCount(); i++)
            {
                if (equipPointR.onInstantiateEquiment.GetPersistentTarget(i).GetType().Equals(typeof(vShooterManager)) && equipPointR.onInstantiateEquiment.GetPersistentMethodName(i).Equals("SetRightWeapon"))
                {
                    containsListener = true;
                    break;
                }
            }

            if (!containsListener && shooterManager)
            {
#if UNITY_EDITOR
                UnityEventTools.AddPersistentListener<GameObject>(equipPointR.onInstantiateEquiment, shooterManager.SetRightWeapon);
#else
                    equipPointR.onInstantiateEquiment.AddListener(manager.SetRightWeapon);
#endif
            }
        }
        #endregion
    }
}
