using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace InputSystems.CharacterModule
{
    /// <summary>
    /// 互动(例如和场景里3D物体互动输入)
    /// </summary>
    public interface IInteract : IEventSystemHandler
    {
        void Interact();
    }
}
