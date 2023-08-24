using System.IO;
using UnityEditor;
using UnityEngine;

public class TemplateEditorWindow : EditorWindow
{
    private const string PathToFiles = "Assets/Templates";
    private const string BlueprintJsonPath = "Assets/Templates/Example/Example.json";
    private string[] fileNames;
    [SerializeField] string selectedFileContent, previousSelectedFileContent;
    private int selectedIndex = -1;
    private bool visualize,previousVisualize;
    private UIConstructor uiConstructor;
    private Vector2 scrollPosition;


    [MenuItem("Window/Template Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(TemplateEditorWindow));
    }

    private void OnEnable()
    {
        LoadFileNames();
        uiConstructor = FindObjectOfType<UIConstructor>();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        // Left section
        EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.2f));
        DrawLeftSection();
        EditorGUILayout.EndVertical();

        // Right section
        EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.8f));
        DrawRightSection();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawLeftSection()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(position.width * 0.2f));

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button) { fixedWidth = 275 };
        Color vibrantGreen = new Color(0, 1, 0, 0.7f); // Vibrant green color
        GUIStyle selectedButtonStyle = new GUIStyle(buttonStyle);
        selectedButtonStyle.normal.background = MakeTex(2, 2, vibrantGreen); // Setting the background color
        selectedButtonStyle.normal.textColor = Color.green; // Setting the text color to green for selected button

        for (int i = 0; i < fileNames.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(fileNames[i], (i == selectedIndex) ? selectedButtonStyle : buttonStyle))
            {
                if (i == selectedIndex) DeselectFile();
                else SelectFile(i);
            }
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_editicon.sml")))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Rename"), false, RenameFile, i);
                menu.AddItem(new GUIContent("Delete"), false, DeleteFile, i);
                menu.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Create New Template", buttonStyle))
        {
            CreateNewTemplate();
        }
        GUILayout.EndVertical();
    }

    // Utility function to create a Texture2D with a specific color
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }



    private void DrawRightSection()
    {
        if (selectedIndex != -1)
        {
            EditorGUILayout.BeginHorizontal();
            visualize = EditorGUILayout.Toggle("Visualize", visualize);
            if (visualize != previousVisualize)
                HandleVisualization();
            previousVisualize = visualize;
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginChangeCheck();


            // Wrap the TextArea with a ScrollView
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(position.height * 0.9f)); // You can adjust the height as needed
            selectedFileContent = EditorGUILayout.TextArea(selectedFileContent, GUILayout.ExpandHeight(true));
            bool jsonUpdated = !selectedFileContent.Equals(previousSelectedFileContent);
            previousSelectedFileContent = selectedFileContent;
            EditorGUILayout.EndScrollView();

            if (jsonUpdated)
            {
                Undo.RecordObject(this, "JsonUpdated");
                HandleVisualization();
            }

            if (EditorGUI.EndChangeCheck() && jsonUpdated)
            {
                Undo.RecordObject(this, "JsonChange");
            }

            if (GUILayout.Button("Save"))
            {
                AssetDatabase.Refresh();
                File.WriteAllText(Path.Combine(PathToFiles, fileNames[selectedIndex] + ".json"), selectedFileContent);
            }
        }
        else
        {
            EditorGUILayout.LabelField("Click on any template to start editing");
        }
    }

    private void HandleVisualization()
    {
        if (visualize)
        {
            DeleteCurrentUI();

            if (uiConstructor == null)
                uiConstructor = new GameObject("UIConstructor").AddComponent<UIConstructor>();
            if (!string.IsNullOrEmpty(selectedFileContent))
                uiConstructor.ConstructUI(selectedFileContent);
        }
        else
        {
            DeleteCurrentUI();
        }
    }

    private void LoadFileNames()
    {
        fileNames = Directory.GetFiles(PathToFiles, "*.json");
        for (int i = 0; i < fileNames.Length; i++)
        {
            fileNames[i] = Path.GetFileNameWithoutExtension(fileNames[i]);
        }
    }

    private void SelectFile(int index)
    {
        selectedIndex = index;
        selectedFileContent = File.ReadAllText(Path.Combine(PathToFiles, fileNames[index] + ".json"));
    }

    private void DeselectFile()
    {
        selectedIndex = -1;
        selectedFileContent = null;
    }

    private void CreateNewTemplate()
    {
        string blueprintJson = File.ReadAllText(BlueprintJsonPath);
        string newFileName = "NewTemplate" + (fileNames.Length + 1) + ".json";
        File.WriteAllText(Path.Combine(PathToFiles, newFileName), blueprintJson);
        LoadFileNames();
        SelectFile(fileNames.Length - 1);
    }

    private void RenameFile(object index)
    {
        int fileIndex = (int)index;
        string oldName = fileNames[fileIndex];
        RenameDialog.ShowDialog(oldName, newName =>
        {
            string oldPath = Path.Combine(PathToFiles, oldName + ".json");
            string newPath = Path.Combine(PathToFiles, newName + ".json");

            if (File.Exists(newPath))
            {
                EditorUtility.DisplayDialog("Error", "A file with that name already exists!", "OK");
                return;
            }

            File.Move(oldPath, newPath);
            LoadFileNames();

            if (selectedIndex == fileIndex)
            {
                DeselectFile(); // Update the selected file content if needed
            }
        });

        AssetDatabase.Refresh();
    }

    private void DeleteFile(object index)
    {
        int fileIndex = (int)index;

        if (EditorUtility.DisplayDialog("Delete File", "Are you sure you want to delete " + fileNames[fileIndex] + "?", "Delete", "Cancel"))
        {
            string filePath = Path.Combine(PathToFiles, fileNames[fileIndex] + ".json");
            File.Delete(filePath);

            if (selectedIndex == fileIndex)
            {
                DeselectFile();
            }

            LoadFileNames();
        }

        AssetDatabase.Refresh();
    }

    private void DeleteCurrentUI()
    {
        if (uiConstructor != null && uiConstructor.CanvasObject != null)
        {
            DestroyImmediate(uiConstructor.CanvasObject);
            uiConstructor.CanvasObject = null; // Set it to null to avoid referencing destroyed object
        }
    }


    private void OnDestroy()
    {
        DeleteCurrentUI(); // Delete the UI when the window is closed

        // Optionally, delete the UIConstructor GameObject
        if (uiConstructor != null)
        {
            DestroyImmediate(uiConstructor.gameObject);
        }
    }
}
