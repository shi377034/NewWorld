using UnityEngine;
using UnityEditor;
using System.Collections;
using Invector.CharacterController;
using System.Reflection;

[CustomEditor(typeof(vThirdPersonMotor), true)]
public class vCharacterEditor : Editor
{
    GUISkin skin;
    SerializedObject character;
    bool showWindow;

    void OnEnable()
    {
        vThirdPersonMotor motor = (vThirdPersonMotor)target;
        var playerLayer = LayerMask.NameToLayer("Player");
        if (motor.gameObject.layer == LayerMask.NameToLayer("Default") || (playerLayer > 7 && motor.gameObject.layer != playerLayer))
        {
            PopUpInfoEditor window = ScriptableObject.CreateInstance<PopUpInfoEditor>();
            window.position = new Rect(Screen.width, Screen.height / 2, 360, 100);
            window.ShowPopup();
        }
        else if (playerLayer < 7)
        {
            vLayerManager.Create();
        }
    }

    public override void OnInspectorGUI()
    {
        if (!skin) skin = Resources.Load("skin") as GUISkin;
        GUI.skin = skin;

        vThirdPersonMotor motor = (vThirdPersonMotor)target;

        if (!motor) return;

        GUILayout.BeginVertical("Third Person Controller by Invector", "window");

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (motor.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Please assign the Layer of the Character to 'Player'", MessageType.Warning);
        }

        if (motor.groundLayer == 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Please assign the Ground Layer to 'Default' ", MessageType.Warning);
        }

        EditorGUILayout.BeginVertical();

        if (Application.isPlaying)
            GUILayout.Box("Current Health: " + motor.currentHealth.ToString());

        base.OnInspectorGUI();

        GUILayout.EndVertical();
        EditorGUILayout.EndVertical();        

        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }   

    //**********************************************************************************//
    // DEBUG RAYCASTS                                                                   //
    // draw the casts of the controller on play mode 							        //
    //**********************************************************************************//	
    [DrawGizmo(GizmoType.Selected)]
    private static void CustomDrawGizmos(Transform aTarget, GizmoType aGizmoType)
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            vThirdPersonMotor motor = (vThirdPersonMotor)aTarget.GetComponent<vThirdPersonMotor>();
            if (!motor) return;

            // debug auto crouch
            Vector3 posHead = motor.transform.position + Vector3.up * ((motor.colliderHeight * 0.5f) - motor.colliderRadius);
            Ray ray1 = new Ray(posHead, Vector3.up);
            Gizmos.DrawWireSphere(ray1.GetPoint((motor.headDetect - (motor.colliderRadius * 0.1f))), motor.colliderRadius * 0.9f);
            Handles.Label(ray1.GetPoint((motor.headDetect + (motor.colliderRadius))), "Head Detection");            
            // debug stopmove            
            Ray ray3 = new Ray(motor.transform.position + new Vector3(0, motor.stopMoveHeight, 0), motor.transform.forward);
            Debug.DrawRay(ray3.origin, ray3.direction * (motor._capsuleCollider.radius + motor.stopMoveDistance), Color.blue);
            Handles.Label(ray3.GetPoint(motor._capsuleCollider.radius + motor.stopMoveDistance), "Check for StopMove");
            // debug slopelimit            
            Ray ray4 = new Ray(motor.transform.position + new Vector3(0, motor.colliderHeight / 3.5f, 0), motor.transform.forward);
            Debug.DrawRay(ray4.origin, ray4.direction * 1f, Color.cyan);
            Handles.Label(ray4.GetPoint(1f), "Check for SlopeLimit");
            // debug stepOffset
            var dir = motor.isStrafing&&motor.input.magnitude>0 ? (motor.transform.right * motor.input.x + motor.transform.forward * motor.input.y ).normalized: motor.transform.forward;
            Ray ray5 = new Ray((motor.transform.position + new Vector3(0, motor.stepOffsetEnd, 0) + dir * ((motor._capsuleCollider).radius + 0.05f)), Vector3.down);
           
            Debug.DrawRay(ray5.origin, ray5.direction * (motor.stepOffsetEnd - motor.stepOffsetStart), Color.yellow);
            Handles.Label(ray5.origin, "Step OffSet");
        }
#endif
    }
}

public class PopUpInfoEditor : EditorWindow
{
    GUISkin skin;
    Vector2 rect = new Vector2(360, 100);

    void OnGUI()
    {
        this.titleContent = new GUIContent("Warning!");
        this.minSize = rect;

        EditorGUILayout.HelpBox("Please assign your 3rdPersonController to the Layer 'Player'.", MessageType.Warning);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("OK", GUILayout.Width(80), GUILayout.Height(20)))
            this.Close();
    }
}