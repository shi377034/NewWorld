using UnityEngine;
using System.Collections;

public class vTriggerLadderAction : MonoBehaviour
{
    [Tooltip("Automatically execute the action without the need to press a Button")]
    public bool autoAction;
    [Tooltip("Message to display at the HUD Action Text")]
    public string enterMessage;    
    [Tooltip("Trigger an Animation - Use the exactly same name of the AnimationState you want to trigger")]
    public string enterAnimation;
    public Transform enterTarget;
    [Tooltip("Trigger an Animation - Use the exactly same name of the AnimationState you want to trigger")]
    public string exitAnimation;    
}
