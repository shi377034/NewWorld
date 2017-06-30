using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InputSystems
{
    public class VerticalNode : IEventAction
    {
        void IEventAction.Execute(GameObject gameObject)
        {
            float value = vp_Input.GetAxisRaw("Vertical");
            ExecuteEvents.Execute<IVertical>(gameObject, null, ((handler, eventData) => handler.Vertical(value)));
        }
    }
}
