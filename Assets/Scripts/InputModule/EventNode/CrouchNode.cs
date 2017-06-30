using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using InputSystems.CharacterModule;
namespace InputSystems
{
    public class CrouchNode : IEventAction
    {
        void IEventAction.Execute(GameObject gameObject)
        {
            if (vp_Input.GetButtonDown("Crouch"))
            {
                ExecuteEvents.Execute<ICrouch>(gameObject, null, ((handler, eventData) => handler.Crouch()));
            }
        }
    }
}
