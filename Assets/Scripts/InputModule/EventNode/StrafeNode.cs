using UnityEngine.EventSystems;
using InputSystems.CharacterModule;
using UnityEngine;
using System;

namespace InputSystems
{
    public class StrafeNode : IEventAction
    {
        void IEventAction.Execute(GameObject gameObject)
        {
            if (vp_Input.GetButtonDown("Strafe"))
            {
                ExecuteEvents.Execute<IStrafe>(gameObject, null, ((handler, eventData) => handler.Strafe()));
            }
        }
    }
}
