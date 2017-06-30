using UnityEngine;
using System.Collections;
using Invector.CharacterController;

public class vTriggerAction : MonoBehaviour 
{
    [Header("Trigger Action Options")]
    [Tooltip("Automatically execute the action without the need to press a Button")]
    public bool autoAction;
    [Tooltip("Message to display at the HUD Action Text")]
    public string message;
    [Tooltip("Trigger an Animation - Use the exactly same name of the AnimationState you want to trigger")]
    public string playAnimation;
    [Tooltip("Use a transform to help the character climb any height, take a look at the Example Scene ClimbUp, StepUp, JumpOver objects.")]
    public Transform target;
    [Tooltip("Use this to limit the trigger to active if forward of character is close to this forward")]
    public bool activeFromForward;
    [Tooltip("Rotate Character for this rotation when active")]
    public bool useTriggerRotation;

    private Collider _collider;

    protected virtual void Start()
    {
        _collider = GetComponent<Collider>();
    }

    public virtual bool CanUse()
    {
        return _collider != null && _collider.enabled;
    }

    public virtual void DoAction(vThirdPersonController cc)
    {
        
    }
}
