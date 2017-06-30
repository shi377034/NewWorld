using UnityEngine;
using System.Collections;
using UnityEditor;
using Invector;

[CanEditMultipleObjects]
[CustomEditor(typeof(vHitDamageParticle), true)]
public class vHitDamageParticleEditor : Editor
{
    GUISkin skin;

    [MenuItem("Invector/Basic Locomotion/Components/HitDamageParticle")]
    static void MenuComponent()
    {
        if (Selection.activeGameObject)
            Selection.activeGameObject.AddComponent<vHitDamageParticle>();
        else
            Debug.Log("Please select a GameObject to add the component.");
    }

    public override void OnInspectorGUI()
    {
        if (!skin) skin = Resources.Load("skin") as GUISkin;
        GUI.skin = skin;        
        GUILayout.BeginVertical("HitDamage Particle", "window");

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("Default hit Particle to instantiate every time you receive damage", MessageType.Info);

        base.OnInspectorGUI();

        EditorGUILayout.HelpBox("Custom hit Particle to instantiate based on a custom AttackName from a Attack Animation State", MessageType.Info);

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
    }
}
