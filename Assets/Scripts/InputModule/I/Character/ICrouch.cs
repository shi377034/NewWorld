using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace InputSystems.CharacterModule
{
    /// <summary>
    /// 蹲伏
    /// </summary>
    public interface ICrouch : IEventSystemHandler
    {
        void Crouch();
    }
}
