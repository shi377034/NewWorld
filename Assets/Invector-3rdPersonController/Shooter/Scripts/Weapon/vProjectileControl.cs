using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Invector;
using Invector.EventSystems;

public class vProjectileControl : vObjectDamage
{
    public string tagToCast = "Enemy";
    [HideInInspector]
    public int minDamage;
    [HideInInspector]
    public int maxDamage;
    [HideInInspector]
    public float DropOffStart = 8f;
    [HideInInspector]
    public float velocity = 580;
    [HideInInspector]
    public float DropOffEnd = 50f;
    [HideInInspector]
    public Vector3 startPosition;
    public float forceMultiplier = 1;
    public bool destroyOnCollision = true;
    public bool ignoreSelfDamage;
    private bool firstDamage;
    public ProjectilePassDamage onPassDamage;
    public ProjectileRayCastEvent onRayCast;
  
    [Header("RayCast Options")]
    public LayerMask hitLayer = -1; //make sure we aren't in this layer 
    public float skinWidth = 0.1f; //probably doesn't need to be changed 

    private float minimumExtent;
    private float partialExtent;
    private float sqrMinimumExtent;
    private Vector3 previousPosition;
    private Rigidbody myRigidbody;
    private Collider myCollider;
    [HideInInspector]
    public List<string> ignoreTags;
    [HideInInspector]
    public Transform root;
    
    protected override void Start()
    {
        base.Start();
        myRigidbody = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        previousPosition = myRigidbody.position;
        minimumExtent = Mathf.Min(Mathf.Min(myCollider.bounds.extents.x, myCollider.bounds.extents.y), myCollider.bounds.extents.z);
        partialExtent = minimumExtent * (1.0f - skinWidth);
        sqrMinimumExtent = minimumExtent * minimumExtent;
    }

    protected override void Update()
    {
        //have we moved more than our minimum extent? 
        Vector3 movementThisStep = myRigidbody.position - previousPosition;
        float movementSqrMagnitude = movementThisStep.sqrMagnitude;

        if (movementSqrMagnitude > sqrMinimumExtent)
        {
            float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
            RaycastHit hitInfo;

            //check for obstructions we might have missed 
            if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude, hitLayer))
            {                
                if (!hitInfo.collider)
                    return;
                if (ignoreTags.Contains(hitInfo.collider.gameObject.tag) || (root!=null && root.IsChildOf(hitInfo.collider.transform))) return;
                var _damage = new Damage(damage);
                _damage.value = maxDamage;
                onPassDamage.Invoke(_damage);
                onRayCast.Invoke(hitInfo);
                
                if (!hitInfo.collider.isTrigger)
                {
                    //Debug.Log(hitInfo.collider.name, hitInfo.collider);
                    myRigidbody.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent;
                    if(!ignoreSelfDamage)
                    {
                        var dist = Vector3.Distance(startPosition, myRigidbody.position);
                        var result = 0f;
                        var damageDifence = maxDamage - minDamage;

                        if (dist - DropOffStart >= 0)
                        {
                            int percentComplete = (int)Math.Round((double)(100 * (dist - DropOffStart)) / (DropOffEnd - DropOffStart));
                            result = Mathf.Clamp(percentComplete * 0.01f, 0, 1f);
                            damage.value = maxDamage - (int)(damageDifence * result);
                            //Debug.Log(damage.value + " " + percentComplete + " " + dist);
                        }
                        else
                            damage.value = maxDamage;

                        // new stuff
                        damage.hitPosition = hitInfo.point;
                        damage.receiver = hitInfo.collider.transform;
                        if (hitInfo.collider.gameObject.IsAMeleeFighter())
                        {
                            hitInfo.collider.gameObject.GetMeleeFighter().OnReceiveAttack(damage, null);
                        }
                        else
                        {
                            hitInfo.collider.gameObject.ApplyDamage(damage);
                        }
                    }
                   
                }
                var rigb = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
                if (rigb && !hitInfo.collider.gameObject.isStatic)
                {
                    rigb.AddForce(transform.forward * damage.value * forceMultiplier, ForceMode.Impulse);
                }
                
               
                if (destroyOnCollision)
                    PhotonNetwork.Destroy(gameObject);
            }
        }
        previousPosition = myRigidbody.position;
    }

}

[System.Serializable]
public class ProjectileRayCastEvent : UnityEngine.Events.UnityEvent<RaycastHit> { }
[System.Serializable]
public class ProjectilePassDamage : UnityEngine.Events.UnityEvent<Damage> { }
