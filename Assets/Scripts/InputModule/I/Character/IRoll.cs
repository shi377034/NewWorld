using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace InputSystems.CharacterModule
{
    /// <summary>
    /// 翻滚
    /// </summary>
    public interface IRoll : IEventSystemHandler
    {
        void Roll();
    }
}
