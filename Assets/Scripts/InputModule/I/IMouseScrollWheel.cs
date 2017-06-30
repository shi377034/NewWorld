using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace InputSystems
{
    /// <summary>
    /// 鼠标滚轮输入
    /// </summary>
    public interface IMouseScrollWheel : IEventSystemHandler
    {
        void MouseScrollWheel(float value);
    }
}
