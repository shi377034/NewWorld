using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Invector.EventSystems;
using System;

public class vBarrel : MonoBehaviour, vIDamageReceiver
{
    public float health=10;
    public Transform referenceTransformUP;
    public float maxAngleUp = 90;
    protected bool isBarrelRoll;
    public UnityEngine.Events.UnityEvent onDead;
    public UnityEngine.Events.UnityEvent onBarrelRoll;
    public List<string> acceptableAttacks = new List<string>() { "explosion", "projectile" };
   
    void OnCollisionEnter()
    {
       
        if (!referenceTransformUP) return;
        var angle = Vector3.Angle(referenceTransformUP.up, Vector3.up);
       
        if (angle> maxAngleUp && !isBarrelRoll)
        {
            isBarrelRoll = true;
            onBarrelRoll.Invoke();
        }     
    }
    public void TakeDamage(Damage damage, bool hitReaction = true)
    {
        if(acceptableAttacks.Contains(damage.attackName))
        {
            if (health > 0)
                health -= damage.value;
            if (health <= 0)
            {
                onDead.Invoke();
            }
        }
    }    
}
