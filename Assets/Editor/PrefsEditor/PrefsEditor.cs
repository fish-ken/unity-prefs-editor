using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PrefsEditor : EditorWindow
{
    [MenuItem("Tools/Utility/Player or Editor Prefs", priority = 14)]
    public static void ShowWindow()
    {
        var window = GetWindow<PrefsEditor>();
        window.minSize = new Vector2(760f, 300);
        window.Show();
    }

    private IPrefsProvider PrefsProvider = 
#if UNITY_EDITOR_WIN
                                            new WindowsPrefsProvider();
#elif UNITY_EDITOR_OSX
                                            null; // new OSXPrefsProvider();
#else
                                            null;
#endif

    private IEnumerable<PrefsPair> Prefs
    {
        get => prefsTab == 0 ? PrefsProvider.PlayerPrefs : PrefsProvider.EditorPrefs;
    }

    private int prefsTab = 0;
    private string searchField = "";
    private Vector2 scrollPos = Vector2.zero;
    private Dictionary<string, KeyValuePair<Type, string>> inputPrefs = new Dictionary<string, KeyValuePair<Type, string>>();

    private void OnGUI()
    {
        RenderTopTab();
        RenderQuickButton();
        RenderSearchField();
        RenderPrefs();
    }

    private void RenderQuickButton()
    {
        Color origin = GUI.color;

        GUILayout.BeginHorizontal();

        GUI.color = Color.green;
        if (GUILayout.Button("Save All"))
            SaveAll();

        GUI.color = Color.magenta;
        if (GUILayout.Button("Revert All"))
            RevertAll();

        GUI.color = Color.red;
        if (GUILayout.Button("Delete All"))
            DeleteAll();

        GUILayout.EndHorizontal();

        GUI.color = origin;
    }

    private void RenderTopTab()
    {
        int before = prefsTab;

        prefsTab = GUILayout.Toolbar(prefsTab, new string[] { "PlayerPrefs", "EditorPrefs" });

        if (before != prefsTab)
            inputPrefs.Clear();
    }

    private void RenderSearchField()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("Search", GUILayout.Width((int)(position.width * 0.3)));
        searchField = GUILayout.TextField(searchField);
        GUILayout.EndHorizontal();
    }

    private void RenderPrefs()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        Color originBackground = GUI.backgroundColor;
        Color originColor = GUI.color;

        int maxWidth = (int)position.width;
        int keyWidth = (int)(maxWidth * 0.25f);
        int typeWidth = (int)(maxWidth * 0.1f);
        int valueWidth = (int)(maxWidth * 0.40f);
        int behaviourWidth = (int)(maxWidth * 0.22f / 3f);

        foreach (var pair in Prefs)
        {
            if (string.IsNullOrWhiteSpace(searchField) == false)
            {
                bool show = pair.Key.Contains(searchField) ||
                            pair.Value.ToString().Contains(searchField) ||
                            pair.SimpleTypeString.Contains(searchField);

                if (show == false)
                    continue;
            }

            EditorGUILayout.BeginHorizontal();

            // Key
            GUILayout.Label(pair.Key, GUILayout.Width(keyWidth));

            // Type
            Type type = pair.Value.GetType();
            var typeStyle = new GUIStyle(GUI.skin.label);
            typeStyle.normal.textColor = pair.TypeColor;
            GUILayout.Label(pair.SimpleTypeString, typeStyle, GUILayout.Width(typeWidth));

            // Value
            bool isChanged = inputPrefs.ContainsKey(pair.Key);
            string value = isChanged ? inputPrefs[pair.Key].Value : pair.Value.ToString();

            if (isChanged)
                GUI.backgroundColor = Color.red;

            value = GUILayout.TextArea(value, GUILayout.Width(valueWidth));
            OnChangeValue(pair, value);
            GUI.backgroundColor = originBackground;

            // Behaviour
            GUI.color = Color.green;
            if (GUILayout.Button("Save", GUILayout.Width(behaviourWidth)))
            {
                if (inputPrefs.ContainsKey(pair.Key))
                {
                    bool result = Save(pair.Key, inputPrefs[pair.Key]);

                    if (result)
                        inputPrefs.Remove(pair.Key);
                }
            }

            GUI.color = Color.magenta;
            if (GUILayout.Button("Revert", GUILayout.Width(behaviourWidth)))
            {
                if (inputPrefs.ContainsKey(pair.Key))
                    Revert(pair.Key);
            }

            GUI.color = Color.red;
            if (GUILayout.Button("Delete", GUILayout.Width(behaviourWidth)))
                Delete(pair.Key);

            GUI.color = originColor;
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void OnChangeValue(PrefsPair pair, string value)
    {
        // Changed
        if (value != pair.Value.ToString())
        {
            if (pair.Value.ToString() != value)
                inputPrefs[pair.Key] = new KeyValuePair<Type, string>(pair.Type, value);
        }
        else
        {
            inputPrefs.Remove(pair.Key);
        }
    }

    private HashSet<string> savedKey = new HashSet<string>();
    private void SaveAll()
    {
        foreach (var pair in inputPrefs)
        {
            if (Save(pair.Key, pair.Value))
                savedKey.Add(pair.Key);
        }

        foreach (var key in savedKey)
            inputPrefs.Remove(key);
    }

    private bool Save(string key, KeyValuePair<Type, string> pair)
    {
        bool result = false;
        string typeString = pair.Key.ToString();

        if (prefsTab == 0)
        {
            switch (typeString)
            {
                case PrefsPair.IntString:
                    {
                        result = int.TryParse(pair.Value.ToString(), out int value);

                        if (result)
                            PlayerPrefs.SetInt(key, value);
                        break;
                    }
                case PrefsPair.StringString:
                    {
                        result = true;
                        PlayerPrefs.SetString(key, pair.Value);
                        break;
                    }
                case PrefsPair.FloatString:
                    {
                        result = float.TryParse(pair.Value.ToString(), out float value);

                        if (result)
                            PlayerPrefs.SetFloat(key, value);
                        break;
                    }
            }
        }
        else
        {
            switch (typeString)
            {
                case PrefsPair.IntString:
                    {
                        result = int.TryParse(pair.Value.ToString(), out int value);

                        if (result)
                            EditorPrefs.SetInt(key, value);
                    }
                    break;

                case PrefsPair.StringString:
                    {
                        result = true;
                        EditorPrefs.SetString(key, pair.Value);
                        break;

                    }

                case PrefsPair.FloatString:
                    {
                        result = float.TryParse(pair.Value.ToString(), out float value);

                        if (result)
                            EditorPrefs.SetFloat(key, value);
                    }
                    break;

                case PrefsPair.BoolString:
                    {
                        result = bool.TryParse(pair.Value.ToString(), out bool value);

                        if (result)
                            EditorPrefs.SetBool(key, value);
                        break;
                    }
            }
        }

        return result;
    }

    private void DeleteAll()
    {
        foreach (var pair in Prefs)
            Delete(pair.Key);
    }

    private void Delete(string key)
    {
        if (prefsTab == 0)
            PlayerPrefs.DeleteKey(key);
        else
            EditorPrefs.DeleteKey(key);
    }

    private void RevertAll()
    {
        inputPrefs.Clear();
    }

    private void Revert(string key)
    {
        if (inputPrefs.ContainsKey(key) == false)
            return;

        inputPrefs.Remove(key);
    }
}
