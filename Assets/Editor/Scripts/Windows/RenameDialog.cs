using System;
using UnityEditor;
using UnityEngine;

public class RenameDialog : EditorWindow
{
    public string newName;
    public Action<string> onRenameConfirmed;

    public static void ShowDialog(string oldName, Action<string> onRenameConfirmed)
    {
        RenameDialog window = CreateInstance<RenameDialog>();
        window.newName = oldName;
        window.onRenameConfirmed = onRenameConfirmed;
        window.position = new Rect(200, 200, 250, 80);
        window.ShowUtility();
    }

    private void OnGUI()
    {
        newName = EditorGUILayout.TextField("New Name:", newName);

        if (GUILayout.Button("Rename"))
        {
            onRenameConfirmed?.Invoke(newName);
            Close();
            AssetDatabase.Refresh();
        }
    }
}
