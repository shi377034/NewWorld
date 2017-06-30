using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using InputSystems.CharacterModule;
namespace InputSystems
{
    /// <summary>
    /// 互动
    /// </summary>
    public class InteractNode : IEventAction
    {
        void IEventAction.Execute(GameObject gameObject)
        {
            if(vp_Input.GetButtonDown("Interact"))
            {
                ExecuteEvents.Execute<IInteract>(gameObject, null, ((handler, eventData) => handler.Interact()));
            }
        }
    }
}
