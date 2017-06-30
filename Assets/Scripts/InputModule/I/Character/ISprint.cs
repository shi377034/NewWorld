using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace InputSystems.CharacterModule
{
    /// <summary>
    /// 冲刺
    /// </summary>
    public interface ISprint : IEventSystemHandler
    {
        void Sprint(bool value);
    }
}
