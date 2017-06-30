using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace InputSystems
{
    /// <summary>
    /// 鼠标移动输入X轴
    /// </summary>
    public interface IMouseX : IEventSystemHandler
    {
        void MouseX(float value);
    }
}
