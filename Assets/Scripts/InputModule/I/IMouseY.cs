using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace InputSystems
{
    /// <summary>
    /// 鼠标移动输入Y轴
    /// </summary>
    public interface IMouseY : IEventSystemHandler
    {
        void MouseY(float value);
    }
}
