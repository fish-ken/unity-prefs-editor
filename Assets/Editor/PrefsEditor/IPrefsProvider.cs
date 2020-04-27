using System.Collections.Generic;

interface IPrefsProvider
{
    IEnumerable<PrefsPair> PlayerPrefs { get; }

    IEnumerable<PrefsPair> EditorPrefs { get; }
}
