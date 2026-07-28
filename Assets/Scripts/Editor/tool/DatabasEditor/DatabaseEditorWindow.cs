using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class DatabaseEditorWindow : EditorWindow
{
    private DatabaseExportFile database;
    private int selectedTableIndex = -1;
    private Vector2 tableScroll;
    private Vector2 detailScroll;
    private Vector2 rowsScroll;
    private ReorderableList rowsList;
    private DatabaseTableData rowsListTable;
    private float[] rowsListColumnWidths;
    private int rowsListColumnCount = -1;

    [MenuItem("Tools/Database Editor")]
    public static void Open()
    {
        GetWindow<DatabaseEditorWindow>("Database Editor");
    }

    private void OnEnable()
    {
        LoadDatabaseFile();
    }

    private void OnDisable()
    {
        SaveDatabase();
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

        EditorGUILayout.BeginHorizontal();
        DrawTableList();
        DrawTableDetail();
        EditorGUILayout.EndHorizontal();

        if (GUI.changed)
        {
            SaveDatabase();
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

        if (GUILayout.Button("Create File", EditorStyles.toolbarButton, GUILayout.Width(90)))
        {
            CreateDatabaseFile();
            GUIUtility.ExitGUI();
        }

        if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(60)))
        {
            SaveDatabase();
            GUIUtility.ExitGUI();
        }

        GUILayout.FlexibleSpace();

        if (database != null)
        {
            if (GUILayout.Button("Import JSON", EditorStyles.toolbarButton, GUILayout.Width(90)))
            {
                ImportJson();
                GUIUtility.ExitGUI();
            }

            if (GUILayout.Button("Export JSON", EditorStyles.toolbarButton, GUILayout.Width(90)))
            {
                ExportJson();
                GUIUtility.ExitGUI();
            }

            if (GUILayout.Button("Export SO", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                SOExport.Open();
                GUIUtility.ExitGUI();
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawMissingDatabaseState()
    {
        EditorGUILayout.HelpBox("Chua co database file. Hay tao file de luu data ngay trong project.", MessageType.Info);

        if (GUILayout.Button("Create Database File", GUILayout.Height(28)))
        {
            CreateDatabaseFile();
        }
    }

    private void DrawTableList()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(220));
        EditorGUILayout.LabelField("Tables", EditorStyles.boldLabel);

        tableScroll = EditorGUILayout.BeginScrollView(tableScroll, GUI.skin.box, GUILayout.ExpandHeight(true));

        for (int i = 0; i < database.Tables.Count; i++)
        {
            bool isSelected = selectedTableIndex == i;
            DatabaseTableData table = database.Tables[i];

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Toggle(isSelected, string.IsNullOrEmpty(table.TableName) ? "Unnamed Table" : table.TableName, "Button"))
            {
                selectedTableIndex = i;
            }

            if (GUILayout.Button("X", GUILayout.Width(24)))
            {
                if (EditorUtility.DisplayDialog("Confirm Delete", $"Delete table \"{table.TableName}\"?", "Delete", "Cancel"))
                {
                    RemoveTable(i);
                    GUIUtility.ExitGUI();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add Table", GUILayout.Height(26)))
        {
            AddTable();
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawTableDetail()
    {
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("Table Detail", EditorStyles.boldLabel);

        if (selectedTableIndex < 0 || selectedTableIndex >= database.Tables.Count)
        {
            EditorGUILayout.HelpBox("Hay chon mot table ben trai de chinh sua.", MessageType.Info);
            EditorGUILayout.EndVertical();
            return;
        }

        DatabaseTableData table = database.Tables[selectedTableIndex];

        detailScroll = EditorGUILayout.BeginScrollView(detailScroll, GUI.skin.box, GUILayout.ExpandHeight(true));

        table.TableName = EditorGUILayout.TextField("Table Name", table.TableName);

        EditorGUILayout.Space(6);
        DrawColumnsTable(table);

        EditorGUILayout.Space(8);
        DrawRowsTable(table);

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawColumnsTable(DatabaseTableData table)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Columns", EditorStyles.boldLabel);
        if (GUILayout.Button("Add Column", GUILayout.Width(90)))
        {
            AddColumn(table);
        }
        EditorGUILayout.EndHorizontal();

        if (table.Columns.Count == 0)
        {
            EditorGUILayout.HelpBox("Chua co column nao. Them column de bat dau tao table.", MessageType.Info);
            return;
        }

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("Name", GUILayout.MinWidth(160));
        GUILayout.Label("Description", GUILayout.MinWidth(220));
        GUILayout.Label("Type", GUILayout.Width(90));
        GUILayout.Label("Role", GUILayout.Width(110));
        GUILayout.Label("Ref Table", GUILayout.Width(140));
        GUILayout.Label("Dropdown Options", GUILayout.Width(180));
        GUILayout.Label("", GUILayout.Width(24));
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < table.Columns.Count; i++)
        {
            DatabaseColumnDefinition column = table.Columns[i];
            EditorGUILayout.BeginHorizontal();
            column.Name = EditorGUILayout.TextField(column.Name, GUILayout.MinWidth(160));
            column.Description = EditorGUILayout.TextField(column.Description, GUILayout.MinWidth(220));
            column.Type = DrawColumnTypePopup(column.Type, GUILayout.Width(90));
            DatabaseColumnRole newRole = (DatabaseColumnRole)EditorGUILayout.EnumPopup(column.Role, GUILayout.Width(110));
            if (newRole != column.Role)
            {
                SetColumnRole(table, i, newRole);
            }
            DrawColumnReferenceTableSelector(column, GUILayout.Width(140));
            if (column.Type == DatabaseColumnType.Dropdown)
            {
                string newOptions = EditorGUILayout.TextField(column.DropdownOptions, GUILayout.Width(180));
                if (newOptions != column.DropdownOptions)
                {
                    column.DropdownOptions = newOptions;
                    MarkDatabaseDirty();
                }
            }
            else
            {
                GUILayout.Label("", GUILayout.Width(180));
            }
            if (GUILayout.Button("X", GUILayout.Width(24)))
            {
                if (EditorUtility.DisplayDialog("Confirm Delete", $"Delete column \"{column.Name}\"?", "Delete", "Cancel"))
                {
                    RemoveColumn(table, i);
                    GUIUtility.ExitGUI();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawRowsTable(DatabaseTableData table)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Rows", EditorStyles.boldLabel);
        if (GUILayout.Button("Add Row", GUILayout.Width(90)))
        {
            AddRow(table);
        }
        EditorGUILayout.EndHorizontal();

        if (table.Columns.Count == 0)
        {
            EditorGUILayout.HelpBox("Hay them column truoc khi nhap row.", MessageType.Warning);
            return;
        }

        if (table.Rows.Count == 0)
        {
            EditorGUILayout.HelpBox("Chua co row nao. Bam Add Row de tao ban ghi moi.", MessageType.Info);
            return;
        }

        float[] columnWidths = BuildColumnWidths(table);

        rowsScroll = EditorGUILayout.BeginScrollView(rowsScroll, GUI.skin.box, GUILayout.Height(260));
        DrawDataHeader(table, columnWidths);
        ReorderableList list = GetRowsList(table, columnWidths);
        list.DoLayoutList();
        EditorGUILayout.EndScrollView();
    }

    private void DrawDataHeader(DatabaseTableData table, float[] columnWidths)
    {
        GUIStyle headerBarStyle = new GUIStyle(EditorStyles.helpBox);
        headerBarStyle.padding = new RectOffset(6, 6, 8, 8);

        EditorGUILayout.BeginHorizontal(headerBarStyle, GUILayout.Height(40));
        GUILayout.Label("", GUILayout.Width(18));
        GUILayout.Label("#", GUILayout.Width(36));

        for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
        {
            DatabaseColumnDefinition column = table.Columns[columnIndex];
            string label = column.Name;
            if (string.IsNullOrEmpty(label))
            {
                label = $"Column {columnIndex + 1}";
            }

            if (!string.IsNullOrEmpty(column.Description))
            {
                label += "\n(" + column.Description + ")";
            }

            GUILayout.Label(label, GUILayout.Width(columnWidths[columnIndex]));
        }

        GUILayout.Label("", GUILayout.Width(70));
        EditorGUILayout.EndHorizontal();
    }

    private ReorderableList GetRowsList(DatabaseTableData table, float[] columnWidths)
    {
        rowsListColumnWidths = columnWidths;

        if (rowsList != null && rowsListTable == table && rowsListColumnCount == columnWidths.Length)
        {
            return rowsList;
        }

        rowsListTable = table;
        rowsListColumnCount = columnWidths.Length;
        rowsList = new ReorderableList(table.Rows, typeof(DatabaseRowData), true, false, false, false);
        rowsList.elementHeight = EditorGUIUtility.singleLineHeight + 8f;
        rowsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            DatabaseRowData row = table.Rows[index];
            EnsureRowSize(table, row);

            rect.y += 2f;
            rect.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, rect.height), isActive ? new Color(0.3f, 0.5f, 0.9f, 0.12f) : new Color(0f, 0f, 0f, 0.05f));

            float x = rect.x + 4f;

            Rect handleRect = new Rect(x, rect.y, 18f, rect.height);
            GUI.Label(handleRect, "||");
            x += 22f;

            Rect indexRect = new Rect(x, rect.y, 36f, rect.height);
            GUI.Label(indexRect, (index + 1).ToString());
            x += 40f;

            for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
            {
                DatabaseColumnDefinition column = table.Columns[columnIndex];
                float width = rowsListColumnWidths[columnIndex];
                Rect cellRect = new Rect(x, rect.y, width, rect.height);
                row.Values[columnIndex] = DrawCellField(table, index, columnIndex, column, row.Values[columnIndex], column.Type, cellRect);
                x += width + 4f;
            }

            Rect removeRect = new Rect(x, rect.y, 70f, rect.height);
            if (GUI.Button(removeRect, "Remove"))
            {
                if (EditorUtility.DisplayDialog("Confirm Delete", $"Delete row {index + 1}?", "Delete", "Cancel"))
                {
                    table.Rows.RemoveAt(index);
                    GUIUtility.ExitGUI();
                }
            }
        };
        rowsList.onReorderCallback = _ => MarkDatabaseDirty();
        return rowsList;
    }

    private float[] BuildColumnWidths(DatabaseTableData table)
    {
        float[] widths = new float[table.Columns.Count];
        for (int i = 0; i < table.Columns.Count; i++)
        {
            widths[i] = 140f;
        }

        return widths;
    }

    private string DrawCellField(DatabaseTableData currentTable, int rowIndex, int columnIndex, DatabaseColumnDefinition column, string value, DatabaseColumnType type, Rect rect)
    {
        switch (type)
        {
            case DatabaseColumnType.Int:
                int intValue;
                if (!int.TryParse(value, out intValue))
                {
                    intValue = 0;
                }
                intValue = EditorGUI.IntField(rect, intValue);
                return intValue.ToString();
            case DatabaseColumnType.Float:
                float floatValue;
                if (!float.TryParse(value, out floatValue))
                {
                    floatValue = 0f;
                }
                floatValue = EditorGUI.FloatField(rect, floatValue);
                return floatValue.ToString();
            case DatabaseColumnType.Bool:
                bool boolValue;
                if (!bool.TryParse(value, out boolValue))
                {
                    boolValue = false;
                }
                boolValue = EditorGUI.Toggle(rect, boolValue);
                return boolValue.ToString();
            case DatabaseColumnType.Table:
                return DrawTableReferenceField(currentTable, rowIndex, columnIndex, column, value, rect);
            case DatabaseColumnType.Dropdown:
                return DrawDropdownField(column, value, rect);
            default:
                return EditorGUI.TextField(rect, value);
        }
    }

    private string DrawDropdownField(DatabaseColumnDefinition column, string value, Rect rect)
    {
        string[] options = GetDropdownOptions(column);
        if (options.Length == 0)
        {
            return EditorGUI.TextField(rect, value);
        }

        string[] popupOptions = new string[options.Length + 1];
        popupOptions[0] = "<Select>";
        Array.Copy(options, 0, popupOptions, 1, options.Length);

        int currentIndex = Array.IndexOf(options, value);
        int newIndex = EditorGUI.Popup(rect, currentIndex < 0 ? 0 : currentIndex + 1, popupOptions);
        return newIndex <= 0 ? string.Empty : popupOptions[newIndex];
    }

    private void DrawColumnReferenceTableSelector(DatabaseColumnDefinition column, GUILayoutOption widthOption)
    {
        if (column.Type != DatabaseColumnType.Table)
        {
            return;
        }

        if (database == null || database.Tables.Count == 0)
        {
            EditorGUILayout.LabelField("No Tables", GUILayout.Width(140));
            return;
        }

        string[] tableNames = GetTableNames();
        int currentIndex = FindTableIndex(tableNames, column.ReferenceTableName);
        int newIndex = EditorGUILayout.Popup(currentIndex < 0 ? 0 : currentIndex, tableNames, widthOption);
        string newReference = newIndex <= 0 ? string.Empty : tableNames[newIndex];

        if (newReference != column.ReferenceTableName)
        {
            column.ReferenceTableName = newReference;
            MarkDatabaseDirty();
        }
    }

    private string DrawTableReferenceField(DatabaseTableData currentTable, int rowIndex, int columnIndex, DatabaseColumnDefinition column, string value, Rect rect)
    {
        DatabaseTableData referenceTable = GetTableByName(column.ReferenceTableName);
        if (referenceTable == null)
        {
            return EditorGUI.TextField(rect, value);
        }

        int primaryKeyIndex = GetColumnIndexByRole(referenceTable, DatabaseColumnRole.PrimaryKey);
        int labelIndex = GetColumnIndexByRole(referenceTable, DatabaseColumnRole.Label);

        if (primaryKeyIndex < 0 || labelIndex < 0)
        {
            GUI.Label(rect, "Need PK + Label");
            return value;
        }

        string displayLabel = GetReferenceDisplayLabel(referenceTable, primaryKeyIndex, labelIndex, value);
        Rect buttonRect = rect;

        if (GUI.Button(buttonRect, displayLabel, GUI.skin.button))
        {
            List<DatabaseReferenceItem> items = BuildReferenceItems(referenceTable, primaryKeyIndex, labelIndex);
            PopupWindow.Show(buttonRect, new DatabaseReferencePopupContent(items, value, delegate (string selectedPrimaryKey)
            {
                if (currentTable != null && rowIndex >= 0 && rowIndex < currentTable.Rows.Count && columnIndex >= 0 && columnIndex < currentTable.Columns.Count)
                {
                    currentTable.Rows[rowIndex].Values[columnIndex] = selectedPrimaryKey;
                    MarkDatabaseDirty();
                }
            }));
        }

        return value;
    }

    private string GetReferenceDisplayLabel(DatabaseTableData referenceTable, int primaryKeyIndex, int labelIndex, string value)
    {
        for (int i = 0; i < referenceTable.Rows.Count; i++)
        {
            DatabaseRowData row = referenceTable.Rows[i];
            EnsureRowSize(referenceTable, row);
            if (row.Values[primaryKeyIndex] == value)
            {
                string label = row.Values[labelIndex];
                if (string.IsNullOrEmpty(label))
                {
                    label = row.Values[primaryKeyIndex];
                }

                return label;
            }
        }

        return string.IsNullOrEmpty(value) ? "Select..." : value;
    }

    private List<DatabaseReferenceItem> BuildReferenceItems(DatabaseTableData referenceTable, int primaryKeyIndex, int labelIndex)
    {
        List<DatabaseReferenceItem> items = new List<DatabaseReferenceItem>();

        foreach (DatabaseRowData row in referenceTable.Rows)
        {
            EnsureRowSize(referenceTable, row);
            string primaryKey = row.Values[primaryKeyIndex];
            string label = row.Values[labelIndex];

            if (string.IsNullOrEmpty(label))
            {
                label = primaryKey;
            }

            items.Add(new DatabaseReferenceItem
            {
                PrimaryKey = primaryKey,
                Label = label
            });
        }

        return items;
    }

    private DatabaseTableData GetTableByName(string tableName)
    {
        if (database == null || string.IsNullOrEmpty(tableName))
        {
            return null;
        }

        foreach (DatabaseTableData table in database.Tables)
        {
            if (table.TableName == tableName)
            {
                return table;
            }
        }

        return null;
    }

    private string[] GetTableNames()
    {
        if (database == null || database.Tables.Count == 0)
        {
            return new[] { "<Select Table>" };
        }

        string[] names = new string[database.Tables.Count + 1];
        names[0] = "<Select Table>";
        for (int i = 0; i < database.Tables.Count; i++)
        {
            names[i + 1] = database.Tables[i].TableName;
        }

        return names;
    }

    private int FindTableIndex(string[] tableNames, string tableName)
    {
        if (tableNames == null)
        {
            return -1;
        }

        for (int i = 0; i < tableNames.Length; i++)
        {
            if (tableNames[i] == tableName)
            {
                return i;
            }
        }

        return -1;
    }

    private int GetColumnIndexByRole(DatabaseTableData table, DatabaseColumnRole role)
    {
        for (int i = 0; i < table.Columns.Count; i++)
        {
            if (table.Columns[i].Role == role)
            {
                return i;
            }
        }

        return -1;
    }

    private DatabaseColumnType DrawColumnTypePopup(DatabaseColumnType currentType, GUILayoutOption widthOption)
    {
        string[] options = new[]
        {
            "String",
            "Int",
            "Float",
            "Bool",
            "Table",
            "Dropdown"
        };

        int selectedIndex = (int)currentType;
        if (selectedIndex < 0 || selectedIndex >= options.Length)
        {
            selectedIndex = 0;
        }

        int newIndex = EditorGUILayout.Popup(selectedIndex, options, widthOption);
        return (DatabaseColumnType)newIndex;
    }

    private void LoadDatabaseFile()
    {
        database = LoadDatabaseFileInternal();

        if (database != null && database.Tables.Count > 0)
        {
            selectedTableIndex = Mathf.Clamp(selectedTableIndex, 0, database.Tables.Count - 1);
        }
        else
        {
            selectedTableIndex = -1;
        }
    }

    private void CreateDatabaseFile()
    {
        database = new DatabaseExportFile();
        database.DatabaseName = "Database";
        SaveDatabase();
        selectedTableIndex = -1;
    }

    private void AddTable()
    {
        EnsureDatabase();

        DatabaseTableData table = new DatabaseTableData();
        table.TableName = $"Table_{database.Tables.Count + 1}";
        database.Tables.Add(table);
        selectedTableIndex = database.Tables.Count - 1;
        MarkDatabaseDirty();
    }

    private void RemoveTable(int index)
    {
        EnsureDatabase();

        if (index < 0 || index >= database.Tables.Count)
        {
            return;
        }

        database.Tables.RemoveAt(index);
        selectedTableIndex = Mathf.Clamp(selectedTableIndex, -1, database.Tables.Count - 1);
        MarkDatabaseDirty();
    }

    private void AddColumn(DatabaseTableData table)
    {
        table.Columns.Add(new DatabaseColumnDefinition { Name = $"Column_{table.Columns.Count + 1}" });

        foreach (DatabaseRowData row in table.Rows)
        {
            row.Values.Add(string.Empty);
        }

        MarkDatabaseDirty();
    }

    private void SetColumnRole(DatabaseTableData table, int columnIndex, DatabaseColumnRole role)
    {
        if (table == null || columnIndex < 0 || columnIndex >= table.Columns.Count)
        {
            return;
        }

        if (role == DatabaseColumnRole.PrimaryKey || role == DatabaseColumnRole.Label)
        {
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (i == columnIndex)
                {
                    continue;
                }

                if (table.Columns[i].Role == role)
                {
                    table.Columns[i].Role = DatabaseColumnRole.Normal;
                }
            }
        }

        table.Columns[columnIndex].Role = role;
        MarkDatabaseDirty();
    }

    private void RemoveColumn(DatabaseTableData table, int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= table.Columns.Count)
        {
            return;
        }

        table.Columns.RemoveAt(columnIndex);

        foreach (DatabaseRowData row in table.Rows)
        {
            if (columnIndex < row.Values.Count)
            {
                row.Values.RemoveAt(columnIndex);
            }
        }

        MarkDatabaseDirty();
    }

    private void AddRow(DatabaseTableData table)
    {
        DatabaseRowData row = new DatabaseRowData();
        EnsureRowSize(table, row);
        table.Rows.Add(row);
        MarkDatabaseDirty();
    }

    private void EnsureRowSize(DatabaseTableData table, DatabaseRowData row)
    {
        while (row.Values.Count < table.Columns.Count)
        {
            row.Values.Add(string.Empty);
        }

        while (row.Values.Count > table.Columns.Count)
        {
            row.Values.RemoveAt(row.Values.Count - 1);
        }
    }

    private void ExportJson()
    {
        if (database == null)
        {
            return;
        }

        if (!EditorUtility.DisplayDialog("Confirm Export", "Export all tables to JSON files?", "Export", "Cancel"))
        {
            return;
        }

        SaveDatabase();
        EnsureFolderExists(DataConfig.DatabaseExportFolder);

        int exportedCount = 0;
        foreach (DatabaseTableData table in database.Tables)
        {
            DatabaseTableExportFile exportFile = new DatabaseTableExportFile();
            exportFile.DatabaseName = database.DatabaseName;
            exportFile.TableName = table.TableName;

            foreach (DatabaseColumnDefinition column in table.Columns)
            {
                exportFile.Columns.Add(new DatabaseColumnDefinition
                {
                    Name = column.Name,
                    Description = column.Description,
                    Type = column.Type,
                    Role = column.Role,
                    ReferenceTableName = column.ReferenceTableName,
                    DropdownOptions = column.DropdownOptions
                });
            }

            foreach (DatabaseRowData row in table.Rows)
            {
                DatabaseRowData copiedRow = new DatabaseRowData();
                copiedRow.Values.AddRange(row.Values);
                exportFile.Rows.Add(copiedRow);
            }

            string baseName = string.IsNullOrEmpty(table.TableName) ? $"Table_{exportedCount + 1}" : table.TableName;
            string fileName = GetSafeFileName(baseName);
            string json = JsonUtility.ToJson(exportFile, true);
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), DataConfig.DatabaseExportFolder, fileName + ".json");
            File.WriteAllText(fullPath, json, Encoding.UTF8);
            exportedCount++;
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Database Editor", $"Da export {exportedCount} file JSON vao:\n{DataConfig.DatabaseExportFolder}", "OK");
    }

    private void ImportJson()
    {
        EnsureDatabase();

        string startFolder = Path.Combine(Directory.GetCurrentDirectory(), DataConfig.DatabaseExportFolder);
        string filePath = EditorUtility.OpenFilePanel("Import Table JSON", startFolder, "json");
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        string json = File.ReadAllText(filePath, Encoding.UTF8);
        DatabaseTableExportFile importFile = JsonUtility.FromJson<DatabaseTableExportFile>(json);
        if (importFile == null)
        {
            EditorUtility.DisplayDialog("Database Editor", "Khong doc duoc file JSON.", "OK");
            return;
        }

        DatabaseTableData table = new DatabaseTableData();
        table.TableName = string.IsNullOrEmpty(importFile.TableName)
            ? Path.GetFileNameWithoutExtension(filePath)
            : importFile.TableName;

        foreach (DatabaseColumnDefinition column in importFile.Columns)
        {
            table.Columns.Add(new DatabaseColumnDefinition
            {
                Name = column.Name,
                Description = column.Description,
                Type = column.Type,
                Role = column.Role,
                ReferenceTableName = column.ReferenceTableName,
                DropdownOptions = column.DropdownOptions
            });
        }

        foreach (DatabaseRowData row in importFile.Rows)
        {
            DatabaseRowData copiedRow = new DatabaseRowData();
            copiedRow.Values.AddRange(row.Values);
            EnsureRowSize(table, copiedRow);
            table.Rows.Add(copiedRow);
        }

        database.Tables.Add(table);
        selectedTableIndex = database.Tables.Count - 1;
        MarkDatabaseDirty();
    }

    private void MarkDatabaseDirty()
    {
        if (database == null)
        {
            return;
        }

        SaveDatabase();
    }

    private void SaveDatabase()
    {
        if (database == null)
        {
            return;
        }

        EnsureFolderExists(DataConfig.DatabaseExportFolder);

        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), DataConfig.DatabaseFilePath);
        string json = JsonUtility.ToJson(database, true);
        File.WriteAllText(fullPath, json, Encoding.UTF8);

        AssetDatabase.Refresh();
    }

    private void EnsureDatabase()
    {
        if (database != null)
        {
            return;
        }

        LoadDatabaseFile();

        if (database == null)
        {
            CreateDatabaseFile();
        }
    }

    private void EnsureFolderExists(string assetPath)
    {
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
        Directory.CreateDirectory(fullPath);
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
            database = migrated;
            SaveDatabase();
            return migrated;
        }

        return new DatabaseExportFile();
    }

    private string GetSafeFileName(string fileName)
    {
        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(invalidChar.ToString(), "_");
        }

        return fileName.Trim();
    }

    private string[] GetDropdownOptions(DatabaseColumnDefinition column)
    {
        if (column == null || string.IsNullOrEmpty(column.DropdownOptions))
        {
            return Array.Empty<string>();
        }

        string[] rawOptions = column.DropdownOptions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < rawOptions.Length; i++)
        {
            rawOptions[i] = rawOptions[i].Trim();
        }

        return rawOptions;
    }
}

[Serializable]
public class DatabaseReferenceItem
{
    public string PrimaryKey;
    public string Label;
}

public class DatabaseReferencePopupContent : PopupWindowContent
{
    private readonly List<DatabaseReferenceItem> items;
    private readonly Action<string> onSelect;
    private string searchText = string.Empty;
    private Vector2 scrollPosition;
    private readonly string currentValue;

    public DatabaseReferencePopupContent(List<DatabaseReferenceItem> items, string currentValue, Action<string> onSelect)
    {
        this.items = items ?? new List<DatabaseReferenceItem>();
        this.currentValue = currentValue;
        this.onSelect = onSelect;
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(320f, 360f);
    }

    public override void OnGUI(Rect rect)
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Search", EditorStyles.boldLabel);
        searchText = EditorGUILayout.TextField(searchText);

        EditorGUILayout.Space(4);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        List<DatabaseReferenceItem> filteredItems = GetFilteredItems();

        if (filteredItems.Count == 0)
        {
            EditorGUILayout.HelpBox("No matching items.", MessageType.Info);
        }
        else
        {
            foreach (DatabaseReferenceItem item in filteredItems)
            {
                bool isSelected = item.PrimaryKey == currentValue;
                string buttonText = item.Label;
                if (!string.IsNullOrEmpty(item.PrimaryKey) && item.PrimaryKey != item.Label)
                {
                    buttonText += "  [" + item.PrimaryKey + "]";
                }

                if (GUILayout.Button(buttonText, isSelected ? EditorStyles.miniButtonMid : EditorStyles.miniButton))
                {
                    onSelect?.Invoke(item.PrimaryKey);
                    editorWindow.Close();
                }
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private List<DatabaseReferenceItem> GetFilteredItems()
    {
        if (string.IsNullOrEmpty(searchText))
        {
            return items;
        }

        string query = searchText.Trim().ToLowerInvariant();
        List<DatabaseReferenceItem> filtered = new List<DatabaseReferenceItem>();

        foreach (DatabaseReferenceItem item in items)
        {
            string label = item.Label ?? string.Empty;
            string primaryKey = item.PrimaryKey ?? string.Empty;

            if (label.ToLowerInvariant().Contains(query) || primaryKey.ToLowerInvariant().Contains(query))
            {
                filtered.Add(item);
            }
        }

        return filtered;
    }
}
