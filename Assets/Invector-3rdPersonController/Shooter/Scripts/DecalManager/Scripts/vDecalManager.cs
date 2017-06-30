using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class vDecalManager : MonoBehaviour 
{
	public LayerMask layermask;
	public List<DecalObject> decalObjects;
	public virtual void CreateDecal(RaycastHit hitInfo)
    {
        CreateDecal(hitInfo.collider.gameObject, hitInfo.point, hitInfo.normal);
    }
	public virtual void CreateDecal(GameObject target,Vector3 position,Vector3 normal)
	{
		if (layermask == (layermask | (1 << target.layer)))
		{
			var decalObj = decalObjects.Find(d => d.tag.Equals(target.tag));
			if(decalObj != null)
			{
                RaycastHit hit;
                if(Physics.SphereCast(new Ray(position + (normal * 0.1f), -normal),0.0001f,out hit,1f, layermask))
                {
                    if(hit.collider.gameObject == target)
                    {
                        var rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
                        // instantiate hit effect particle
                        if (decalObj.hitEffect != null)
                        {
                            decalObj.hitEffect.Instantiate(hit.point, rotation,0);
                        }

                        // instantiate decal based on the gameobject tag
                        var decal = decalObj.GetDecal();
                        if (decal != null)
                        {
                            var _decal = decal.Instantiate(hit.point, rotation,0) as GameObject;
                            _decal.transform.SetParent(target.gameObject.transform);
                        }
                    }
                   
                }
				
			}
		}
	}
	
	[System.Serializable]
	public class DecalObject
	{
		public string tag;
		public GameObject hitEffect;
		public List<GameObject> decals;
		
		public GameObject GetDecal()
		{
			if(decals.Count > 1)
			{
				var index = Random.Range(0, decals.Count -1);
				return decals[index];
			}
			else if(decals.Count == 1)
				return decals[0];
			else 
				return null;
		}
	}
}


