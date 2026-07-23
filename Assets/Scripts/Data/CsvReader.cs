using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

public static class CsvReader
{
    public static List<T> Read<T>(string path) where T : new()
    {
        TextAsset csv = Resources.Load<TextAsset>(path);

        if (csv == null)
        {
            Debug.LogError($"CSV not found : {path}");
            return new List<T>();
        }

        string[] lines = csv.text.Split(
            new[] { "\r\n", "\n" },
            StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length <= 1)
            return new List<T>();

        string[] headers = lines[0].Split(',');
        

        List<T> result = new();
        Debug.Log($"Length : {lines.Length}");
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("//") || line.StartsWith("#"))
                continue;

            Debug.Log($"Line {i} : {line}");
            string[] values = line.Split(',');

            T obj = new T();

            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                string header = headers[j].Trim();

                FieldInfo field = typeof(T).GetField(header);

                if (field == null)
                    continue;

                object value = ConvertValue(values[j], field.FieldType);

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
            return int.Parse(value);

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

        return Convert.ChangeType(value, type);
    }
}
