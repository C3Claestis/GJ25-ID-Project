using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PuzzleButton))]
public class PuzzleButtonEditor : Editor
{
    SerializedProperty buttonIndexProp;
    SerializedProperty targetLabelProp;
    SerializedProperty isLokerProp;
    SerializedProperty textLokerProp;

    void OnEnable()
    {
        // Menghubungkan properti dari skrip PuzzleButton
        buttonIndexProp = serializedObject.FindProperty("buttonIndex");
        targetLabelProp = serializedObject.FindProperty("targetLabel");
        isLokerProp = serializedObject.FindProperty("isLoker");
        textLokerProp = serializedObject.FindProperty("textLoker");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(buttonIndexProp);
        EditorGUILayout.PropertyField(targetLabelProp);
        EditorGUILayout.PropertyField(isLokerProp);

        // Hanya tampilkan textLokerProp jika isLokerProp bernilai true
        if (isLokerProp.boolValue)
        {
            EditorGUILayout.PropertyField(textLokerProp, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
