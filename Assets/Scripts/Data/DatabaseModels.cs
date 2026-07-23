using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum DatabaseColumnType
{
    String,
    Int,
    Float,
    Bool,
    Table
}

[Serializable]
public enum DatabaseColumnRole
{
    Normal,
    PrimaryKey,
    Label
}

[Serializable]
public class DatabaseColumnDefinition
{
    public string Name = "Column";
    public string Description = "";
    public DatabaseColumnType Type = DatabaseColumnType.String;
    public DatabaseColumnRole Role = DatabaseColumnRole.Normal;
    public string ReferenceTableName = "";
}

[Serializable]
public class DatabaseRowData
{
    public List<string> Values = new List<string>();
}

[Serializable]
public class DatabaseTableData
{
    public string TableName = "NewTable";
    public List<DatabaseColumnDefinition> Columns = new List<DatabaseColumnDefinition>();
    public List<DatabaseRowData> Rows = new List<DatabaseRowData>();
}

[Serializable]
public class DatabaseExportFile
{
    public string DatabaseName = "Database";
    public List<DatabaseTableData> Tables = new List<DatabaseTableData>();
}

[Serializable]
public class DatabaseTableExportFile
{
    public string DatabaseName = "Database";
    public string TableName = "NewTable";
    public List<DatabaseColumnDefinition> Columns = new List<DatabaseColumnDefinition>();
    public List<DatabaseRowData> Rows = new List<DatabaseRowData>();
}

public class DatabaseProjectData : ScriptableObject
{
    public string DatabaseName = "Database";
    public List<DatabaseTableData> Tables = new List<DatabaseTableData>();
}
