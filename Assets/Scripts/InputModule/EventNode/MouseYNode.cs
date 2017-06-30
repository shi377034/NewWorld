using System;
using UnityEngine;
using UnityEngine.EventSystems;
namespace InputSystems
{
    public class MouseYNode : IEventAction
    {
        void IEventAction.Execute(GameObject gameObject)
        {
            float value = vp_Input.GetAxisRaw("Mouse Y");
            ExecuteEvents.Execute<IMouseY>(gameObject, null, ((handler, eventData) => handler.MouseY(value)));
        }
    }
}
