using UnityEngine;
using System.Collections;
using Invector.EventSystems;
using Invector;
using System;

public partial class vCollisionMessage : MonoBehaviour, vIMeleeFighter
{
    public float damageMultiplier = 1f;

    public void OnEnableAttack()
    {

    }

    public void OnDisableAttack()
    {

    }

    public void ResetAttackTriggers()
    {

    }

    public void BreakAttack(int breakAtkID)
    {

    }

    public void OnRecoil(int recoilID)
    {

    }

    public void OnReceiveAttack(Damage damage, vIMeleeFighter attacker)
    {
        if (!ragdoll) return;
        if (!ragdoll.iChar.isDead && ragdoll.gameObject.IsAMeleeFighter())
        {
            var _damage = new Damage(damage);
            var value = (float)_damage.value;
            _damage.value = (int)(value * damageMultiplier);            
            ragdoll.gameObject.GetMeleeFighter().OnReceiveAttack(_damage, attacker);
        }
    }

    public vCharacter Character()
    {
        return ragdoll.iChar;
    }
}
