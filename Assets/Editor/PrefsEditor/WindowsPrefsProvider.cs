#if UNITY_EDITOR_WIN

using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

public class WindowsPrefsProvider : IPrefsProvider
{
    private static Encoding encoding = new UTF8Encoding();

    private string PlayerPrefsPath
    {
        get
        {
#if UNITY_5_5_OR_NEWER
            return string.Format("Software\\Unity\\UnityEditor\\{0}\\{1}", Application.companyName, Application.productName);
#else
            return string.Format("Software\\{0}\\{1}", Application.companyName, Application.productName);
#endif
        }
    }

    private string EditorPrefsPath
    {
        get
        {
            return "Software\\Unity Technologies\\Unity Editor 5.x";
        }
    }

    IEnumerable<PrefsPair> IPrefsProvider.PlayerPrefs => GetPrefs(PlayerPrefsPath);

    IEnumerable<PrefsPair> IPrefsProvider.EditorPrefs => GetPrefs(EditorPrefsPath);

    private IEnumerable<PrefsPair> GetPrefs(string path)
    {
        var isEditorPrefs = path == EditorPrefsPath;
        var registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path);

        if (registryKey != null)
        {
            // Get an array of what keys (registry value names) are stored
            var valueNames = registryKey.GetValueNames();
            var prefs = new List<PrefsPair>(valueNames.Length);

            foreach (var subKey in valueNames)
            {
                // Remove the _h193410979 style suffix used on PlayerPref keys in Windows registry
                int index = subKey.LastIndexOf("_");
                string key = subKey.Remove(index, subKey.Length - index);

                // Get the value from the registry
                var ambiguousValue = registryKey.GetValue(subKey);

                // Unfortunately floats will come back as an int (at least on 64 bit) because the float is stored as
                // 64 bit but marked as 32 bit - which confuses the GetValue() method greatly! 
                if (ambiguousValue.GetType() == typeof(int))
                {
                    // If the PlayerPref is not actually an int then it must be a float, this will evaluate to true
                    // (impossible for it to be 0 and -1 at the same time)

                    // Not int
                    if (GetInt(key, -1, isEditorPrefs) == -1 && GetInt(key, 0, isEditorPrefs) == 0)
                    {
                        // Fetch the float value from PlayerPrefs in memory
                        ambiguousValue = GetFloat(key, 0f, isEditorPrefs);
                    }
                    else if(isEditorPrefs)
                    {
                        int value = (int)ambiguousValue;

                        // bool ?
                        if (value == 0 || value == 1)
                            ambiguousValue = Convert.ToBoolean(value);
                    }
                }
                else if (ambiguousValue.GetType() == typeof(byte[]))
                {
                    // On Unity 5 a string may be stored as binary, so convert it back to a string
                    ambiguousValue = encoding.GetString((byte[])ambiguousValue).TrimEnd('\0');
                }

                prefs.Add(new PrefsPair() { Key = key, Value = ambiguousValue });
            }

            // Return the results
            return prefs;
        }
        else
        {
            return new PrefsPair[0];
        }
    }

    private bool GetBool(string key, bool defaultValue = false, bool isEditorPrefs = false)
    {
        if (isEditorPrefs)
            return EditorPrefs.GetBool(key, defaultValue);
        else
            return false;
    }

    private int GetInt(string key, int defaultValue = 0, bool isEditorPrefs = false)
    {
        if (isEditorPrefs)
            return EditorPrefs.GetInt(key, defaultValue);
        else
            return PlayerPrefs.GetInt(key, defaultValue);
    }

    private float GetFloat(string key, float defaultValue = 0f, bool isEditorPrefs = false)
    {
        if (isEditorPrefs)
            return EditorPrefs.GetFloat(key, defaultValue);
        else
            return PlayerPrefs.GetFloat(key, defaultValue);
    }
}

#endif