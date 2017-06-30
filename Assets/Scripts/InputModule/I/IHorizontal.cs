using UnityEngine.EventSystems;

namespace InputSystems
{
    /// <summary>
    /// 水平输入
    /// </summary>
    public interface IHorizontal : IEventSystemHandler
    {
        void Horizontal(float value);
    }

}
