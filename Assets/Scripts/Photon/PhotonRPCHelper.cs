using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PhotonView))]
[DisallowMultipleComponent]
public partial class PhotonRPCHelper : Photon.MonoBehaviour {
    private static PhotonRPCHelper _instanc;
    public static PhotonRPCHelper Instance
    {
        get {
            if(_instanc==null)
            {
                GameObject go = GameObject.Find("PhotonRPC");
                if(go==null)
                {
                    go = new GameObject("PhotonRPC");
                }
                DontDestroyOnLoad(go);
                _instanc = go.GetComponent<PhotonRPCHelper>();
                if(_instanc == null)
                {
                    _instanc = go.AddComponent<PhotonRPCHelper>();
                }
            }
            return _instanc; }
    }
    public void RPC(string methodName, PhotonTargets target, params object[] parameters)
    {
        photonView.RPC(methodName, target, parameters);
    }
}
