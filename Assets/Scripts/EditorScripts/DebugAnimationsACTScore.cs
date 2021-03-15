using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ACT_UI))]
public class DebugAnimationsACTScore : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ACT_UI script = (ACT_UI)target;

        if (GUILayout.Button("ANimateResultIcon"))
        {
            script.StartAnimationEditor();
        }
    }
}
#endif
