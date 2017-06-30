using UnityEngine;
using UniRx;
using System;
public class vInstantiateStepMark : MonoBehaviour
{
    public GameObject stepMark;
    public LayerMask stepLayer;
    public float timeToDestroy = 5f;

    void StepMark(FootStepObject footStep)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), -footStep.sender.up, out hit, 1f, stepLayer))
        {
            var angle = Quaternion.FromToRotation(footStep.sender.up, hit.normal);
            if (stepMark != null)
            {
                var step = stepMark.Instantiate(hit.point, angle * footStep.sender.rotation,0) as GameObject;
                //if (footStep.ground != null)
                //    step.transform.SetParent(footStep.ground);
                Observable.Timer(TimeSpan.FromSeconds(timeToDestroy)).Subscribe(x =>
                {
                    if(step != null)
                    {
                        PhotonNetwork.Destroy(step);
                    }                   
                });
                //Destroy(gameObject, timeToDestroy);
            }
            else
                Observable.Timer(TimeSpan.FromSeconds(timeToDestroy)).Subscribe(x =>
                {
                    if(gameObject != null)
                    {
                        PhotonNetwork.Destroy(gameObject);
                    }                
                });
        }
    }
}
