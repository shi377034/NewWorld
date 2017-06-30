using UnityEngine;
using UnityEngine.EventSystems;

namespace InputSystems
{
    public class MouseXNode : IEventAction
    {
        void IEventAction.Execute(GameObject gameObject)
        {
            float value = vp_Input.GetAxisRaw("Mouse X");
            ExecuteEvents.Execute<IMouseX>(gameObject, null, ((handler, eventData) => handler.MouseX(value)));
        }
    }
}
