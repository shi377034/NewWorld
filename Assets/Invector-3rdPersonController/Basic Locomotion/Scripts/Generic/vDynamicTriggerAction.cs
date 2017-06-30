using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class vDynamicTriggerAction : vTriggerAction
{
    [HideInInspector]
    public vBoxTrigger[] boxTriggers;
    public Transform rootTransform;

    protected override void Start()
    {
        if (rootTransform)
        {
            var colliders = rootTransform.GetComponentsInChildren<Collider>();
            for(int i =0;i<colliders.Length; i++)
            {
                var coll = colliders[i];
                for(int a=0;a<boxTriggers.Length;a++)
                {
                    var triggerColl = boxTriggers[a].GetComponent<Collider>();
                    if (triggerColl != coll)
                    {
                        Physics.IgnoreCollision(triggerColl, coll);
                    }
                }
            }           
        }
    }

    /// <summary>
    /// Check if CanUse this Trigger
    /// Work with trigger box cast
    /// </summary>
    /// <returns></returns>
    public override bool CanUse()
    {
        for (int i = 0; i < boxTriggers.Length; i++)
        {
            if (BoxCast(boxTriggers[i]))
                return false;
        }

        return true;
    }

    bool BoxCast(vBoxTrigger boxCast)
    {     
        return boxCast.inCollision;
    }
}
