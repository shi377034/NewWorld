using UnityEngine.EventSystems;
using InputSystems.CharacterModule;
using UnityEngine;

namespace InputSystems
{
    public class RollNode : IEventAction
    {
        void IEventAction.Execute(GameObject gameObject)
        {
            if (vp_Input.GetButtonDown("Roll"))
            {
                ExecuteEvents.Execute<IRoll>(gameObject, null, ((handler, eventData) => handler.Roll()));
            }
        }
    }
}
