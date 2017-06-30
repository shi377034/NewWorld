using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace InputSystems.CharacterModule
{
    /// <summary>
    /// 跳跃输入
    /// </summary>
    public interface IJump : IEventSystemHandler
    {
        void Jump();
    }
}
