using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

public static class DatabaseJsonLoader
{
    public static List<T> Read<T>(string path) where T : new()
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("DatabaseJsonLoader.Read: path is empty.");
            return new List<T>();
        }

        TextAsset jsonFile = Resources.Load<TextAsset>(path);
        if (jsonFile == null)
        {
            Debug.LogError("DatabaseJsonLoader.Read: file not found at Resources/" + path);
            return new List<T>();
        }

        DatabaseTableExportFile database = JsonUtility.FromJson<DatabaseTableExportFile>(jsonFile.text);
        if (database == null)
        {
            Debug.LogError("DatabaseJsonLoader.Read: invalid json at Resources/" + path);
            return new List<T>();
        }

        List<T> result = new List<T>();

        for (int i = 0; i < database.Rows.Count; i++)
        {
            DatabaseRowData row = database.Rows[i];
            T obj = new T();

            for (int j = 0; j < database.Columns.Count && j < row.Values.Count; j++)
            {
                string header = database.Columns[j].Name;
                if (string.IsNullOrEmpty(header))
                {
                    continue;
                }

                FieldInfo field = typeof(T).GetField(header);
                if (field == null)
                {
                    continue;
                }

                object value = ConvertValue(row.Values[j], field.FieldType);
                field.SetValue(obj, value);
            }

            result.Add(obj);
        }

        return result;
    }

    private static object ConvertValue(string value, Type type)
    {
        value = value.Trim();

        if (type == typeof(int))
            return int.Parse(value, CultureInfo.InvariantCulture);

        if (type == typeof(float))
            return float.Parse(value, CultureInfo.InvariantCulture);

        if (type == typeof(double))
            return double.Parse(value, CultureInfo.InvariantCulture);

        if (type == typeof(bool))
            return bool.Parse(value);

        if (type == typeof(string))
            return value;

        if (type.IsEnum)
            return Enum.Parse(type, value);

        return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
    }
}
