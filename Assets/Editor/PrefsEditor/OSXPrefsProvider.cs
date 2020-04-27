/// XXX : NOT WORKING


//using System;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;

//#if UNITY_EDITOR_OSX

//public class OSXPrefsProvider : IPrefsProvider
//{
//    private string PlayerPrefsPath
//    {
//        get
//        {
//            string plistFilename = string.Format("unity.{0}.{1}.plist", Application.companyName, Application.productName);
//            return Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library/Preferences"), plistFilename);
//        }
//    }

//    private string EditorPrefsPath
//    {
//        get
//        {
//            var majorVersion = Application.unityVersion.Split('.')[0];
//            return Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library/Preferences"), "com.unity3d.UnityEditor" + majorVersion + ".x.plist");
//        }
//    }

//    public IEnumerable<PrefsPair> EditorPrefs => GetPrefs(EditorPrefsPath);

//    public IEnumerable<PrefsPair> PlayerPrefs => GetPrefs(PlayerPrefsPath);

//    private IEnumerable<PrefsPair> GetPrefs(string path)
//    {
//        if (File.Exists(path))
//        {
//            var plist = Plist.readPlist(path);
//            var parsed = plist as Dictionary<string, object>;
//            var prefs = new List<PrefsPair>(parsed.Count);

//            foreach (var pair in parsed)
//            {
//                if (pair.Value.GetType() == typeof(double))
//                {
//                    // Some float values may come back as double, so convert them back to floats
//                    prefs.Add(new PrefsPair() { Key = pair.Key, Value = (float)(double)pair.Value });
//                }
//                else if (pair.Value.GetType() == typeof(bool))
//                {
//                    // PlayerPrefs API doesn't allow bools, so ignore them
//                }
//                else
//                {
//                    prefs.Add(new PrefsPair() { Key = pair.Key, Value = pair.Value });
//                }
//            }

//            return prefs.ToArray();
//        }
//        else
//        {
//            return new PrefsPair[0];
//        }
//    }
//}

//#endif

