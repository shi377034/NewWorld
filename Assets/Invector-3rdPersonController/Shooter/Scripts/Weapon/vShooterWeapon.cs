using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Invector.EventSystems;
using UnityEngine.Events;
using Invector.ItemManager;

[System.Serializable]
public class vShooterWeapon : MonoBehaviour, vIEquipment
{

    #region variables

    [Header("Weapon Settings")]
    [Tooltip("Change between automatic weapon or shot once")]
    public bool automaticWeapon;
    [Tooltip("Frequency of shots")]
    public float shootFrequency;
    [Tooltip("Max clip size of your weapon")]
    public int clipSize;
    [Tooltip("Starting ammo")]
    [SerializeField]
    protected int ammo;
    [Tooltip("Ammo ID - make sure your AmmoManager and ItemListData use the same ID")]
    public int ammoID;
    [Tooltip("What moveset the underbody will play")]
    public float moveSetID;
    [Tooltip("What moveset the uperbody will play")]
    public float upperBodyID;
    [Tooltip("What shot animation will trigger")]
    public float attackID;
    [Tooltip("What reload animation will play")]
    public int reloadID;
    [Tooltip("What equip animation will play")]
    public int equipID;
    [Tooltip("IK will help the right hand to align where you actually is aiming")]
    public bool alignRightHandToAim = true;
    [Tooltip("IK will help the right hand to align where you actually is aiming")]
    public bool alignRightUpperArmToAim = true;
    public bool raycastAimTarget = true;
    [Tooltip("Left IK on Idle")]
    public bool useIkOnIdle = true;
    [Tooltip("Left IK on free locomotion")]
    public bool useIkOnFree = true;
    [Tooltip("Left IK on strafe locomotion")]
    public bool useIkOnStrafe = true;
    [Tooltip("Left IK while attacking")]
    public bool useIkAttacking = false;
    [Tooltip("Use this offset to better align your spine with the aim animation")]
    public Vector2 headTrackOffset;
    [Tooltip("Use this offset to better align your spine with the aim animation")]
    public Vector2 headTrackOffsetCrouch;
    [Tooltip("Left IK of the weapon")]
    public Transform handIKTarget;

    [Header("Projectile Settings")]
    [Tooltip("Prefab of the projectile")]
    public GameObject projectile;
    [Tooltip("Assign the projectile you want to hide, for example an RPG with a missile")]
    public GameObject projectileToHide;
    [Tooltip("Delay to active the hidden projectile to syinc with the reload animation")]
    public float projectileToHideDelay;
    [Tooltip("Assign the muzzle of your weapon")]
    public Transform muzzle;
    [Tooltip("Assign the aimReference of your weapon")]
    public Transform aimReference;
    [Tooltip("How many projectiles will spawn per shot")]
    [Range(1, 20)]
    public int projectilesPerShot = 1;
    [Range(0, 90)]
    [Tooltip("If you have more then 1 projectile, how much dispersion will they have")]
    public float dispersion = 2;
    [Range(0, 1000)]
    [Tooltip("Velocity of your projectile")]
    public float velocity = 380;
    [Tooltip("Min distance to apply damage")]
    public float DropOffStart = 8f;
    [Tooltip("Max distance to apply damage")]
    public float DropOffEnd = 50f;
    [Tooltip("Minimum damage caused by the shot, regardless the distance")]
    public int minDamage;
    [Tooltip("Maximum damage caused by the close shot")]
    public int maxDamage;
    [Tooltip("Creates a right recoil on the camera")]
    public float recoilRight = 1;
    [Tooltip("Creates a left recoil on the camera")]
    public float recoilLeft = -1;
    [Tooltip("Creates a up recoil on the camera")]
    public float recoilUp = 1;

    [Header("Audio")]
    public AudioClip fireClip;
    public AudioClip emptyClip;
    public AudioClip reloadClip;
    public AudioSource source;

    [Header("Effects")]
    public Light lightOnShot;
    [SerializeField]
    public ParticleEmitter[] emittLegacyParticle;
    public ParticleSystem[] emittShurykenParticle;

    [Header("Scope Options")]
    [Tooltip("Check this bool to use an UI image for the scope, ex: snipers")]
    public bool useUI;
    [Tooltip("You can create different Aim sprites and use for different weapons")]
    public int scopeID;
    [Tooltip("change the FOV of the scope view\n **The calc is default value (60)-scopeZoom**"),Range(-118,60)]
    public float scopeZoom =60;

    [Tooltip("assign an empty transform with the pos/rot of your scope view")]
    public Transform scopeTarget;
  
    public Camera zoomScopeCamera;
    protected Transform sender;

    [HideInInspector]
    public OnDestroyEvent onDestroy;
    [System.Serializable]
    public class OnDestroyEvent : UnityEngine.Events.UnityEvent<GameObject> { }
    [HideInInspector]
    public bool isAiming,usingScope;
    public UnityEvent onShot, onReload, onEmptyClip, onEnableAim, onDisableAim,onEnableScope,onDisableScope;
    [HideInInspector]
    public List<string> ignoreTags = new List<string>();
    [HideInInspector]
    public LayerMask hitLayer;
    [HideInInspector]
    public Transform root;
    vItem item;
    #endregion
    void StartEmitters()
    {
        if (emittLegacyParticle != null)
        {
            foreach (ParticleEmitter pe in emittLegacyParticle)
                pe.Emit();
        }

        if (emittShurykenParticle != null)
        {
            foreach (ParticleSystem pe in emittShurykenParticle)
                pe.Play();
        }
    }

    void StopEmitters()
    {
        if (emittLegacyParticle != null)
        {
            foreach (ParticleEmitter pe in emittLegacyParticle)
                pe.emit = false;
        }

        if (emittShurykenParticle != null)
        {
            foreach (ParticleSystem pe in emittShurykenParticle)
                pe.Stop();
        }
    }    
   
    public virtual string weaponName
    {
        get
        {
            var value = gameObject.name.Replace("(Clone)", string.Empty);
            return value;
        }
    }

    public void OnDestroy()
    {
        onDestroy.Invoke(gameObject);
    }

    public virtual void ShootEffect(Transform sender = null)
    {
        this.sender = sender != null ? sender : transform;
        HandleShotEffet(muzzle.position + muzzle.forward * 100f);
    }

    public virtual void ShootEffect(Vector3 aimPosition, Transform sender = null)
    {
        if (item)
        {
            var ammoAttribute = item.attributes.Find(a => a.name == Invector.ItemManager.vItemAttributes.AmmoCount);
            if (ammoAttribute != null)
            {
                ammoAttribute.value--;
            }
        }
        else
            ammo--;
        this.sender = sender != null ? sender : transform;
        HandleShotEffet(aimPosition);
    }

    public bool HasAmmo()
    {
        if (item)
        {
            var ammoAttribute = item.attributes.Find(a => a.name == Invector.ItemManager.vItemAttributes.AmmoCount);
            if (ammoAttribute != null)
            {
               return  ammoAttribute.value>0;
            }
        }
        else
            return ammo > 0;
        return false;
    }

    public int ammoCount
    {
        get
        {
            if (item)
            {
                var ammoAttribute = item.attributes.Find(a => a.name == Invector.ItemManager.vItemAttributes.AmmoCount);
                if (ammoAttribute != null)
                {
                    return ammoAttribute.value ;
                }
            }
            else
                return ammo ;
            return 0;
        }
    }

    public void AddAmmo(int value)
    {
        if (item)
        {
            var ammoAttribute = item.attributes.Find(a => a.name == Invector.ItemManager.vItemAttributes.AmmoCount);
            if (ammoAttribute != null)
            {
              ammoAttribute.value += value;
            }
        }
        else
             ammo +=value;
       
    }

    public virtual void ReloadEffect()
    {
        source.Stop();
        source.PlayOneShot(reloadClip);
        SendMessage("Reload", SendMessageOptions.DontRequireReceiver);
        if (projectileToHide != null)
            StartCoroutine(ActiveHiddenProjectile(projectileToHideDelay));
        onReload.Invoke();
    }

    public virtual void EmptyClipEffect()
    {
        source.Stop();
        source.PlayOneShot(emptyClip);
        onEmptyClip.Invoke();
    }

    public virtual void StopSound()
    {
        source.Stop();
    }

    protected virtual IEnumerator ActiveHiddenProjectile(float time)
    {
        yield return new WaitForSeconds(time);
        projectileToHide.gameObject.SetActive(true);
    }

    protected virtual IEnumerator LightOnShoot(float time)
    {
        lightOnShot.enabled = true;

        yield return new WaitForSeconds(time);
        lightOnShot.enabled = false;
    }

    protected virtual Vector3 Dispersion(Vector3 aim, float distance, float variance)
    {
        aim.Normalize();
        Vector3 v3 = Vector3.zero;
        do
        {
            v3 = Random.insideUnitSphere;
        }
        while (v3 == aim || v3 == -aim);
        v3 = Vector3.Cross(aim, v3);
        v3 = v3 * Random.Range(0f, variance);
        return aim * distance + v3;
    }

    protected virtual void HandleShotEffet(Vector3 aimPosition)
    {
        onShot.Invoke();
        StopCoroutine("LightOnShoot");
        source.Stop();
        source.PlayOneShot(fireClip);
        StartCoroutine("LightOnShoot", 0.037f);

        StartEmitters();

        if (projectileToHide != null)
            projectileToHide.gameObject.SetActive(false);
        var dir = aimPosition - muzzle.position;
        var rotation = Quaternion.LookRotation(dir);
        if (projectilesPerShot > 1)
        {
            for (int i = 0; i < projectilesPerShot; i++)
            {
                var spreadRotation = Quaternion.LookRotation(Dispersion(dir.normalized, DropOffEnd, dispersion));
                var obj = projectile.Instantiate(muzzle.transform.position, spreadRotation, 0);
                var pCtrl = obj.GetComponent<vProjectileControl>();
                pCtrl.root = root;
                pCtrl.ignoreTags = ignoreTags;
                pCtrl.hitLayer = hitLayer;
                pCtrl.damage.sender = sender;
                pCtrl.startPosition = obj.transform.position;
                pCtrl.maxDamage = maxDamage/projectilesPerShot;
                pCtrl.minDamage = minDamage/projectilesPerShot;
                pCtrl.DropOffStart = DropOffStart;
                pCtrl.DropOffEnd = DropOffEnd;

                StartCoroutine(ShootBullet(obj, spreadRotation * Vector3.forward));
            }
        }
        else if (projectilesPerShot > 0)
        {
            var obj = projectile.Instantiate(muzzle.transform.position, rotation,0);
            var pCtrl = obj.GetComponent<vProjectileControl>();
            pCtrl.root = root;
            pCtrl.ignoreTags = ignoreTags;
            pCtrl.hitLayer = hitLayer;
            pCtrl.damage.sender = sender;
            pCtrl.startPosition = obj.transform.position;
            pCtrl.maxDamage = maxDamage;
            pCtrl.minDamage = minDamage;
            pCtrl.DropOffStart = DropOffStart;
            pCtrl.DropOffEnd = DropOffEnd;

            StartCoroutine(ShootBullet(obj, dir));
        }
    }

    protected virtual IEnumerator ShootBullet(GameObject bullet, Vector3 dir)
    {
        yield return new WaitForSeconds(0.01f);
        try
        {
            var _rigidbody = bullet.GetComponent<Rigidbody>();
            _rigidbody.mass = _rigidbody.mass / projectilesPerShot;//Change mass per projectiles count.
            _rigidbody.AddForce(dir.normalized * velocity, ForceMode.VelocityChange);
        }
        catch
        {

        }
    }

    public void SetScopeZoom(float value)
    {
        if(zoomScopeCamera)
        {
            var zoom = Mathf.Clamp(61-value, 1, 179);
            zoomScopeCamera.fieldOfView = zoom;
        }
    }

    public void SetActiveAim(bool value)
    {
        if (isAiming != value)
        {
            isAiming = value;
            if (isAiming)
                onEnableAim.Invoke();
            else
                onDisableAim.Invoke();
        }
    }
    /// <summary>
    /// Set if Weapon is using scope
    /// </summary>
    /// <param name="value"></param>
    public void SetActiveScope(bool value)
    {
        if (usingScope != value)
        {
            usingScope = value;
            if (usingScope)
                onEnableScope.Invoke();
            else
                onDisableScope.Invoke();
        }
    }

    /// <summary>
    /// Set look target point to Zoom scope camera
    /// </summary>
    /// <param name="point"></param>
    public void SetScopeLookTarget(Vector3 point)
    {
        if (zoomScopeCamera) zoomScopeCamera.transform.LookAt(point);
    }
    public void OnEquip(vItem item)
    {
        SetScopeZoom(scopeZoom);
        this.item = item;
        var damageAttribute = item.GetItemAttribute("Damage");
        if (damageAttribute != null)
        {
            maxDamage = damageAttribute.value;
        }
    }

    public void OnUnequip(vItem item)
    {      
       
        this.item = null;       
    }
}