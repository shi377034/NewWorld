using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InputSystems
{
    public class MouseScrollWheelNode : IEventAction
    {
        void IEventAction.Execute(GameObject gameObject)
        {
            float value = vp_Input.GetAxisRaw("Mouse ScrollWheel");
            ExecuteEvents.Execute<IMouseScrollWheel>(gameObject, null, ((handler, eventData) => handler.MouseScrollWheel(value)));
        }
    }
}
