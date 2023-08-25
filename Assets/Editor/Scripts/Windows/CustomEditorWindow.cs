using UnityEditor;
using UnityEngine;
public class TextData : ScriptableObject
{
    public string text;
}

public class CustomEditorWindow : EditorWindow
{
    private SerializedObject serializedObject;
    private SerializedProperty textAreaContent;


    private TextData textData;

    [MenuItem("Window/Custom Editor Window")]
    public static void ShowWindow()
    {
        GetWindow<CustomEditorWindow>("Custom Editor Window");
    }

    void OnEnable()
    {
        textData = ScriptableObject.CreateInstance<TextData>();
        serializedObject = new SerializedObject(textData);
        textAreaContent = serializedObject.FindProperty("text");
    }

    void OnGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(textAreaContent, GUIContent.none, GUILayout.Height(100));

        textData.text = "Something";

        if (GUILayout.Button("Undo"))
        {
            Undo.PerformUndo();
        }

        if (GUILayout.Button("Redo"))
        {
            Undo.PerformRedo();
        }

        if (serializedObject.ApplyModifiedProperties())
        {
            Undo.RecordObject(textData, "Edit Text Area");
        }
    }
}
