using UnityEngine.EventSystems;

namespace InputSystems
{
    /// <summary>
    /// 垂直输入
    /// </summary>
    public interface IVertical : IEventSystemHandler
    {
        void Vertical(float value);
    }
}