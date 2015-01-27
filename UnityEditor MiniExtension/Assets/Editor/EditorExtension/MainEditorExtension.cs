using UnityEngine;
using UnityEditor;

public class MainEditorExtension
{
    [MenuItem(EditorStrings.Setup.StringDeletePrefs,false,0)]
    private static void MenuDeletePrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem(EditorStrings.Setup.StringDeleteEditorPrefs, false, 1)]
    private static void MenuDeleteEditorPrefs()
    {
        EditorPrefs.DeleteAll();
    }
}

public class EditorStrings
{
    public class Setup
    {
        public const string StringDeletePrefs                               = "Tools/PlayerPrefs/Clear PlayerPrefs";
        public const string StringDeleteEditorPrefs                         = "Tools/PlayerPrefs/Clear EditorPrefs";
    }
}


