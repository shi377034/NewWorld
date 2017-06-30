using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
namespace InputSystems
{
    public class InputModule : MonoBehaviour
    {
        [HideInInspector]
        public List<IEventAction> ActionList = new List<IEventAction>();
        // Use this for initialization
        void Start()
        {
            ActionList.Add(new VerticalNode());
            ActionList.Add(new HorizontalNode());
            ActionList.Add(new MouseXNode());
            ActionList.Add(new MouseYNode());
            ActionList.Add(new MouseScrollWheelNode());
        }

        // Update is called once per frame
        void Update()
        {
            for(int i=0;i< ActionList.Count;i++)
            {
                ActionList[i].Execute(gameObject);
            }
        }
    }
}

