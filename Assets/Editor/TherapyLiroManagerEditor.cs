using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if DEBUG_LIRO
[CustomEditor(typeof(TherapyLIROManager))]
public class TherapyLiroManagerEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Update Therapy"))
        {
            if (Application.isPlaying && Application.isEditor)
            {
                TherapyLIROManager.Instance.ChangeLIROSectionButton();
            }
        }
    }

}
#endif
