using UnityEngine.EventSystems;
using InputSystems.CharacterModule;
using UnityEngine;

namespace InputSystems
{
    public class JumpNode : IEventAction
    {
        void IEventAction.Execute(GameObject gameObject)
        {
            if (vp_Input.GetButtonDown("Jump"))
            {
                ExecuteEvents.Execute<IJump>(gameObject, null, ((handler, eventData) => handler.Jump()));
            }
        }
    }
}
