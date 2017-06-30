using UnityEngine;
using System.Collections;
/// <summary>
/// GameObject扩展类
/// </summary>
public static class GameObjectExtension
{
    /// <summary>
    /// PhotonNetwork.Instantiate
    /// </summary>
    /// <param name="source"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="group"></param>
    /// <returns></returns>
    public static GameObject Instantiate(this GameObject source, Vector3 position, Quaternion rotation, int group)
    {
        GameObject go = null;
        PhotonNetwork.PrefabCache.TryGetValue(source.name, out go);
        if (go == null)
        {
            PhotonNetwork.PrefabCache.Add(source.name, source);
        }
        return PhotonNetwork.Instantiate(source.name, position, rotation, group);
    }
    /// <summary>
    /// PhotonNetwork.Instantiate
    /// </summary>
    /// <param name="source"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="group"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static GameObject Instantiate(this GameObject source,Vector3 position, Quaternion rotation, int group, object[] data)
    {
        GameObject go = null;
        PhotonNetwork.PrefabCache.TryGetValue(source.name, out go);
        if(go==null)
        {
            PhotonNetwork.PrefabCache.Add(source.name, source);
        }
        return PhotonNetwork.Instantiate(source.name, position, rotation, group, data);
    }
}
