using UnityEngine;
using UnityEditor;

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
    }

    [MenuItem(EditorStrings.Setup.StringScreenShotSettings, false, 3)]
    public static void MenuScreenShotSettings()
    {
        EditorWindow editorWindow = EditorWindow.GetWindow(typeof(ScreemShotInstant));
        editorWindow.autoRepaintOnSceneChange = true;
        editorWindow.Show();
        editorWindow.title = "Screenshot";
    }

    [MenuItem(EditorStrings.Setup.StringMenuRayCast, false, 4)]
    public static void MenuRaycast()
    {
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


    //COMPONENT

    [MenuItem(EditorStrings.Setup.StringComponentRayCast, false, 5)]
    public static void ComponentRaycast()
    {
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
    }

    public class ScreenShot
    {
        public const string StringResolution                                = "Resolution";
        public const string StringWindows                                   = "Windows";
        public const string StringResSettings                               = "Resolution Settings";
        public const string StringSetResScreen                              = "Set To Screen Res";
        public const string StringNameSettings                              = "Name Settings";
        public const string StringSetNameDefaut                             = "Set default Name";
        public const string StringNameScreen                                = "Screen";
        public const string StringNameScreenShot                            = "Name ScreenShot: ";
        public const string StringWidth                                     = "Width";
        public const string StringHeight                                    = "Height";
        public const string StringScaleScreen                               = "Scale Screen";
        public const string StringScale                                     = "Scale";
        public const string StringPathSettings                              = "Path Settings";
        public const string StringBrowse                                    = "Browse";
        public const string StringPath                                      = "Path";
        public const string StringCameraSettings                            = "Camera Settings";
        public const string StringSelectCameraScene                         = "Select Camera Scene";
        public const string StringTextureSettings                           = "Texture Settings";
        public const string StringTransparentBackground                     = "Transparent Background";
        public const string StringDefaultOptions                            = "Default Options";
        public const string StringSetToScreenSize                           = "Set To Screen Size";
        public const string StringDefaultSize                               = "Default Size";
        public const string StringScreenshotres                             = "Screenshot res [ ";
        public const string StringX                                         = " x ";
        public const string StringPX                                        = " px ]";
        public const string StringTakeScreenShot                            = "Take Screenshot";
        public const string StringPathToSaveImages                          = "Path to Save Images";
        public const string StringScreenShotName                            = "Screenshot name [ ";
        public const string StringOpenLastScreenshot                        = "Open Last Screenshot";
        public const string StringOpenFolder                                = "Open Folder";
        public const string StringPathImages                                = "Path Images";

        public const string StringMDebug                                    = "{e}[ScreenShot] ";
        public const string StringDebug01                                   = StringMDebug + "Path Set";
        public const string StringDebug02                                   = StringMDebug + "Opening File ";
        public const string StringDebug03                                   = StringMDebug + "Taking Screenshot";
    }

    public class RayCastDebug
    {
        public const string StringRayCastDebugWarning                       = "RayCast Debug Warning";
        public const string StringNeedRayCast                               = "You need an Raycast Debug to create a copy.";
        public const string StringOK                                        = "OK";
        public const string StringName                                      = "Name";
        public const string StringGizmoSize                                 = "Gizmo Size";
        public const string StringColorLine                                 = "Color Line";
        public const string StringShowScale                                 = "Show Scale";
        public const string StringPixelUnit                                 = "Pixel unit";
        public const string StringAuxInfo1                                  = "       Distance from Start point: ";
        public const string StringAuxInfo2                                  = " - Scale per pixel: ";
        public const string StrinPX                                         = "px";

        public const string StringRCDebug                                   = "{e}[RayCast Debug] ";
        public const string StringDebug01                                   = StringRCDebug + "Create RayCast Debug";
        public const string StringDebug02                                   = StringRCDebug + "Add RayCast Debug";
    }
}


