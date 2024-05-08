using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;




[CustomEditor(typeof(WriteTextNode))]
public class WriteTextNodeEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        WriteTextNode myScript = (WriteTextNode)target;
        GUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if(GUILayout.Button("Create Child Node", GUILayout.Width(200), GUILayout.Height(50)))
        {

            // Call the CreateNewNode method of the WriteTextNode script
            myScript.CreateNewNode();


        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        DrawDefaultInspector();

    }
}