using UnityEngine.EventSystems;
using InputSystems.CharacterModule;
using UnityEngine;
using System;

namespace InputSystems
{
    public class SprintNode : IEventAction
    {
        void IEventAction.Execute(GameObject gameObject)
        {
            if (vp_Input.GetButtonDown("Sprint"))
            {
                ExecuteEvents.Execute<ISprint>(gameObject, null, ((handler, eventData) => handler.Sprint(true)));
            }else
            {
                ExecuteEvents.Execute<ISprint>(gameObject, null, ((handler, eventData) => handler.Sprint(false)));
            }
        }
    }
}
