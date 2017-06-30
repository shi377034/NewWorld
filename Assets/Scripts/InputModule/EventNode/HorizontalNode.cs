using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
namespace InputSystems
{
    public class HorizontalNode : IEventAction
    {
        void IEventAction.Execute(GameObject gameObject)
        {
            float value = vp_Input.GetAxisRaw("Horizontal");
            ExecuteEvents.Execute<IHorizontal>(gameObject, null, ((handler, eventData) => handler.Horizontal(value)));
        }
    }
}
