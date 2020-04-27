using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrefsTester : MonoBehaviour
{
    private void Start()
    {
        var gen = new System.Random();

        for (int i = 0; i < 100; i++)
        {
            PlayerPrefs.SetInt(string.Format("Int Key {0}", i), gen.Next());
            PlayerPrefs.SetFloat(string.Format("Flaot Key {0}", i), (float)gen.NextDouble());
            PlayerPrefs.SetString(string.Format("String Key {0}", i), gen.Next().ToString());

            EditorPrefs.SetInt(string.Format("Int Key {0}", i), gen.Next());
            EditorPrefs.SetFloat(string.Format("Flaot Key {0}", i), (float)gen.NextDouble());
            EditorPrefs.SetString(string.Format("String Key {0}", i), gen.Next().ToString());
        }
    }
}
