using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace InputSystems.CharacterModule
{
    /// <summary>
    /// 取消
    /// </summary>
    public interface ICancel : IEventSystemHandler
    {
        void Cancel();
    }
}
