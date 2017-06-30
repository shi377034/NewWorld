using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class vHitEffects : MonoBehaviour
{
    public GameObject audioSource;
    public AudioClip[] hitSounds;
    public AudioClip[] recoilSounds;
    public GameObject[] recoilParticles;
    public AudioClip[] defSounds;

    void Start()
    {
	    var weaponObject = GetComponent<vMeleeWeapon>();
        if (weaponObject)
        {
            weaponObject.onDamageHit.AddListener(PlayHitEffects);
            weaponObject.onRecoilHit.AddListener(PlayRecoilEffects);
            weaponObject.onDefense.AddListener(PlayDefenseEffects);
        }
    }

    public void PlayHitEffects(HitInfo hitInfo)
    {
        if (audioSource != null && hitSounds.Length > 0)
        {
            var clip = hitSounds[UnityEngine.Random.Range(0, hitSounds.Length)];
            var audioObj = audioSource.Instantiate(transform.position, transform.rotation,0) as GameObject;
            audioObj.GetComponent<AudioSource>().PlayOneShot(clip);
        }
    }

    public void PlayRecoilEffects(HitInfo hitInfo)
    {
        if (audioSource != null && recoilSounds.Length > 0)
        {
            var clip = recoilSounds[UnityEngine.Random.Range(0, recoilSounds.Length)];
            var audioObj = audioSource.Instantiate(transform.position, transform.rotation,0) as GameObject;
            audioObj.GetComponent<AudioSource>().PlayOneShot(clip);
        }
        if (recoilParticles.Length > 0)
        {
            var particles = recoilParticles[UnityEngine.Random.Range(0, recoilParticles.Length)];
            var hitrotation = Quaternion.LookRotation(new Vector3(transform.position.x, hitInfo.hitPoint.y, transform.position.z) - hitInfo.hitPoint);
            if (particles != null)
                particles.Instantiate(hitInfo.hitPoint, hitrotation,0);
        }
    }

    public void PlayDefenseEffects()
    {
        if (audioSource != null && defSounds.Length > 0)
        {
            var clip = defSounds[UnityEngine.Random.Range(0, defSounds.Length)];
            var audioObj = audioSource.Instantiate(transform.position, transform.rotation,0) as GameObject;
            audioObj.GetComponent<AudioSource>().PlayOneShot(clip);
        }
    }
}
