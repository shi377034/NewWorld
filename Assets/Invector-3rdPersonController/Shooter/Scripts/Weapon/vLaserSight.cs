using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class vLaserSight : MonoBehaviour
{
    public LayerMask layerMask;
    public GameObject aimSprite;
    public float aimSpriteOffset;
    public float maxDistance;

    Ray ray;
    RaycastHit hit;
    LineRenderer line;
    void Start ()
    {
        line = GetComponent<LineRenderer>();
        ray = new Ray();        
    }
	
	void LateUpdate ()
    {
        ray.origin = transform.position;
        ray.direction = transform.forward;
        var laserLenght = Vector3.zero;

        if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
        {            
            laserLenght.z = hit.distance - aimSpriteOffset;
            line.SetPosition(1, laserLenght);
            aimSprite.transform.rotation = Quaternion.LookRotation(hit.normal);
        }
        else
        {            
            laserLenght.z = Vector3.Distance(transform.position, ray.GetPoint(maxDistance - aimSpriteOffset));
            line.SetPosition(1, laserLenght);
            aimSprite.transform.localEulerAngles = Vector3.zero;
        }

        aimSprite.transform.localPosition = laserLenght;
	}
}
