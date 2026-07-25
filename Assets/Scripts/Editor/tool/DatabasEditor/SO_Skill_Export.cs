using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

[Serializable]
public class SkillBookBonus
{
    public string Skill_ID;
    public string Type;
    public int Value;
    public int Level;
}

[Serializable]
public class SkillBookData
{
    public string id;
    public string Name;
    public string Type;
    public string Icon;

    public List<SkillBookBonus> bounus = new List<SkillBookBonus>();
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

    public void BuildFromTable(DatabaseTableData table)
    {
        BuildFromTable(table, null);
    }

    public void BuildFromTable(DatabaseTableData table, List<SkillColumnMapping> mappings)
    {
        BuildFromTables(table, null, mappings);
    }

    public void BuildFromTables(DatabaseTableData skillTable, DatabaseTableData skillBonusTable, List<SkillColumnMapping> mappings)
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
                ReferenceTableName = column.ReferenceTableName
            });
        }

        for (int rowIndex = 0; rowIndex < skillTable.Rows.Count; rowIndex++)
        {
            DatabaseRowData row = skillTable.Rows[rowIndex];
            SkillBookData skill = new SkillBookData();

            for (int columnIndex = 0; columnIndex < skillTable.Columns.Count && columnIndex < row.Values.Count; columnIndex++)
            {
                string fieldName = ResolveFieldName(skillTable.Columns[columnIndex].Name, mappings);
                SetFieldValue(skill, fieldName, row.Values[columnIndex]);
            }

            Skills.Add(skill);
        }

        if (skillBonusTable == null)
        {
            return;
        }

        for (int rowIndex = 0; rowIndex < skillBonusTable.Rows.Count; rowIndex++)
        {
            DatabaseRowData row = skillBonusTable.Rows[rowIndex];
            SkillBookBonus bonus = BuildBonusFromRow(skillBonusTable, row);
            if (bonus == null || string.IsNullOrEmpty(bonus.Skill_ID))
            {
                continue;
            }

            for (int skillIndex = 0; skillIndex < Skills.Count; skillIndex++)
            {
                SkillBookData skill = Skills[skillIndex];
                if (skill != null && string.Equals(NormalizeKey(skill.id), NormalizeKey(bonus.Skill_ID), StringComparison.OrdinalIgnoreCase))
                {
                    skill.bounus.Add(bonus);
                    break;
                }
            }
        }
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

    private void SetFieldValue(SkillBookData skill, string fieldName, string value)
    {
        if (skill == null || string.IsNullOrEmpty(fieldName))
        {
            return;
        }

        FieldInfo field = typeof(SkillBookData).GetField(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (field == null)
        {
            return;
        }

        object convertedValue = ConvertValue(value, field.FieldType);
        field.SetValue(skill, convertedValue);
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
            return "Skill_ID";
        }

        if (string.Equals(columnName, "SkillId", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(columnName, "Skill_ID", StringComparison.OrdinalIgnoreCase))
        {
            return "Skill_ID";
        }

        return columnName;
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
