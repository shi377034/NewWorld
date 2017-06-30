using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MenuEditor
{
    [MenuItem("Input/Input Manager")]
    public static void InputManager()
    {
        vp_InputWindow.Init();
    }
}
