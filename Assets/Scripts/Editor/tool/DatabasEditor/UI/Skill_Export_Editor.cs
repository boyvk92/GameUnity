using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class SOExport : EditorWindow
{
    private const string PrefKeyPrefix = "TuTien.DatabaseEditor.SOExport.";
    private const string PrefSelectedPreset = PrefKeyPrefix + "SelectedPreset";

    private const string PrefSkillState = PrefKeyPrefix + "SkillState";
    private const string PrefSkillLevelState = PrefKeyPrefix + "SkillLevelState";
    private const string PrefSkillBonusState = PrefKeyPrefix + "SkillBonusState";

    private static readonly string[] PresetOptions = new[]
    {
        "Skill",
        // "SkillBounus"
    };

    private DatabaseExportFile database;
    private int selectedSkillTableIndex = -1;
    private int selectedSkillBonusTableIndex = -1;
    private int selectedSkillLevelTableIndex = -1;
    
    private string exportAssetName = Path.GetFileNameWithoutExtension(DataConfig.SOExportFileName);
    private int selectedPresetIndex;

    [MenuItem("Tools/Database/Export ScriptableObject")]
    public static void Open()
    {
        GetWindow<SOExport>("SO Export");
    }

    private void OnEnable()
    {
        LoadDatabaseFile();
        LoadSelectedPreset();
    }

    private void OnDisable()
    {
        SaveCurrentPresetState();
    }

    private void OnGUI()
    {
        DrawToolbar();
        EditorGUILayout.Space(8);

        if (database == null)
        {
            DrawMissingDatabaseState();
            return;
        }

        DrawExportSettings();
        EditorGUILayout.Space(8);
        DrawExportAction();

        if (GUI.changed)
        {
            SaveCurrentPresetState();
        }
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Reload", EditorStyles.toolbarButton, GUILayout.Width(60)))
        {
            LoadDatabaseFile();
            GUIUtility.ExitGUI();
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawMissingDatabaseState()
    {
        EditorGUILayout.HelpBox("Chua co database de export. Hay tao luu database truoc.", MessageType.Info);

        if (GUILayout.Button("Reload Database", GUILayout.Height(28)))
        {
            LoadDatabaseFile();
        }
    }

    private void DrawExportSettings()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("Export Settings", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();

        DrawTableSelectors();
        DrawSkillColumnMappings();

        EditorGUILayout.LabelField("Output Folder", DataConfig.SOExportFolder);
        exportAssetName = EditorGUILayout.TextField("Asset Name", exportAssetName);
        if (string.IsNullOrEmpty(exportAssetName))
        {
            exportAssetName = GetDefaultAssetName();
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawTableSelectors()
    {
        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Tables", EditorStyles.boldLabel);

        if (database == null || database.Tables.Count == 0)
        {
            EditorGUILayout.HelpBox("Chua co table nao trong database.", MessageType.Info);
            return;
        }

        string[] tableNames = GetTableNames();
        selectedSkillTableIndex = DrawTablePopup("Skill Table", selectedSkillTableIndex, tableNames);
        selectedSkillBonusTableIndex = DrawTablePopup("SkillBounus Table", selectedSkillBonusTableIndex, tableNames);
        selectedSkillLevelTableIndex = DrawTablePopup("SkillLevel Table", selectedSkillLevelTableIndex, tableNames);
    }

    private void DrawSkillColumnMappings()
    {
        DatabaseTableData skillTable = GetSelectedSkillTable();
        if (skillTable == null)
        {
            EditorGUILayout.HelpBox("Hay chon bang Skill de map cot.", MessageType.Info);

            return;
        }

        string[] fieldNames = SO_Skill_Export.GetSkillFieldNames();

        EditorGUILayout.Space(6);
        EditorGUILayout.BeginVertical(GUI.skin.box);


        EditorGUILayout.EndVertical();
    }

    private int DrawTablePopup(string label, int currentIndex, string[] tableNames)
    {
        int safeIndex = ClampTableIndex(currentIndex, tableNames.Length);
        int newIndex = EditorGUILayout.Popup(label, safeIndex, tableNames);
        return ClampTableIndex(newIndex, tableNames.Length);
    }

    private string[] GetTableNames()
    {
        if (database == null || database.Tables.Count == 0)
        {
            return new[] { "<No Tables>" };
        }

        string[] names = new string[database.Tables.Count + 1];
        names[0] = "<Select Table>";
        for (int i = 0; i < database.Tables.Count; i++)
        {
            names[i + 1] = string.IsNullOrEmpty(database.Tables[i].TableName) ? "Unnamed Table" : database.Tables[i].TableName;
        }

        return names;
    }

    private void DrawExportAction()
    {
        if (GUILayout.Button("Export ScriptableObject", GUILayout.Height(32)))
        {
            ExportScriptableObject();
        }
    }

    private void ExportScriptableObject()
    {
        if (database == null)
        {
            return;
        }

        DatabaseTableData skillTable = GetSelectedSkillTable();
        if (skillTable == null)
        {
            EditorUtility.DisplayDialog("SO Export", "Hay chon bang Skill de export.", "OK");
            return;
        }

        DatabaseTableData skillLevelTable = GetSelectedSkillLevelTable();
        if (skillLevelTable == null)
        {
            EditorUtility.DisplayDialog("SO Export", "Hay chon bang skillLevelTable de export.", "OK");
            return;
        }

        DatabaseTableData skillBonusTable = GetSelectedSkillBonusTable();
        if (skillBonusTable == null)
        {
            EditorUtility.DisplayDialog("SO Export", "Hay chon bang SkillBounus de export.", "OK");
            return;
        }

        if (!EditorUtility.DisplayDialog("Confirm Export", "Export selected tables to ScriptableObject?", "Export", "Cancel"))
        {
            return;
        }

        EnsureFolderExists(DataConfig.SOExportFolder);

        string safeAssetName = GetSafeFileName(string.IsNullOrEmpty(exportAssetName)
            ? GetDefaultAssetName()
            : exportAssetName);
        string assetPath = Path.Combine(DataConfig.SOExportFolder, safeAssetName + ".asset").Replace("\\", "/");

        AssetDatabase.DeleteAsset(assetPath);

        SO_Skill_Export exportAsset = CreateInstance<SO_Skill_Export>();
        exportAsset.name = safeAssetName;
        exportAsset.BuildFromTables(skillTable, skillLevelTable, skillBonusTable);

        AssetDatabase.CreateAsset(exportAsset, assetPath);
        EditorUtility.SetDirty(exportAsset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Selection.activeObject = exportAsset;

        SaveCurrentPresetState();

        EditorUtility.DisplayDialog("SO Export", "Da export ScriptableObject vao:\n" + assetPath, "OK");
    }

    private DatabaseTableData GetSelectedSkillTable()
    {
        if (database == null || database.Tables.Count == 0)
        {
            return null;
        }

        int tableIndex = ClampTableIndex(selectedSkillTableIndex, database.Tables.Count + 1);
        if (tableIndex <= 0 || tableIndex > database.Tables.Count)
        {
            return null;
        }

        return database.Tables[tableIndex - 1];
    }

    private DatabaseTableData GetSelectedSkillBonusTable()
    {
        if (database == null || database.Tables.Count == 0)
        {
            return null;
        }

        int tableIndex = ClampTableIndex(selectedSkillBonusTableIndex, database.Tables.Count + 1);
        if (tableIndex <= 0 || tableIndex > database.Tables.Count)
        {
            return null;
        }

        return database.Tables[tableIndex - 1];
    }

    private DatabaseTableData GetSelectedSkillLevelTable()
    {
        if (database == null || database.Tables.Count == 0)
        {
            return null;
        }

        int tableIndex = ClampTableIndex(selectedSkillLevelTableIndex, database.Tables.Count + 1);
        if (tableIndex <= 0 || tableIndex > database.Tables.Count)
        {
            return null;
        }

        return database.Tables[tableIndex - 1];
    }

    private void LoadDatabaseFile()
    {
        database = LoadDatabaseFileInternal();
        LoadPresetState();
    }

    private DatabaseExportFile LoadDatabaseFileInternal()
    {
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), DataConfig.DatabaseFilePath);
        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath, Encoding.UTF8);
            DatabaseExportFile file = JsonUtility.FromJson<DatabaseExportFile>(json);
            if (file != null)
            {
                return file;
            }
        }

        DatabaseProjectData existingAsset = AssetDatabase.LoadAssetAtPath<DatabaseProjectData>(DataConfig.DatabaseAssetPath);
        if (existingAsset != null)
        {
            DatabaseExportFile migrated = new DatabaseExportFile();
            migrated.DatabaseName = existingAsset.DatabaseName;
            migrated.Tables = existingAsset.Tables;
            return migrated;
        }

        return null;
    }

    private void EnsureFolderExists(string assetPath)
    {
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
        Directory.CreateDirectory(fullPath);
    }

    private string GetSafeFileName(string fileName)
    {
        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(invalidChar.ToString(), "_");
        }

        return fileName.Trim();
    }

    private void LoadSelectedPreset()
    {
        selectedPresetIndex = Mathf.Clamp(EditorPrefs.GetInt(PrefSelectedPreset, 0), 0, PresetOptions.Length - 1);
        LoadPresetState();
    }

    private void LoadPresetState()
    {
        string stateJson = EditorPrefs.GetString(GetPresetStateKey(), string.Empty);
        if (string.IsNullOrEmpty(stateJson))
        {
            ApplyDefaultPresetState();
            return;
        }

        SOExportPresetState state = JsonUtility.FromJson<SOExportPresetState>(stateJson);
        if (state == null)
        {
            ApplyDefaultPresetState();
            return;
        }

        ApplyPresetState(state);
    }

    private void ApplyDefaultPresetState()
    {
        exportAssetName = GetDefaultAssetName();
        selectedSkillTableIndex = FindTablePopupIndexByName(GetDefaultSkillTableName());
        selectedSkillLevelTableIndex = FindTablePopupIndexByName(GetDefaultSkillLevelTableName());
        selectedSkillBonusTableIndex = FindTablePopupIndexByName(GetDefaultSkillBonusTableName());

        if (selectedSkillTableIndex < 0)
        {
            selectedSkillTableIndex = 0;
        }

        if (selectedSkillLevelTableIndex < 0)
        {
            selectedSkillLevelTableIndex = 0;
        }

        if (selectedSkillBonusTableIndex < 0)
        {
            selectedSkillBonusTableIndex = 0;
        }

        
    }

    private void ApplyPresetState(SOExportPresetState state)
    {
        exportAssetName = string.IsNullOrEmpty(state.AssetName) ? GetDefaultAssetName() : state.AssetName;
        selectedSkillTableIndex = FindTablePopupIndexByName(state.SkillTableName);
        selectedSkillLevelTableIndex = FindTablePopupIndexByName(state.SkillLevelTableName);
        selectedSkillBonusTableIndex = FindTablePopupIndexByName(state.SkillBonusTableName);

        if (selectedSkillTableIndex < 0)
        {
            selectedSkillTableIndex = 0;
        }

        if (selectedSkillLevelTableIndex < 0)
        {
            selectedSkillLevelTableIndex = 0;
        }

        if (selectedSkillBonusTableIndex < 0)
        {
            selectedSkillBonusTableIndex = 0;
        }
    }

    private void SaveCurrentPresetState()
    {
        SaveCurrentPresetState(selectedPresetIndex);
    }

    private void SaveCurrentPresetState(int presetIndex)
    {
        SOExportPresetState state = new SOExportPresetState();
        state.AssetName = exportAssetName;
        state.SkillTableName = GetSelectedTableName(selectedSkillTableIndex);
        state.SkillLevelTableName = GetSelectedTableName(selectedSkillLevelTableIndex);
        state.SkillBonusTableName = GetSelectedTableName(selectedSkillBonusTableIndex);

        EditorPrefs.SetString(GetPresetStateKey(presetIndex), JsonUtility.ToJson(state));
        EditorPrefs.SetInt(PrefSelectedPreset, presetIndex);
    }

    private int FindTablePopupIndexByName(string tableName)
    {
        if (database == null || string.IsNullOrEmpty(tableName))
        {
            return -1;
        }

        for (int i = 0; i < database.Tables.Count; i++)
        {
            if (database.Tables[i].TableName == tableName)
            {
                return i + 1;
            }
        }

        return 0;
    }

    private string GetSelectedTableName(int popupIndex)
    {
        if (database == null || database.Tables.Count == 0)
        {
            return string.Empty;
        }

        int tableIndex = ClampTableIndex(popupIndex, database.Tables.Count + 1);
        if (tableIndex <= 0 || tableIndex > database.Tables.Count)
        {
            return string.Empty;
        }

        return database.Tables[tableIndex - 1].TableName;
    }

    private int ClampTableIndex(int popupIndex, int popupLength)
    {
        if (popupLength <= 0)
        {
            return 0;
        }

        if (popupIndex < 0)
        {
            return 0;
        }

        if (popupIndex >= popupLength)
        {
            return popupLength - 1;
        }

        return popupIndex;
    }

    private string GetPresetStateKey()
    {
        return GetPresetStateKey(selectedPresetIndex);
    }

    private string GetPresetStateKey(int presetIndex)
    {
        return presetIndex == 0 ? PrefSkillState : PrefSkillBonusState;
    }

    

    private string GetDefaultAssetName()
    {
        return PresetOptions[Mathf.Clamp(selectedPresetIndex, 0, PresetOptions.Length - 1)];
    }

    private string GetDefaultSkillTableName()
    {
        return string.Empty;
    }

    private string GetDefaultSkillBonusTableName()
    {
        return string.Empty;
    }

    private string GetDefaultSkillLevelTableName()
    {
        return string.Empty;
    }


    private string GetDefaultFieldNameForColumn(string columnName)
    {
        string[] fieldNames = SO_Skill_Export.GetSkillFieldNames();
        for (int i = 1; i < fieldNames.Length; i++)
        {
            if (string.Equals(fieldNames[i], columnName, StringComparison.OrdinalIgnoreCase))
            {
                return fieldNames[i];
            }
        }

        return string.Empty;
    }

    private int FindFieldIndex(string[] fieldNames, string fieldName)
    {
        if (fieldNames == null)
        {
            return 0;
        }

        for (int i = 0; i < fieldNames.Length; i++)
        {
            if (string.Equals(fieldNames[i], fieldName, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return 0;
    }
}

[Serializable]
public class SOExportPresetState
{
    public string AssetName;
    public string SkillTableName;
    public string SkillLevelTableName;
    public string SkillBonusTableName;
}
