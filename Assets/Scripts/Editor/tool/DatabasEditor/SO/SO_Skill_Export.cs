using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;



[Serializable]
public class SkillLevel
{
    public string id;
    public string Skill_Id;
    public int train;
    public int level;
    public string label;
    
    public List<SkillBookBonus> bounus = new List<SkillBookBonus>();
}

[Serializable]
public class SkillBookBonus
{
    public string Level_ID;
    public string Type;
    public int Value;
}

[Serializable]
public class SkillBookData
{
    public string id;
    public string Name;
    public string Type;
    public string Icon;

    public List<SkillLevel> levels = new List<SkillLevel>();
}

[Serializable]
public class SkillColumnMapping
{
    public string ColumnName;
    public string FieldName;
}

public class SO_Skill_Export : ScriptableObject
{
    public string TableName = "Skill";
    public List<DatabaseColumnDefinition> Columns = new List<DatabaseColumnDefinition>();
    public List<SkillBookData> Skills = new List<SkillBookData>();

    public void BuildFromTables(DatabaseTableData skillTable, DatabaseTableData skillLevelTable, DatabaseTableData skillBonusTable)
    {
        TableName = skillTable != null ? skillTable.TableName : string.Empty;
        Columns.Clear();
        Skills.Clear();

        if (skillTable == null)
        {
            return;
        }

        for (int i = 0; i < skillTable.Columns.Count; i++)
        {
            DatabaseColumnDefinition column = skillTable.Columns[i];
            Columns.Add(new DatabaseColumnDefinition
            {
                Name = column.Name,
                Description = column.Description,
                Type = column.Type,
                Role = column.Role,
                ReferenceTableName = column.ReferenceTableName,
                DropdownOptions = column.DropdownOptions
            });
        }

        for (int rowIndex = 0; rowIndex < skillTable.Rows.Count; rowIndex++)
        {
            DatabaseRowData row = skillTable.Rows[rowIndex];
            SkillBookData skill = BuildFromRow<SkillBookData>(row);
            skill.levels = GetSkillLevel(skill.id, skillLevelTable, skillBonusTable);
            Skills.Add(skill);
        }
    }

    List<SkillLevel> GetSkillLevel(string skillID, DatabaseTableData skillLevelTable, DatabaseTableData skillBonusTable) { 
        if(skillID == null || skillLevelTable == null){
            Debug.Log("skillID == null || skillLevelTable == null");
            return new List<SkillLevel>();
        }

        List<SkillLevel> levels = new List<SkillLevel>();

       
        
        for (int rowIndex = 0; rowIndex < skillLevelTable.Rows.Count; rowIndex++)
        {
            DatabaseRowData row = skillLevelTable.Rows[rowIndex];
            SkillLevel level = BuildFromRow<SkillLevel>(row);
            FieldInfo[] levelFields = typeof(SkillLevel).GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < levelFields.Length; i++)
            {
                object value = levelFields[i].GetValue(level);
                Debug.Log("level." + levelFields[i].Name + " = " + (value != null ? value.ToString() : "null"));
            }
            
            if(isEquals(skillID, level, "Skill_Id")){
            level.bounus = GetSkillBounus(level.id, skillBonusTable);
               levels.Add(level);
            }
        }
        return levels;
    }

    List<SkillBookBonus> GetSkillBounus(string skillLevelId, DatabaseTableData skillBonusTable) { 
        if(skillLevelId == null || skillBonusTable == null){return new List<SkillBookBonus>();}

        List<SkillBookBonus> bonuses = new List<SkillBookBonus>();
        
        for (int rowIndex = 0; rowIndex < skillBonusTable.Rows.Count; rowIndex++)
        {
            DatabaseRowData row = skillBonusTable.Rows[rowIndex];
            SkillBookBonus bonus = BuildFromRow<SkillBookBonus>(row);
            if(isEquals(skillLevelId, bonus, "Level_ID")){
               
               bonuses.Add(bonus);
            }
        }
        return bonuses;
    }

    private bool isEquals<T>(string id, T row, string key)
    {
        if (row == null)
        {
            return false;
        }

        FieldInfo field = typeof(T).GetField(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (field != null)
        {
            object value = field.GetValue(row);
            string rowId = value as string;
            return !string.IsNullOrEmpty(rowId) &&
                   string.Equals(id, rowId, StringComparison.OrdinalIgnoreCase);
        }

        PropertyInfo property = typeof(T).GetProperty(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (property != null)
        {
            object value = property.GetValue(row, null);
            string rowId = value as string;
            return !string.IsNullOrEmpty(rowId) &&
                   string.Equals(id, rowId, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    private SkillBookBonus BuildBonusFromRow(DatabaseTableData table, DatabaseRowData row)
    {
        SkillBookBonus bonus = new SkillBookBonus();
        if (table == null || row == null)
        {
            return bonus;
        }

        for (int columnIndex = 0; columnIndex < table.Columns.Count && columnIndex < row.Values.Count; columnIndex++)
        {
            DatabaseColumnDefinition column = table.Columns[columnIndex];
            SetBonusFieldValue(bonus, column.Name, row.Values[columnIndex]);
        }

        return bonus;
    }

    private T BuildFromRow<T>(DatabaseRowData row) where T : new()
    {
        T result = new T();
        if (row == null)
        {
            return result;
        }

        FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
        PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        int valueCount = row.Values != null ? row.Values.Count : 0;
        int fieldCount = fields != null ? fields.Length : 0;
        int propertyCount = properties != null ? properties.Length : 0;
        int count = valueCount;
        if (fieldCount > 0 && count > fieldCount)
        {
            count = fieldCount;
        }
        if (propertyCount > 0 && count > propertyCount)
        {
            count = propertyCount;
        }

        for (int i = 0; i < count; i++)
        {
            string value = row.Values[i];

            if (i < fieldCount)
            {
                FieldInfo field = fields[i];
                if (field.FieldType != typeof(string) &&
                    field.FieldType != typeof(int) &&
                    field.FieldType != typeof(float) &&
                    field.FieldType != typeof(bool))
                {
                    continue;
                }

                object convertedValue = ConvertValue(value, field.FieldType);
                field.SetValue(result, convertedValue);
                continue;
            }

            int propertyIndex = i - fieldCount;
            if (propertyIndex < propertyCount)
            {
                PropertyInfo property = properties[propertyIndex];
                if (property.CanWrite)
                {
                    if (property.PropertyType != typeof(string) &&
                        property.PropertyType != typeof(int) &&
                        property.PropertyType != typeof(float) &&
                        property.PropertyType != typeof(bool))
                    {
                        continue;
                    }

                    object convertedValue = ConvertValue(value, property.PropertyType);
                    property.SetValue(result, convertedValue, null);
                }
            }
        }

        return result;
    }

    private string ResolveFieldName(string columnName, List<SkillColumnMapping> mappings)
    {
        if (mappings != null)
        {
            for (int i = 0; i < mappings.Count; i++)
            {
                SkillColumnMapping mapping = mappings[i];
                if (mapping != null && !string.IsNullOrEmpty(mapping.ColumnName) &&
                    string.Equals(mapping.ColumnName, columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return mapping.FieldName;
                }
            }
        }

        return columnName;
    }


    private object ConvertValue(string value, Type type)
    {
        if (type == typeof(string))
        {
            return value;
        }

        if (type == typeof(int))
        {
            int result;
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result) ? result : 0;
        }

        if (type == typeof(float))
        {
            float result;
            return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result) ? result : 0f;
        }

        if (type == typeof(bool))
        {
            bool result;
            return bool.TryParse(value, out result) && result;
        }

        return value;
    }

    private void SetBonusFieldValue(SkillBookBonus bonus, string fieldName, string value)
    {
        if (bonus == null || string.IsNullOrEmpty(fieldName))
        {
            return;
        }

        string resolvedFieldName = ResolveBonusFieldName(fieldName);
        if (string.IsNullOrEmpty(resolvedFieldName))
        {
            return;
        }

        FieldInfo field = typeof(SkillBookBonus).GetField(resolvedFieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (field == null)
        {
            return;
        }

        object convertedValue = ConvertValue(value, field.FieldType);
        field.SetValue(bonus, convertedValue);
    }

    private string ResolveBonusFieldName(string columnName)
    {
        if (string.IsNullOrEmpty(columnName))
        {
            return columnName;
        }

        if (string.Equals(columnName, "ID", StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        if (string.Equals(columnName, "SkillId", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(columnName, "Skill_ID", StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        return columnName;
    }

    private string GetBonusSkillId(DatabaseTableData table, DatabaseRowData row)
    {
        if (table == null || row == null)
        {
            return string.Empty;
        }

        for (int columnIndex = 0; columnIndex < table.Columns.Count && columnIndex < row.Values.Count; columnIndex++)
        {
            DatabaseColumnDefinition column = table.Columns[columnIndex];
            if (string.Equals(column.Name, "ID", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(column.Name, "Skill_ID", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(column.Name, "SkillId", StringComparison.OrdinalIgnoreCase))
            {
                return row.Values[columnIndex];
            }
        }

        return string.Empty;
    }

    private string NormalizeKey(string value)
    {
        return string.IsNullOrEmpty(value) ? string.Empty : value.Trim();
    }

    public static string[] GetSkillFieldNames()
    {
        FieldInfo[] fields = typeof(SkillBookData).GetFields(BindingFlags.Public | BindingFlags.Instance);
        string[] fieldNames = new string[fields.Length + 1];
        fieldNames[0] = "<Ignore>";

        for (int i = 0; i < fields.Length; i++)
        {
            fieldNames[i + 1] = fields[i].Name;
        }

        return fieldNames;
    }
}
