using UnityEngine;
using UnityEngine.UI;
using UnityEditor;


//      Custom Editor PreciseUI
//Class PreciseUICheck
[CustomEditor(typeof(PreciseUICheck)), CanEditMultipleObjects]
public class PreciseUICheckEditor : Editor
{
    private SerializedProperty alphaThreshold;
    private SerializedProperty includeMaterialAlpha;

    private void OnEnable()
    {
        alphaThreshold = serializedObject.FindProperty("AlphaThreshold");
        includeMaterialAlpha = serializedObject.FindProperty("IncludeMaterialAlpha");
    }

    public override void OnInspectorGUI()
    {
        var go = Selection.activeGameObject;
        if (go)
        {
            var image = go.GetComponent<Image>();
            if (image)
            {
                var path = AssetDatabase.GetAssetPath(image.mainTexture);
                if (path != string.Empty && !image.sprite.packed)
                {
                    var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (!textureImporter.isReadable)
                    {
                        EditorGUILayout.HelpBox("The texture is not readable. Alpha check won't have effect.", MessageType.Warning);
                        if (GUILayout.Button("FIX"))
                        {
                            textureImporter.isReadable = true;
                            AssetDatabase.ImportAsset(path);
                        }
                        return;
                    }
                }
                else if (!image.sprite.packed)
                {
                    EditorGUILayout.HelpBox("Assign a source image to the Image component to configure alpha checking.", MessageType.Warning);
                    return;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Can't find Image component. Alpha check is only possible for UI objects with an Image.", MessageType.Error);
                return;
            }
        }
        else return;

        serializedObject.Update();
        EditorGUILayout.PropertyField(alphaThreshold);
        EditorGUILayout.PropertyField(includeMaterialAlpha);
        serializedObject.ApplyModifiedProperties();
    }
}
