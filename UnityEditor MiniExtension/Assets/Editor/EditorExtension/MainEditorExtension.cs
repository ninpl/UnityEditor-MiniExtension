/*  
    Autor: Antonio Mateo Tomas (lPinchol)
    Date: 14/04/2015
    GitHub: https://github.com/lPinchol
*/

using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

//      Main Menu
//Class menus with different extensions
public class MainEditorExtension : MonoBehaviour
{
    //MENUS

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

    [MenuItem(EditorStrings.Setup.StringScreenShotInstant, false, 2)]
    public static void MenuScreenShot()
    {
        ScreemShotInstant.SendCall = true;
        Load();
    }

    [MenuItem(EditorStrings.Setup.StringScreenShotSettings, false, 3)]
    public static void MenuScreenShotSettings()
    {
        EditorWindow editorWindow = EditorWindow.GetWindow(typeof(ScreemShotInstant));
        editorWindow.autoRepaintOnSceneChange = true;
        editorWindow.Show();
        editorWindow.title = "Screenshot";
        Load();
    }

    [MenuItem(EditorStrings.Setup.StringMenuRayCast, false, 4)]
    public static void MenuRaycast()
    {
        Load();
        if (Selection.activeGameObject != null)
        {
            if (Selection.activeGameObject.name == "RayCast Debug")
            {
                addNewRayCastDebug(Selection.activeGameObject);
            }
            else
            {
                if (GameObject.Find("RayCast Debug") != null)
                {
                    EditorUtility.DisplayDialog(EditorStrings.RayCastDebug.StringRayCastDebugWarning, EditorStrings.RayCastDebug.StringNeedRayCast, EditorStrings.RayCastDebug.StringOK);
                }
                else
                {
                    createNewRayCastDebug();
                }
            }
        }
        else
        {
            if (GameObject.Find("RayCast Debug") != null)
            {
                addNewRayCastDebug(GameObject.Find("RayCast Debug"));
            }
            else
            {
                createNewRayCastDebug();
            }
        }
    }

    //Class Scriptable Object Menu
    public class ScriptableObjectMenu
    {
        [MenuItem(EditorStrings.Setup.StringScriptableObject1,false,5)]
        [MenuItem(EditorStrings.Setup.StringScriptableObject2)]
        public static void MenuScriptableObject()
        {
            Load();
            var assembly = GetAssembly();
            var allScriptableObjects = (from t in assembly.GetTypes()
                                        where t.IsSubclassOf(typeof(ScriptableObject))
                                        select t).ToArray();

            var window = EditorWindow.GetWindow<ScriptableObjectWindows>(true, EditorStrings.ScriptableObject.StringScriptableObjectNewScripta, true);
            window.ShowPopup();

            window.Types = allScriptableObjects;
        }

        private static Assembly GetAssembly()
        {
            return Assembly.Load(new AssemblyName("Assembly-CSharp"));
        }
    }

    [MenuItem(EditorStrings.Setup.StringAbout, false, 99)]
    public static void MenuAboutWin()
    {
        Load();
        AboutWindows.Init(true);
    }

    [MenuItem(EditorStrings.Setup.StringResourceChecked,false,5)]
    static void MenuResourceChecker()
    {
        ResourceChecker window = (ResourceChecker)EditorWindow.GetWindow(typeof(ResourceChecker));
        window.CheckResources();
        window.minSize = new Vector2(ResourceChecker.MinWidth2, 300);
    }

    [MenuItem(EditorStrings.Setup.StringLocalizations, false, 6)]
    static void MenuLocalizations()
    {
        EditorWindow editorWindow = EditorWindow.GetWindow(typeof(Localizations));
        editorWindow.autoRepaintOnSceneChange = true;
        editorWindow.Show();
        editorWindow.title = "Localizations";
    }


    //LANGUAGES

    [MenuItem(EditorStrings.Setup.StringMenuLanguageEnglish, false, 100)]
    public static void MenuLanguageEnglish()
    {
        EditorTranslator.English();
        EditorStrings.Data._Languaje = "ENG";
        EditorPrefs.SetString("EDITORLanguaje", EditorStrings.Data._Languaje);
    }

    [MenuItem(EditorStrings.Setup.StringMenuLanguageSpanish, false, 101)]
    public static void MenuLanguageSpanish()
    {
        EditorTranslator.Spanish();
        EditorStrings.Data._Languaje = "ESP";
        EditorPrefs.SetString("EDITORLanguaje", EditorStrings.Data._Languaje);
    }


    //COMPONENT

    [MenuItem(EditorStrings.Setup.StringComponentRayCast, false, 5)]
    public static void ComponentRaycast()
    {
        Load();
        if (Selection.activeGameObject != null)
        {
            if (Selection.activeGameObject.name == "RayCast Debug")
            {
                addNewRayCastDebug(Selection.activeGameObject);
            }
            else
            {
                if (GameObject.Find("RayCast Debug") != null)
                {
                    EditorUtility.DisplayDialog(EditorStrings.RayCastDebug.StringRayCastDebugWarning, EditorStrings.RayCastDebug.StringNeedRayCast, EditorStrings.RayCastDebug.StringOK);
                }
                else
                {
                    createNewRayCastDebug();
                }
            }
        }
        else
        {
            if (GameObject.Find("RayCast Debug") != null)
            {
                addNewRayCastDebug(GameObject.Find("RayCast Debug"));
            }
            else
            {
                createNewRayCastDebug();
            }
        }
    }


    //EVENTS
    private static void createNewRayCastDebug()
    {
        GameObject go = new GameObject("RayCast Debug");
        go.transform.position = Vector3.zero;
        go.AddComponent(typeof(RayCastDebug));
        Debug.Log(EditorStrings.RayCastDebug.StringDebug01);
    }
    private static void addNewRayCastDebug(GameObject go)
    {
        go.AddComponent(typeof(RayCastDebug));
        Debug.Log(EditorStrings.RayCastDebug.StringDebug02);
    }

    public static void Load()
    {
        EditorPrefs.GetString("EDITORLanguaje");
    }
}

//      Editor extension screenshot
//Class that creates a screenshot from the editor
[ExecuteInEditMode]
public class ScreemShotInstant : EditorWindow
{

    public static bool SendCall                 = false;

    public string lastScreenshot                = "";
    public Camera Camera;

    private float lastTime;
    private int resWidth                        = Screen.width * 4;
    private int resHeight                       = Screen.height * 4;
    private bool takeHiResShot                  = false;
    private int scale                           = 1;
    private string path                         = "";
    private RenderTexture renderTexture;
    private bool isTransparent                  = false;
    private bool isViewSettings                 = false;
    private string nameScreen                   = "";

    void Update()
    {
        if (SendCall == true)
        {
            if (path == "")
            {
                path = EditorUtility.SaveFolderPanel(EditorStrings.ScreenShot.StringPathImages, path, Application.dataPath);
                Debug.Log(EditorStrings.ScreenShot.StringDebug01);
                TakeHiResShot();
            }
            else
            {
                TakeHiResShot();
            }
            SendCall = false;
            return;
        }
    }

    void OnGUI()
    {

#if UNITY_FREE_LICENSE
        GUI.backgroundColor = Color.red;
        EditorGUILayout.HelpBox("This is a feature of Unity PRO", MessageType.Warning);
#endif

#if UNITY_PRO_LICENSE

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(EditorStrings.ScreenShot.StringResolution, EditorStyles.boldLabel);
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button(EditorStrings.ScreenShot.StringWindows))
        {
            ChangeResolutionView();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginVertical();
        if (isViewSettings)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(EditorStrings.ScreenShot.StringResSettings, EditorStyles.boldLabel);
            if (GUILayout.Button(EditorStrings.ScreenShot.StringSetResScreen))
            {
                resHeight = (int)Handles.GetMainGameViewSize().y;
                resWidth = (int)Handles.GetMainGameViewSize().x;

            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(EditorStrings.ScreenShot.StringNameSettings, EditorStyles.boldLabel);
            if (GUILayout.Button(EditorStrings.ScreenShot.StringSetNameDefaut))
            {
                nameScreen = EditorStrings.ScreenShot.StringNameScreen;

            }

            EditorGUILayout.EndHorizontal();

            nameScreen = EditorGUILayout.TextField(EditorStrings.ScreenShot.StringNameScreenShot, nameScreen);

            EditorGUILayout.Space();

            resWidth = EditorGUILayout.IntField(EditorStrings.ScreenShot.StringWidth, resWidth);
            resHeight = EditorGUILayout.IntField(EditorStrings.ScreenShot.StringHeight, resHeight);

            EditorGUILayout.Space();

            scale = EditorGUILayout.IntSlider(EditorStrings.ScreenShot.StringScaleScreen, scale, 1, 15);

            EditorGUILayout.Space();

            GUILayout.Label(EditorStrings.ScreenShot.StringPathSettings, EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.TextField(path, GUILayout.ExpandWidth(false));
            if (GUILayout.Button(EditorStrings.ScreenShot.StringBrowse, GUILayout.ExpandWidth(false)))
            {
                path = EditorUtility.SaveFolderPanel(EditorStrings.ScreenShot.StringPath, path, Application.dataPath);
            }
                
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(EditorStrings.ScreenShot.StringCameraSettings, EditorStyles.boldLabel);
            GUI.backgroundColor = Color.green;
            Camera = EditorGUILayout.ObjectField(Camera, typeof(Camera), true, null) as Camera;
            if (GUILayout.Button(EditorStrings.ScreenShot.StringSelectCameraScene))
            {
                Camera = Camera.main;
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();

            if (Camera == null)
            {
                Camera = Camera.main;
            }

            EditorGUILayout.Space();

            GUILayout.Label(EditorStrings.ScreenShot.StringTextureSettings, EditorStyles.boldLabel);
            isTransparent = EditorGUILayout.Toggle(EditorStrings.ScreenShot.StringTransparentBackground, isTransparent);

            EditorGUILayout.Space();
        }
        

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField(EditorStrings.ScreenShot.StringDefaultOptions, EditorStyles.boldLabel);


        if (GUILayout.Button(EditorStrings.ScreenShot.StringSetToScreenSize))
        {
            resHeight = (int)Handles.GetMainGameViewSize().y;
            resWidth = (int)Handles.GetMainGameViewSize().x;

        }


        if (GUILayout.Button(EditorStrings.ScreenShot.StringDefaultSize))
        {
            resHeight = 1080;
            resWidth = 1980;
            scale = 1;
        }



        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(EditorStrings.ScreenShot.StringScreenshotres + resWidth * scale + EditorStrings.ScreenShot.StringX + resHeight * scale + EditorStrings.ScreenShot.StringPX, EditorStyles.boldLabel);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button(EditorStrings.ScreenShot.StringTakeScreenShot, GUILayout.MinHeight(60)))
        {
            if (path == "")
            {
                path = EditorUtility.SaveFolderPanel(EditorStrings.ScreenShot.StringPathToSaveImages, path, Application.dataPath);
                Debug.Log(EditorStrings.ScreenShot.StringDebug01);
                TakeHiResShot();
            }
            else
            {
                TakeHiResShot();
            }
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.LabelField(EditorStrings.ScreenShot.StringScreenShotName + nameScreen + " ]", EditorStyles.boldLabel);

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button(EditorStrings.ScreenShot.StringOpenLastScreenshot, GUILayout.MinHeight(40)))
        {
            if (lastScreenshot != "")
            {
                Application.OpenURL("file://" + lastScreenshot);
                Debug.Log(EditorStrings.ScreenShot.StringDebug02 + lastScreenshot);
            }
        }

        if (GUILayout.Button(EditorStrings.ScreenShot.StringOpenFolder, GUILayout.MinHeight(40)))
        {

            Application.OpenURL("file://" + path);
        }

        EditorGUILayout.EndHorizontal();


        if (takeHiResShot)
        {
            int resWidthN = resWidth * scale;
            int resHeightN = resHeight * scale;
            RenderTexture rt = new RenderTexture(resWidthN, resHeightN, 24);
            Camera.targetTexture = rt;

            TextureFormat tFormat;
            if (isTransparent)
                tFormat = TextureFormat.ARGB32;
            else
                tFormat = TextureFormat.RGB24;


            Texture2D screenShot = new Texture2D(resWidthN, resHeightN, tFormat, false);
            Camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidthN, resHeightN), 0, 0);
            Camera.targetTexture = null;
            RenderTexture.active = null;
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidthN, resHeightN);

            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("{e}[ScreenShot] Took screenshot to: {0}", filename));
            Application.OpenURL(filename);
            takeHiResShot = false;
        }

#endif

    }

    public string ScreenShotName(int width, int height)
    {

        string strPath = "";

        strPath = string.Format("{0}/" + nameScreen +"_{1}x{2}_{3}.png",
                             path,
                             width, height,
                                       System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        lastScreenshot = strPath;

        return strPath;
    }

    public void TakeHiResShot()
    {
        Debug.Log(EditorStrings.ScreenShot.StringDebug03);
        takeHiResShot = true;
    }

    private void ChangeResolutionView()
    {
        if (isViewSettings == true)
        {
            isViewSettings = false;
            return;
        }
        if (isViewSettings == false)
        {
            isViewSettings = true;
            return;
        }
    }

}

//      Editor extension RayCast
//class to create a raycash and check the distance
[ExecuteInEditMode]
[CustomEditor(typeof(RayCastDebug))]
public class RayCastDebugEditor : Editor
{
    RayCastDebug _target;
    GUIStyle style = new GUIStyle();
    public static int count = 0;

    void OnEnable()
    {
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        _target = (RayCastDebug)target;
        if (!_target.initialized)
        {
            _target.initialized = true;
            _target.rayCastDebugName = "RayCast Debug " + ++count;
            _target.initialName = _target.rayCastDebugName;
        }
    }

    public override void OnInspectorGUI()
    {

        if (_target.rayCastDebugName == "")
        {
            _target.rayCastDebugName = _target.initialName;
        }

        EditorGUILayout.BeginVertical();

        EditorGUILayout.PrefixLabel(EditorStrings.RayCastDebug.StringName);
        _target.rayCastDebugName = EditorGUILayout.TextField(_target.rayCastDebugName, GUILayout.ExpandWidth(false));

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        EditorGUILayout.PrefixLabel(EditorStrings.RayCastDebug.StringGizmoSize);
        _target.gizmoRadius = Mathf.Clamp(EditorGUILayout.Slider(_target.gizmoRadius, 0.1f, 3.0f, GUILayout.ExpandWidth(false)), 0.1f, 100);

        EditorGUILayout.Separator();

        EditorGUILayout.PrefixLabel(EditorStrings.RayCastDebug.StringColorLine);
        _target.lineColor = EditorGUILayout.ColorField(_target.lineColor, GUILayout.ExpandWidth(false));

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        _target.scaleToPixels = EditorGUILayout.Toggle(EditorStrings.RayCastDebug.StringShowScale, _target.scaleToPixels, GUILayout.ExpandWidth(false));

        _target.pixelPerUnit = EditorGUILayout.IntField(EditorStrings.RayCastDebug.StringPixelUnit, _target.pixelPerUnit, GUILayout.ExpandWidth(false));

        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(_target);
        }
    }

    void OnSceneGUI()
    {
        Undo.RecordObject(_target, "rayCast debug undo");
        float distance = Vector3.Distance(_target.startPoint, _target.endPoint);
        float scalePerPixel = distance * _target.pixelPerUnit;

        if (_target.scaleToPixels)
        {
            Handles.Label(_target.endPoint, EditorStrings.RayCastDebug.StringAuxInfo1 + distance + EditorStrings.RayCastDebug.StringAuxInfo2 + scalePerPixel + EditorStrings.RayCastDebug.StrinPX, style);
        }
        else
        {
            Handles.Label(_target.endPoint, EditorStrings.RayCastDebug.StringAuxInfo1 + distance, style);
        }
        _target.startPoint = Handles.PositionHandle(_target.startPoint, Quaternion.identity);
        _target.endPoint = Handles.PositionHandle(_target.endPoint, Quaternion.identity);
    }
}

//      Editor ScriptableObjects
//Class Scriptable objects windows
public class ScriptableObjectWindows : EditorWindow
{
    private int selectedIndex;
    private string[] names;
    private Type[] types;

    public Type[] Types
    {
        get { return types; }
        set
        {
            types = value;
            names = types.Select(t => t.FullName).ToArray();
        }
    }

    public void OnGUI()
    {
        GUILayout.Label(EditorStrings.ScriptableObject.StringScriptableObjectClass);
        selectedIndex = EditorGUILayout.Popup(selectedIndex, names);

        if (GUILayout.Button(EditorStrings.ScriptableObject.StringScriptableObjectCreate))
        {
            var asset = ScriptableObject.CreateInstance(types[selectedIndex]);
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                asset.GetInstanceID(),
                ScriptableObject.CreateInstance<EndNameEdit>(),
                string.Format("{0}.asset", names[selectedIndex]),
                AssetPreview.GetMiniThumbnail(asset),
                null);

            Debug.Log(EditorStrings.ScriptableObject.StringScriptableObjectDebug01);
            Close();
        }
    }
}

//      Editor ResourceChecker
//Class Editor ResourceChecker
public class ResourceChecker : EditorWindow
{
    string[] inspectToolbarStrings = { "Textures", "Materials", "Meshes" };

    enum InspectType
    {
        Textures, Materials, Meshes
    };

    InspectType ActiveInspectType = InspectType.Textures;

    float ThumbnailWidth = 40;
    float ThumbnailHeight = 40;

    List<TextureDetails> ActiveTextures = new List<TextureDetails>();
    List<MaterialDetails> ActiveMaterials = new List<MaterialDetails>();
    List<MeshDetails> ActiveMeshDetails = new List<MeshDetails>();

    Vector2 textureListScrollPos = new Vector2(0, 0);
    Vector2 materialListScrollPos = new Vector2(0, 0);
    Vector2 meshListScrollPos = new Vector2(0, 0);

    int TotalTextureMemory = 0;
    int TotalMeshVertices = 0;

    bool ctrlPressed = false;

    static int MinWidth = 455;
    public static int MinWidth2 = MinWidth;

    void OnGUI()
    {
        if (GUILayout.Button("Refresh")) CheckResources();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Materials " + ActiveMaterials.Count);
        GUILayout.Label("Textures " + ActiveTextures.Count + " - " + FormatSizeString(TotalTextureMemory));
        GUILayout.Label("Meshes " + ActiveMeshDetails.Count + " - " + TotalMeshVertices + " verts");
        GUILayout.EndHorizontal();
        ActiveInspectType = (InspectType)GUILayout.Toolbar((int)ActiveInspectType, inspectToolbarStrings);

        ctrlPressed = Event.current.control || Event.current.command;

        switch (ActiveInspectType)
        {
            case InspectType.Textures:
                ListTextures();
                break;
            case InspectType.Materials:
                ListMaterials();
                break;
            case InspectType.Meshes:
                ListMeshes();
                break;
        }
    }

    int GetBitsPerPixel(TextureFormat format)
    {
        switch (format)
        {
            case TextureFormat.Alpha8:          //  Alpha-only texture format.
                return 8;
            case TextureFormat.ARGB4444:        //  A 16 bits/pixel texture format. Texture stores color with an alpha channel.
                return 16;
            case TextureFormat.RGBA4444:        //  A 16 bits/pixel texture format.
                return 16;
            case TextureFormat.RGB24:	        //  A color texture format.
                return 24;
            case TextureFormat.RGBA32:	        //  Color with an alpha channel texture format.
                return 32;
            case TextureFormat.ARGB32:	        //  Color with an alpha channel texture format.
                return 32;
            case TextureFormat.RGB565:	        //  A 16 bit color texture format.
                return 16;
            case TextureFormat.DXT1:	        //  Compressed color texture format.
                return 4;
            case TextureFormat.DXT5:	        //  Compressed color with alpha channel texture format.
                return 8;
            case TextureFormat.PVRTC_RGB2:      //  PowerVR (iOS) 2 bits/pixel compressed color texture format.
                return 2;
            case TextureFormat.PVRTC_RGBA2:     //  PowerVR (iOS) 2 bits/pixel compressed with alpha channel texture format
                return 2;
            case TextureFormat.PVRTC_RGB4:      //  PowerVR (iOS) 4 bits/pixel compressed color texture format.
                return 4;
            case TextureFormat.PVRTC_RGBA4:     //  PowerVR (iOS) 4 bits/pixel compressed with alpha channel texture format
                return 4;
            case TextureFormat.ETC_RGB4:        //  ETC (GLES2.0) 4 bits/pixel compressed RGB texture format.
                return 4;
            case TextureFormat.ATC_RGB4:        //  ATC (ATITC) 4 bits/pixel compressed RGB texture format.
                return 4;
            case TextureFormat.ATC_RGBA8:       //  ATC (ATITC) 8 bits/pixel compressed RGB texture format.
                return 8;
            case TextureFormat.BGRA32:          //  Format returned by iPhone camera
                return 32;
#if UNITY_4_6 || UNITY_4_6_1
            case TextureFormat.ATF_RGB_DXT1:    //  Flash-specific RGB DXT1 compressed color texture format.
            case TextureFormat.ATF_RGBA_JPG:    //  Flash-specific RGBA JPG-compressed color texture format.
            case TextureFormat.ATF_RGB_JPG:     //  Flash-specific RGB JPG-compressed color texture format.
                return 0;                       //  Not supported yet
#endif
        }
        return 0;
    }

    int CalculateTextureSizeBytes(Texture tTexture)
    {
        int tWidth = tTexture.width;
        int tHeight = tTexture.height;
        if (tTexture is Texture2D)
        {
            Texture2D tTex2D = tTexture as Texture2D;
            int bitsPerPixel = GetBitsPerPixel(tTex2D.format);
            int mipMapCount = tTex2D.mipmapCount;
            int mipLevel = 1;
            int tSize = 0;
            while (mipLevel <= mipMapCount)
            {
                tSize += tWidth * tHeight * bitsPerPixel / 8;
                tWidth = tWidth / 2;
                tHeight = tHeight / 2;
                mipLevel++;
            }
            return tSize;
        }

        if (tTexture is Cubemap)
        {
            Cubemap tCubemap = tTexture as Cubemap;
            int bitsPerPixel = GetBitsPerPixel(tCubemap.format);
            return tWidth * tHeight * 6 * bitsPerPixel / 8;
        }
        return 0;
    }

    void SelectObject(UnityEngine.Object selectedObject, bool append)
    {
        if (append)
        {
            List<UnityEngine.Object> currentSelection = new List<UnityEngine.Object>(Selection.objects);
            if (currentSelection.Contains(selectedObject)) currentSelection.Remove(selectedObject);
            else currentSelection.Add(selectedObject);

            Selection.objects = currentSelection.ToArray();
        }
        else Selection.activeObject = selectedObject;
    }

    void SelectObjects(List<UnityEngine.Object> selectedObjects, bool append)
    {
        if (append)
        {
            List<UnityEngine.Object> currentSelection = new List<UnityEngine.Object>(Selection.objects);
            currentSelection.AddRange(selectedObjects);
            Selection.objects = currentSelection.ToArray();
        }
        else Selection.objects = selectedObjects.ToArray();
    }

    string FormatSizeString(int memSizeKB)
    {
        if (memSizeKB < 1024) return "" + memSizeKB + "k";
        else
        {
            float memSizeMB = ((float)memSizeKB) / 1024.0f;
            return memSizeMB.ToString("0.00") + "Mb";
        }
    }

    public void CheckResources()
    {
        ActiveTextures.Clear();
        ActiveMaterials.Clear();
        ActiveMeshDetails.Clear();

        Renderer[] renderers = (Renderer[])FindObjectsOfType(typeof(Renderer));
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.sharedMaterials)
            {

                MaterialDetails tMaterialDetails = FindMaterialDetails(material);
                if (tMaterialDetails == null)
                {
                    tMaterialDetails = new MaterialDetails();
                    tMaterialDetails.material = material;
                    ActiveMaterials.Add(tMaterialDetails);
                }
                tMaterialDetails.FoundInRenderers.Add(renderer);
            }
        }

        foreach (MaterialDetails tMaterialDetails in ActiveMaterials)
        {
            Material tMaterial = tMaterialDetails.material;
            var dependencies = EditorUtility.CollectDependencies(new UnityEngine.Object[] { tMaterial });
            foreach (UnityEngine.Object obj in dependencies)
            {
                if (obj is Texture)
                {
                    Texture tTexture = obj as Texture;
                    var tTextureDetail = GetTextureDetail(tTexture, tMaterial, tMaterialDetails);
                    ActiveTextures.Add(tTextureDetail);
                }
            }

            if (tMaterial.mainTexture != null && !dependencies.Contains(tMaterial.mainTexture))
            {
                var tTextureDetail = GetTextureDetail(tMaterial.mainTexture, tMaterial, tMaterialDetails);
                ActiveTextures.Add(tTextureDetail);
            }
        }

        MeshFilter[] meshFilters = (MeshFilter[])FindObjectsOfType(typeof(MeshFilter));

        foreach (MeshFilter tMeshFilter in meshFilters)
        {
            Mesh tMesh = tMeshFilter.sharedMesh;
            if (tMesh != null)
            {
                MeshDetails tMeshDetails = FindMeshDetails(tMesh);
                if (tMeshDetails == null)
                {
                    tMeshDetails = new MeshDetails();
                    tMeshDetails.mesh = tMesh;
                    ActiveMeshDetails.Add(tMeshDetails);
                }
                tMeshDetails.FoundInMeshFilters.Add(tMeshFilter);
            }
        }

        SkinnedMeshRenderer[] skinnedMeshRenderers = (SkinnedMeshRenderer[])FindObjectsOfType(typeof(SkinnedMeshRenderer));

        foreach (SkinnedMeshRenderer tSkinnedMeshRenderer in skinnedMeshRenderers)
        {
            Mesh tMesh = tSkinnedMeshRenderer.sharedMesh;
            if (tMesh != null)
            {
                MeshDetails tMeshDetails = FindMeshDetails(tMesh);
                if (tMeshDetails == null)
                {
                    tMeshDetails = new MeshDetails();
                    tMeshDetails.mesh = tMesh;
                    ActiveMeshDetails.Add(tMeshDetails);
                }
                tMeshDetails.FoundInSkinnedMeshRenderer.Add(tSkinnedMeshRenderer);
            }
        }

        TotalTextureMemory = 0;
        foreach (TextureDetails tTextureDetails in ActiveTextures) TotalTextureMemory += tTextureDetails.memSizeKB;

        TotalMeshVertices = 0;
        foreach (MeshDetails tMeshDetails in ActiveMeshDetails) TotalMeshVertices += tMeshDetails.mesh.vertexCount;

        ActiveTextures.Sort(delegate(TextureDetails details1, TextureDetails details2) { return details2.memSizeKB - details1.memSizeKB; });
        ActiveMeshDetails.Sort(delegate(MeshDetails details1, MeshDetails details2) { return details2.mesh.vertexCount - details1.mesh.vertexCount; });
    }

    private TextureDetails GetTextureDetail(Texture tTexture, Material tMaterial, MaterialDetails tMaterialDetails)
    {
        TextureDetails tTextureDetails = FindTextureDetails(tTexture);
        if (tTextureDetails == null)
        {
            tTextureDetails = new TextureDetails();
            tTextureDetails.texture = tTexture;
            tTextureDetails.isCubeMap = tTexture is Cubemap;

            int memSize = CalculateTextureSizeBytes(tTexture);

            tTextureDetails.memSizeKB = memSize / 1024;
            TextureFormat tFormat = TextureFormat.RGBA32;
            int tMipMapCount = 1;
            if (tTexture is Texture2D)
            {
                tFormat = (tTexture as Texture2D).format;
                tMipMapCount = (tTexture as Texture2D).mipmapCount;
            }
            if (tTexture is Cubemap)
            {
                tFormat = (tTexture as Cubemap).format;
            }

            tTextureDetails.format = tFormat;
            tTextureDetails.mipMapCount = tMipMapCount;

        }
        tTextureDetails.FoundInMaterials.Add(tMaterial);
        foreach (Renderer renderer in tMaterialDetails.FoundInRenderers)
        {
            if (!tTextureDetails.FoundInRenderers.Contains(renderer)) tTextureDetails.FoundInRenderers.Add(renderer);
        }
        return tTextureDetails;
    }

    void ListTextures()
    {
        textureListScrollPos = EditorGUILayout.BeginScrollView(textureListScrollPos);
        foreach (TextureDetails tDetails in ActiveTextures)
        {

            GUILayout.BeginHorizontal();
            GUILayout.Box(tDetails.texture, GUILayout.Width(ThumbnailWidth), GUILayout.Height(ThumbnailHeight));

            if (GUILayout.Button(tDetails.texture.name, GUILayout.Width(150)))
            {
                SelectObject(tDetails.texture, ctrlPressed);
            }

            string sizeLabel = "" + tDetails.texture.width + "x" + tDetails.texture.height;
            if (tDetails.isCubeMap) sizeLabel += "x6";
            sizeLabel += " - " + tDetails.mipMapCount + "mip";
            sizeLabel += "\n" + FormatSizeString(tDetails.memSizeKB) + " - " + tDetails.format + "";

            GUILayout.Label(sizeLabel, GUILayout.Width(120));

            if (GUILayout.Button(tDetails.FoundInMaterials.Count + " Mat", GUILayout.Width(50)))
            {
                SelectObjects(tDetails.FoundInMaterials, ctrlPressed);
            }

            if (GUILayout.Button(tDetails.FoundInRenderers.Count + " GO", GUILayout.Width(50)))
            {
                List<UnityEngine.Object> FoundObjects = new List<UnityEngine.Object>();
                foreach (Renderer renderer in tDetails.FoundInRenderers) FoundObjects.Add(renderer.gameObject);
                SelectObjects(FoundObjects, ctrlPressed);
            }

            GUILayout.EndHorizontal();
        }
        if (ActiveTextures.Count > 0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box(" ", GUILayout.Width(ThumbnailWidth), GUILayout.Height(ThumbnailHeight));

            if (GUILayout.Button("Select All", GUILayout.Width(150)))
            {
                List<UnityEngine.Object> AllTextures = new List<UnityEngine.Object>();
                foreach (TextureDetails tDetails in ActiveTextures) AllTextures.Add(tDetails.texture);
                SelectObjects(AllTextures, ctrlPressed);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    TextureDetails FindTextureDetails(Texture tTexture)
    {
        foreach (TextureDetails tTextureDetails in ActiveTextures)
        {
            if (tTextureDetails.texture == tTexture) return tTextureDetails;
        }
        return null;
    }

    void ListMaterials()
    {
        materialListScrollPos = EditorGUILayout.BeginScrollView(materialListScrollPos);

        foreach (MaterialDetails tDetails in ActiveMaterials)
        {
            if (tDetails.material != null)
            {
                GUILayout.BeginHorizontal();

                if (tDetails.material.mainTexture != null) GUILayout.Box(tDetails.material.mainTexture, GUILayout.Width(ThumbnailWidth), GUILayout.Height(ThumbnailHeight));
                else
                {
                    GUILayout.Box("n/a", GUILayout.Width(ThumbnailWidth), GUILayout.Height(ThumbnailHeight));
                }

                if (GUILayout.Button(tDetails.material.name, GUILayout.Width(150)))
                {
                    SelectObject(tDetails.material, ctrlPressed);
                }

                string shaderLabel = tDetails.material.shader != null ? tDetails.material.shader.name : "no shader";
                GUILayout.Label(shaderLabel, GUILayout.Width(200));

                if (GUILayout.Button(tDetails.FoundInRenderers.Count + " GO", GUILayout.Width(50)))
                {
                    List<UnityEngine.Object> FoundObjects = new List<UnityEngine.Object>();
                    foreach (Renderer renderer in tDetails.FoundInRenderers) FoundObjects.Add(renderer.gameObject);
                    SelectObjects(FoundObjects, ctrlPressed);
                }


                GUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
    }

    MaterialDetails FindMaterialDetails(Material tMaterial)
    {
        foreach (MaterialDetails tMaterialDetails in ActiveMaterials)
        {
            if (tMaterialDetails.material == tMaterial) return tMaterialDetails;
        }
        return null;
    }

    void ListMeshes()
    {
        meshListScrollPos = EditorGUILayout.BeginScrollView(meshListScrollPos);

        foreach (MeshDetails tDetails in ActiveMeshDetails)
        {
            if (tDetails.mesh != null)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(tDetails.mesh.name, GUILayout.Width(150)))
                {
                    SelectObject(tDetails.mesh, ctrlPressed);
                }
                string sizeLabel = "" + tDetails.mesh.vertexCount + " vert";

                GUILayout.Label(sizeLabel, GUILayout.Width(100));

                if (GUILayout.Button(tDetails.FoundInMeshFilters.Count + " GO", GUILayout.Width(50)))
                {
                    List<UnityEngine.Object> FoundObjects = new List<UnityEngine.Object>();
                    foreach (MeshFilter meshFilter in tDetails.FoundInMeshFilters) FoundObjects.Add(meshFilter.gameObject);
                    SelectObjects(FoundObjects, ctrlPressed);
                }

                if (GUILayout.Button(tDetails.FoundInSkinnedMeshRenderer.Count + " GO", GUILayout.Width(50)))
                {
                    List<UnityEngine.Object> FoundObjects = new List<UnityEngine.Object>();
                    foreach (SkinnedMeshRenderer skinnedMeshRenderer in tDetails.FoundInSkinnedMeshRenderer) FoundObjects.Add(skinnedMeshRenderer.gameObject);
                    SelectObjects(FoundObjects, ctrlPressed);
                }
                GUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
    }

    MeshDetails FindMeshDetails(Mesh tMesh)
    {
        foreach (MeshDetails tMeshDetails in ActiveMeshDetails)
        {
            if (tMeshDetails.mesh == tMesh) return tMeshDetails;
        }
        return null;
    }
}

//      Editor About Windows
//Class editor about windows log
public class AboutWindows : EditorWindow
{
    Texture2D header;
    string changelogText = "";
    Vector2 changelogScroll = Vector2.zero;
    GUIStyle LabelStyle;
    GUIStyle ButtonStyle;
    GUIStyle iconButtonStyle;
    Texture2D iconGitHub;
    Texture2D iconLicense;
    Texture2D iconUnity;
    Texture2D iconPacket;

    public static string pathImages = "Assets/Editor/EditorExtension/Resources/";
    public static string pathChangelogEng = "Assets/Editor/EditorExtension/Resources/ChangeLogEng.txt";
    public static string pathChangelogEsp = "Assets/Editor/EditorExtension/Resources/ChangeLogEsp.txt";

    public static bool Init(bool forceOpen)
    {
        AboutWindows window;
        window = EditorWindow.GetWindow<AboutWindows>(true, EditorStrings.About.StringAboutTitle, true);
        window.minSize = new Vector2(530, 650);
        window.maxSize = new Vector2(530, 650);
        window.ShowUtility();
        return true;
    }

    void OnEnable()
    {
        string versionColor = EditorGUIUtility.isProSkin ? "#ffffffee" : "#000000ee";
        switch (EditorStrings.Data._Languaje)
        {
            case "ENG":
                changelogText = Resources.LoadAssetAtPath<TextAsset>(pathChangelogEng).text;
                changelogText = Regex.Replace(changelogText, @"^[0-9].*", "<color=" + versionColor + "><size=13><b>Version $0</b></size></color>", RegexOptions.Multiline);
                changelogText = Regex.Replace(changelogText, @"^-.*", "  $0", RegexOptions.Multiline);
                break;

            case "ESP":
                changelogText = Resources.LoadAssetAtPath<TextAsset>(pathChangelogEsp).text;
                changelogText = Regex.Replace(changelogText, @"^[0-9].*", "<color=" + versionColor + "><size=13><b>Version $0</b></size></color>", RegexOptions.Multiline);
             changelogText = Regex.Replace(changelogText, @"^-.*", "  $0", RegexOptions.Multiline);
                break;

            default:
                changelogText = Resources.LoadAssetAtPath<TextAsset>(pathChangelogEng).text;
                changelogText = Regex.Replace(changelogText, @"^[0-9].*", "<color=" + versionColor + "><size=13><b>Version $0</b></size></color>", RegexOptions.Multiline);
                changelogText = Regex.Replace(changelogText, @"^-.*", "  $0", RegexOptions.Multiline);
                break;
        }

        header = Resources.LoadAssetAtPath<Texture2D>(pathImages + "headerEditorExt.jpg");
        iconGitHub = Resources.LoadAssetAtPath<Texture2D>(pathImages + "icon-github.png");
        iconLicense = Resources.LoadAssetAtPath<Texture2D>(pathImages + "icon-license.png");
        iconUnity = Resources.LoadAssetAtPath<Texture2D>(pathImages + "icon-unity.png");
        iconPacket = Resources.LoadAssetAtPath<Texture2D>(pathImages + "icon-packet.png");
    }

    void OnGUI()
    {
        if (LabelStyle == null)
        {
            LabelStyle = new GUIStyle(GUI.skin.label);
            LabelStyle.richText = true;
            LabelStyle.wordWrap = true;
            ButtonStyle = new GUIStyle(GUI.skin.button);
            ButtonStyle.richText = true;
            iconButtonStyle = new GUIStyle(GUI.skin.button);
            iconButtonStyle.normal.background = null;
            iconButtonStyle.imagePosition = ImagePosition.ImageOnly;
            iconButtonStyle.fixedWidth = 80;
            iconButtonStyle.fixedHeight = 80;
        }

        Rect headerRect = new Rect(0, 0, 530, 207);
        GUI.DrawTexture(headerRect, header, ScaleMode.ScaleAndCrop, false);

#if UNITY_4_3
		GUILayout.Space(204);
#else
        GUILayout.Space(214);
#endif

        GUILayout.BeginVertical();

        GUILayout.Label(EditorStrings.About.StringAboutNVersion, ButtonStyle);

        changelogScroll = GUILayout.BeginScrollView(changelogScroll);
        GUILayout.Label(changelogText, LabelStyle);
        GUILayout.EndScrollView();

        HR(0, 0);
        GUILayout.BeginHorizontal();
        GUILayout.Label(EditorStrings.About.StringAboutLink, ButtonStyle);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(iconGitHub, iconButtonStyle))
            Application.OpenURL("https://github.com/lPinchol/UnityEditor-MiniExtension");

        if (GUILayout.Button(iconLicense, iconButtonStyle))
            Application.OpenURL("https://github.com/lPinchol/UnityEditor-MiniExtension/blob/master/LICENSE");

        if (GUILayout.Button(iconUnity, iconButtonStyle))
            Application.OpenURL("http://unity3d.com/");

        if (GUILayout.Button(iconPacket, iconButtonStyle))
            Application.OpenURL("https://github.com/lPinchol/UnityEditor-MiniExtension/tree/master/Resources/unitypackage/");

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    void HR(int prevSpace, int nextSpace)
    {
        GUILayout.Space(prevSpace);
        Rect r = GUILayoutUtility.GetRect(Screen.width, 2);
        Color og = GUI.backgroundColor;
        GUI.backgroundColor = Color.black;
        GUI.Box(r, "");
        GUI.backgroundColor = og;
        GUILayout.Space(nextSpace);
    }
}

//      Editor Localizations Windows
//Class editor Localizations
public class Localizations : EditorWindow
{
    public Language selection = Language.English;

    void OnGUI()
    {
        selection = (Language)EditorGUILayout.EnumPopup("Select Language:", selection);
    }
}

//      Editor String
//Class containing the strings
public class EditorStrings
{
    public class Setup
    {
        public const string StringMainMenu                                  = "Tools/UnityEditorExtension";
        public const string StringDeletePrefs                               = StringMainMenu + "/PlayerPrefs/Clear PlayerPrefs";
        public const string StringDeleteEditorPrefs                         = StringMainMenu + "/PlayerPrefs/Clear EditorPrefs";
        public const string StringScreenShotInstant                         = StringMainMenu + "/ScreenShot/Instant Screenshot %g";
        public const string StringScreenShotSettings                        = StringMainMenu + "/ScreenShot/Screenshot Settings";
        public const string StringMenuRayCast                               = StringMainMenu + "/Debug RayCast/Create RayCast";
        public const string StringComponentRayCast                          = "Assets/Create/RayCast Debug";
        public const string StringMenuLanguageEnglish                       = StringMainMenu + "/Language/English";
        public const string StringMenuLanguageSpanish                       = StringMainMenu + "/Language/Spanish";
        public const string StringScriptableObject1                         = StringMainMenu + "/Create/ScriptableObject";
        public const string StringScriptableObject2                         = "Assets/Create/ScriptableObject";
        public const string StringAbout                                     = StringMainMenu + "/About " + EditorStrings.Data.StringVersionActual;
        public const string StringResourceChecked                           = StringMainMenu + "/Resource Checked";
        public const string StringLocalizations                             = StringMainMenu + "/Localization";
    }

    public class Data
    {
        public const string StringVersionActual                                = "0.0.14";


        public static string _Languaje                                          = "ENG";
    }

    public class ScreenShot
    {
        public static string StringResolution                                   = "Resolution";
        public static string StringWindows                                      = "Windows";
        public static string StringResSettings                                  = "Resolution Settings";
        public static string StringSetResScreen                                 = "Set To Screen Res";
        public static string StringNameSettings                                 = "Name Settings";
        public static string StringSetNameDefaut                                = "Set default Name";
        public static string StringNameScreen                                   = "Screen";
        public static string StringNameScreenShot                               = "Name ScreenShot: ";
        public static string StringWidth                                        = "Width";
        public static string StringHeight                                       = "Height";
        public static string StringScaleScreen                                  = "Scale Screen";
        public static string StringScale                                        = "Scale";
        public static string StringPathSettings                                 = "Path Settings";
        public static string StringBrowse                                       = "Browse";
        public static string StringPath                                         = "Path";
        public static string StringCameraSettings                               = "Camera Settings";
        public static string StringSelectCameraScene                            = "Select Camera Scene";
        public static string StringTextureSettings                              = "Texture Settings";
        public static string StringTransparentBackground                        = "Transparent Background";
        public static string StringDefaultOptions                               = "Default Options";
        public static string StringSetToScreenSize                              = "Set To Screen Size";
        public static string StringDefaultSize                                  = "Default Size";
        public static string StringScreenshotres                                = "Screenshot res [ ";
        public static string StringX                                            = " x ";
        public static string StringPX                                           = " px ]";
        public static string StringTakeScreenShot                               = "Take Screenshot";
        public static string StringPathToSaveImages                             = "Path to Save Images";
        public static string StringScreenShotName                               = "Screenshot name [ ";
        public static string StringOpenLastScreenshot                           = "Open Last Screenshot";
        public static string StringOpenFolder                                   = "Open Folder";
        public static string StringPathImages                                   = "Path Images";

        public static string StringMDebug                                       = "{e}[ScreenShot] ";
        public static string StringDebug01                                      = StringMDebug + "Path Set";
        public static string StringDebug02                                      = StringMDebug + "Opening File ";
        public static string StringDebug03                                      = StringMDebug + "Taking Screenshot";
    }

    public class RayCastDebug
    {
        public static string StringRayCastDebugWarning                          = "RayCast Debug Warning";
        public static string StringNeedRayCast                                  = "You need an Raycast Debug to create a copy.";
        public static string StringOK                                           = "OK";
        public static string StringName                                         = "Name";
        public static string StringGizmoSize                                    = "Gizmo Size";
        public static string StringColorLine                                    = "Color Line";
        public static string StringShowScale                                    = "Show Scale";
        public static string StringPixelUnit                                    = "Pixel unit";
        public static string StringAuxInfo1                                     = "       Distance from Start point: ";
        public static string StringAuxInfo2                                     = " - Scale per pixel: ";
        public static string StrinPX                                            = "px";

        public static string StringRCDebug                                      = "{e}[RayCast Debug] ";
        public static string StringDebug01                                      = StringRCDebug + "Create RayCast Debug";
        public static string StringDebug02                                      = StringRCDebug + "Add RayCast Debug";
    }

    public class ScriptableObject
    {
        public static string StringScriptableObjectClass                        = "ScriptableObject Class";
        public static string StringScriptableObjectCreate                       = "Create";
        public static string StringScriptableObjectNewScripta                   = "Create a new ScriptableObject";

        public static string StringScriptableObjectDebug                        = "{e}[ScriptableObject Debug] ";
        public static string StringScriptableObjectDebug01                      = StringScriptableObjectDebug + "Create Scriptable Object";
    }

    public class About
    {
        public static string StringAboutTitle                                   = "About Editor Extensions";
        public static string StringAboutNVersion                                = "> Note Version <";
        public static string StringAboutGitHub                                  = "GitHub";
        public static string StringAboutLink                                    = "> Link <";
    }

    public class Localizations
    { 
    
    }

    public class PreciseUI
    { 
    
    }
}

//      Editor Translator
//Class translator tools editor
public class EditorTranslator
{
    public static void English()
    {
        EditorStrings.ScreenShot.StringResolution                                = "Resolution";
        EditorStrings.ScreenShot.StringWindows                                   = "Windows";
        EditorStrings.ScreenShot.StringResSettings                               = "Resolution Settings";
        EditorStrings.ScreenShot.StringSetResScreen                              = "Set To Screen Res";
        EditorStrings.ScreenShot.StringNameSettings                              = "Name Settings";
        EditorStrings.ScreenShot.StringSetNameDefaut                             = "Set default Name";
        EditorStrings.ScreenShot.StringNameScreen                                = "Screen";
        EditorStrings.ScreenShot.StringNameScreenShot                            = "Name ScreenShot: ";
        EditorStrings.ScreenShot.StringWidth                                     = "Width";
        EditorStrings.ScreenShot.StringHeight                                    = "Height";
        EditorStrings.ScreenShot.StringScaleScreen                               = "Scale Screen";
        EditorStrings.ScreenShot.StringScale                                     = "Scale";
        EditorStrings.ScreenShot.StringPathSettings                              = "Path Settings";
        EditorStrings.ScreenShot.StringBrowse                                    = "Browse";
        EditorStrings.ScreenShot.StringPath                                      = "Path";
        EditorStrings.ScreenShot.StringCameraSettings                            = "Camera Settings";
        EditorStrings.ScreenShot.StringSelectCameraScene                         = "Select Camera Scene";
        EditorStrings.ScreenShot.StringTextureSettings                           = "Texture Settings";
        EditorStrings.ScreenShot.StringTransparentBackground                     = "Transparent Background";
        EditorStrings.ScreenShot.StringDefaultOptions                            = "Default Options";
        EditorStrings.ScreenShot.StringSetToScreenSize                           = "Set To Screen Size";
        EditorStrings.ScreenShot.StringDefaultSize                               = "Default Size";
        EditorStrings.ScreenShot.StringScreenshotres                             = "Screenshot res [ ";
        EditorStrings.ScreenShot.StringX                                         = " x ";
        EditorStrings.ScreenShot.StringPX                                        = " px ]";
        EditorStrings.ScreenShot.StringTakeScreenShot                            = "Take Screenshot";
        EditorStrings.ScreenShot.StringPathToSaveImages                          = "Path to Save Images";
        EditorStrings.ScreenShot.StringScreenShotName                            = "Screenshot name [ ";
        EditorStrings.ScreenShot.StringOpenLastScreenshot                        = "Open Last Screenshot";
        EditorStrings.ScreenShot.StringOpenFolder                                = "Open Folder";
        EditorStrings.ScreenShot.StringPathImages                                = "Path Images";

        EditorStrings.ScreenShot.StringMDebug                                    = "{e}[ScreenShot] ";
        EditorStrings.ScreenShot.StringDebug01                                  = EditorStrings.ScreenShot.StringMDebug + "Path Set";
        EditorStrings.ScreenShot.StringDebug02                                  = EditorStrings.ScreenShot.StringMDebug + "Opening File ";
        EditorStrings.ScreenShot.StringDebug03                                  = EditorStrings.ScreenShot.StringMDebug + "Taking Screenshot";


        EditorStrings.RayCastDebug.StringRayCastDebugWarning                    = "RayCast Debug Warning";
        EditorStrings.RayCastDebug.StringNeedRayCast                            = "You need an Raycast Debug to create a copy.";
        EditorStrings.RayCastDebug.StringOK                                     = "OK";
        EditorStrings.RayCastDebug.StringName                                   = "Name";
        EditorStrings.RayCastDebug.StringGizmoSize                              = "Gizmo Size";
        EditorStrings.RayCastDebug.StringColorLine                              = "Color Line";
        EditorStrings.RayCastDebug.StringShowScale                              = "Show Scale";
        EditorStrings.RayCastDebug.StringPixelUnit                              = "Pixel unit";
        EditorStrings.RayCastDebug.StringAuxInfo1                               = "       Distance from Start point: ";
        EditorStrings.RayCastDebug.StringAuxInfo2                               = " - Scale per pixel: ";
        EditorStrings.RayCastDebug.StrinPX                                      = "px";

        EditorStrings.RayCastDebug.StringRCDebug                                = "{e}[RayCast Debug] ";
        EditorStrings.RayCastDebug.StringDebug01                                = EditorStrings.RayCastDebug.StringRCDebug + "Create RayCast Debug";
        EditorStrings.RayCastDebug.StringDebug02                                = EditorStrings.RayCastDebug.StringRCDebug + "Add RayCast Debug";

        EditorStrings.ScriptableObject.StringScriptableObjectClass              = "ScriptableObject Class";
        EditorStrings.ScriptableObject.StringScriptableObjectCreate             = "Create";
        EditorStrings.ScriptableObject.StringScriptableObjectNewScripta         = "Create a new ScriptableObject";

        EditorStrings.ScriptableObject.StringScriptableObjectDebug              = "{e}[ScriptableObject Debug] ";
        EditorStrings.ScriptableObject.StringScriptableObjectDebug01            = EditorStrings.ScriptableObject.StringScriptableObjectDebug + "Create Scriptable Object";

        EditorStrings.About.StringAboutTitle                                    = "About Editor Extensions";
        EditorStrings.About.StringAboutNVersion                                 = "> Note Version <";
        EditorStrings.About.StringAboutGitHub                                   = "GitHub";
        EditorStrings.About.StringAboutLink                                     = "> Links <";
    }

    public static void Spanish()
    {
        EditorStrings.ScreenShot.StringResolution                               = "Resolucion";
        EditorStrings.ScreenShot.StringWindows                                  = "Windows";
        EditorStrings.ScreenShot.StringResSettings                              = "Ajustes Resolucion";
        EditorStrings.ScreenShot.StringSetResScreen                             = "Resolucion Pantalla";
        EditorStrings.ScreenShot.StringNameSettings                             = "Ajustes de Nombre";
        EditorStrings.ScreenShot.StringSetNameDefaut                            = "Nombre por defecto";
        EditorStrings.ScreenShot.StringNameScreen                               = "Pantalla";
        EditorStrings.ScreenShot.StringNameScreenShot                           = "Nombre captura pantalla: ";
        EditorStrings.ScreenShot.StringWidth                                    = "Ancho";
        EditorStrings.ScreenShot.StringHeight                                   = "Alto";
        EditorStrings.ScreenShot.StringScaleScreen                              = "Escala Ventana";
        EditorStrings.ScreenShot.StringScale                                    = "Escala";
        EditorStrings.ScreenShot.StringPathSettings                             = "Ajustes Ruta";
        EditorStrings.ScreenShot.StringBrowse                                   = "Buscar";
        EditorStrings.ScreenShot.StringPath                                     = "Ruta";
        EditorStrings.ScreenShot.StringCameraSettings                           = "Ajustes Camara";
        EditorStrings.ScreenShot.StringSelectCameraScene                        = "Seleccionar Camara Escena";
        EditorStrings.ScreenShot.StringTextureSettings                          = "Ajustes Texturas";
        EditorStrings.ScreenShot.StringTransparentBackground                    = "Fondo Transparente";
        EditorStrings.ScreenShot.StringDefaultOptions                           = "Opciones por defecto";
        EditorStrings.ScreenShot.StringSetToScreenSize                          = "Tamaño Pantalla";
        EditorStrings.ScreenShot.StringDefaultSize                              = "Tamaño por defecto";
        EditorStrings.ScreenShot.StringScreenshotres                            = "Resolucion captura de pantalla [ ";
        EditorStrings.ScreenShot.StringX                                        = " x ";
        EditorStrings.ScreenShot.StringPX                                       = " px ]";
        EditorStrings.ScreenShot.StringTakeScreenShot                           = "Tomar Captura de pantalla";
        EditorStrings.ScreenShot.StringPathToSaveImages                         = "Ruta para guardar imagen";
        EditorStrings.ScreenShot.StringScreenShotName                           = "Nombre captura de pantalla [ ";
        EditorStrings.ScreenShot.StringOpenLastScreenshot                       = "Abrir ultima captura";
        EditorStrings.ScreenShot.StringOpenFolder                               = "Abrir carpeta";
        EditorStrings.ScreenShot.StringPathImages                               = "Ruta Imagenes";

        EditorStrings.ScreenShot.StringMDebug                                   = "{e}[CapturaPantalla] ";
        EditorStrings.ScreenShot.StringDebug01                                  = EditorStrings.ScreenShot.StringMDebug + "Ruta Seleccionada";
        EditorStrings.ScreenShot.StringDebug02                                  = EditorStrings.ScreenShot.StringMDebug + "Abriendo Archivo ";
        EditorStrings.ScreenShot.StringDebug03                                  = EditorStrings.ScreenShot.StringMDebug + "Tomando Captura";


        EditorStrings.RayCastDebug.StringRayCastDebugWarning                    = "Advertencia RayCast Debug";
        EditorStrings.RayCastDebug.StringNeedRayCast                            = "Necesitas una copia en escena de RayCast Debug";
        EditorStrings.RayCastDebug.StringOK                                     = "OK";
        EditorStrings.RayCastDebug.StringName                                   = "Nombre";
        EditorStrings.RayCastDebug.StringGizmoSize                              = "Gizmo Tamaño";
        EditorStrings.RayCastDebug.StringColorLine                              = "Color Linea";
        EditorStrings.RayCastDebug.StringShowScale                              = "Mostrar Escala";
        EditorStrings.RayCastDebug.StringPixelUnit                              = "Pixel unidades";
        EditorStrings.RayCastDebug.StringAuxInfo1                               = "       Distancia desde el punto inicial: ";
        EditorStrings.RayCastDebug.StringAuxInfo2                               = " - Escala por pixel: ";
        EditorStrings.RayCastDebug.StrinPX                                      = "px";

        EditorStrings.RayCastDebug.StringRCDebug                                = "{e}[RayCast Debug] ";
        EditorStrings.RayCastDebug.StringDebug01                                = EditorStrings.RayCastDebug.StringRCDebug + "Creado RayCast Debug";
        EditorStrings.RayCastDebug.StringDebug02                                = EditorStrings.RayCastDebug.StringRCDebug + "Añadido RayCast Debug";

        EditorStrings.ScriptableObject.StringScriptableObjectClass              = "ScriptableObject Class";
        EditorStrings.ScriptableObject.StringScriptableObjectCreate             = "Crear";
        EditorStrings.ScriptableObject.StringScriptableObjectNewScripta         = "Crear un nuevo ScriptableObject";

        EditorStrings.ScriptableObject.StringScriptableObjectDebug              = "{e}[ScriptableObject Debug] ";
        EditorStrings.ScriptableObject.StringScriptableObjectDebug01            = EditorStrings.ScriptableObject.StringScriptableObjectDebug + "Creado Scriptable Object";

        EditorStrings.About.StringAboutTitle                                    = "About Editor Extension";
        EditorStrings.About.StringAboutNVersion                                 = "> Notas Version <";
        EditorStrings.About.StringAboutGitHub                                   = "GitHub";
        EditorStrings.About.StringAboutLink                                     = "> Vinculos <";
    }
}

//      Internal Scriptable Object
//Class Internal Scriptable Object
internal class EndNameEdit : EndNameEditAction
{
    public override void Action(int instanceId, string pathName, string resourceFile)
    {
        AssetDatabase.CreateAsset(EditorUtility.InstanceIDToObject(instanceId), AssetDatabase.GenerateUniqueAssetPath(pathName));
    }
}

//      Internal ResourceChecker
//Class ResourceChecker
public class TextureDetails
{
    public bool isCubeMap;
    public int memSizeKB;
    public Texture texture;
    public TextureFormat format;
    public int mipMapCount;
    public List<UnityEngine.Object> FoundInMaterials = new List<UnityEngine.Object>();
    public List<UnityEngine.Object> FoundInRenderers = new List<UnityEngine.Object>();
    public TextureDetails()
    {

    }
}

//      Internal ResourceChecker
//Class ResourceChecker
public class MaterialDetails
{

    public Material material;

    public List<Renderer> FoundInRenderers = new List<Renderer>();

    public MaterialDetails()
    {

    }
}

//      Internal ResourceChecker
//Class ResourceChecker
public class MeshDetails
{

    public Mesh mesh;

    public List<MeshFilter> FoundInMeshFilters = new List<MeshFilter>();
    public List<SkinnedMeshRenderer> FoundInSkinnedMeshRenderer = new List<SkinnedMeshRenderer>();

    public MeshDetails()
    {

    }
}

public enum Language
{ 
    English,
    Spanish,
    French
}
