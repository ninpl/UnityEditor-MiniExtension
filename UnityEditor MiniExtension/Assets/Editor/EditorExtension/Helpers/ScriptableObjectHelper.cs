using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

//      Helper class ScriptableObjects
//Helper class for instantiating ScriptableObjects.
public class ScriptableObjectHelper
{
    [MenuItem(EditorStrings.Setup.StringScriptableObject1)]
    [MenuItem(EditorStrings.Setup.StringScriptableObject2)]
    public static void Create()
    {
        var assembly = GetAssembly();
        var allScriptableObjects = (from t in assembly.GetTypes()
                                    where t.IsSubclassOf(typeof(ScriptableObject))
                                    select t).ToArray();

        var window = EditorWindow.GetWindow<ScriptableObjectEditorWindow>(true, "Create a new ScriptableObject", true);
        window.ShowPopup();

        window.Types = allScriptableObjects;
    }

    private static Assembly GetAssembly()
    {
        return Assembly.Load(new AssemblyName("Assembly-CSharp"));
    }
}


