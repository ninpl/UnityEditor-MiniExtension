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

    [MenuItem(EditorStrings.Setup.StringMenuLanguageEnglish, false, 100)]
    public static void MenuLanguageEnglish()
    {
        EditorTranslator.English();
    }

    [MenuItem(EditorStrings.Setup.StringMenuLanguageSpanish, false, 101)]
    public static void MenuLanguageSpanish()
    {
        EditorTranslator.Spanish();
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
        public const string StringMenuLanguageEnglish                       = StringMainMenu + "/Language/English";
        public const string StringMenuLanguageSpanish                       = StringMainMenu + "/Language/Spanish";
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
        public static string StringRayCastDebugWarning                       = "RayCast Debug Warning";
        public static string StringNeedRayCast                               = "You need an Raycast Debug to create a copy.";
        public static string StringOK                                        = "OK";
        public static string StringName                                      = "Name";
        public static string StringGizmoSize                                 = "Gizmo Size";
        public static string StringColorLine                                 = "Color Line";
        public static string StringShowScale                                 = "Show Scale";
        public static string StringPixelUnit                                 = "Pixel unit";
        public static string StringAuxInfo1                                  = "       Distance from Start point: ";
        public static string StringAuxInfo2                                  = " - Scale per pixel: ";
        public static string StrinPX                                         = "px";

        public static string StringRCDebug                                   = "{e}[RayCast Debug] ";
        public static string StringDebug01                                   = StringRCDebug + "Create RayCast Debug";
        public static string StringDebug02                                   = StringRCDebug + "Add RayCast Debug";
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
    }

    public static void Spanish()
    {
        EditorStrings.ScreenShot.StringResolution = "Resolucion";
        EditorStrings.ScreenShot.StringWindows = "Windows";
        EditorStrings.ScreenShot.StringResSettings = "Ajustes Resolucion";
        EditorStrings.ScreenShot.StringSetResScreen = "Resolucion Pantalla";
        EditorStrings.ScreenShot.StringNameSettings = "Ajustes de Nombre";
        EditorStrings.ScreenShot.StringSetNameDefaut = "Nombre por defecto";
        EditorStrings.ScreenShot.StringNameScreen = "Pantalla";
        EditorStrings.ScreenShot.StringNameScreenShot = "Nombre captura pantalla: ";
        EditorStrings.ScreenShot.StringWidth = "Ancho";
        EditorStrings.ScreenShot.StringHeight = "Alto";
        EditorStrings.ScreenShot.StringScaleScreen = "Escala Ventana";
        EditorStrings.ScreenShot.StringScale = "Escala";
        EditorStrings.ScreenShot.StringPathSettings = "Ajustes Ruta";
        EditorStrings.ScreenShot.StringBrowse = "Buscar";
        EditorStrings.ScreenShot.StringPath = "Ruta";
        EditorStrings.ScreenShot.StringCameraSettings = "Ajustes Camara";
        EditorStrings.ScreenShot.StringSelectCameraScene = "Seleccionar Camara Escena";
        EditorStrings.ScreenShot.StringTextureSettings = "Ajustes Texturas";
        EditorStrings.ScreenShot.StringTransparentBackground = "Fondo Transparente";
        EditorStrings.ScreenShot.StringDefaultOptions = "Opciones por defecto";
        EditorStrings.ScreenShot.StringSetToScreenSize = "Tamaño Pantalla";
        EditorStrings.ScreenShot.StringDefaultSize = "Tamaño por defecto";
        EditorStrings.ScreenShot.StringScreenshotres = "Resolucion captura de pantalla [ ";
        EditorStrings.ScreenShot.StringX = " x ";
        EditorStrings.ScreenShot.StringPX = " px ]";
        EditorStrings.ScreenShot.StringTakeScreenShot = "Tomar Captura de pantalla";
        EditorStrings.ScreenShot.StringPathToSaveImages = "Ruta para guardar imagen";
        EditorStrings.ScreenShot.StringScreenShotName = "Nombre captura de pantalla [ ";
        EditorStrings.ScreenShot.StringOpenLastScreenshot = "Abrir ultima captura";
        EditorStrings.ScreenShot.StringOpenFolder = "Abrir carpeta";
        EditorStrings.ScreenShot.StringPathImages = "Ruta Imagenes";

        EditorStrings.ScreenShot.StringMDebug = "{e}[CapturaPantalla] ";
        EditorStrings.ScreenShot.StringDebug01 = EditorStrings.ScreenShot.StringMDebug + "Ruta Seleccionada";
        EditorStrings.ScreenShot.StringDebug02 = EditorStrings.ScreenShot.StringMDebug + "Abriendo Archivo ";
        EditorStrings.ScreenShot.StringDebug03 = EditorStrings.ScreenShot.StringMDebug + "Tomando Captura";


        EditorStrings.RayCastDebug.StringRayCastDebugWarning = "Advertencia RayCast Debug";
        EditorStrings.RayCastDebug.StringNeedRayCast = "Necesitas una copia en escena de RayCast Debug";
        EditorStrings.RayCastDebug.StringOK = "OK";
        EditorStrings.RayCastDebug.StringName = "Nombre";
        EditorStrings.RayCastDebug.StringGizmoSize = "Gizmo Tamaño";
        EditorStrings.RayCastDebug.StringColorLine = "Color Linea";
        EditorStrings.RayCastDebug.StringShowScale = "Mostrar Escala";
        EditorStrings.RayCastDebug.StringPixelUnit = "Pixel unidades";
        EditorStrings.RayCastDebug.StringAuxInfo1 = "       Distancia desde el punto inicial: ";
        EditorStrings.RayCastDebug.StringAuxInfo2 = " - Escala por pixel: ";
        EditorStrings.RayCastDebug.StrinPX = "px";

        EditorStrings.RayCastDebug.StringRCDebug = "{e}[RayCast Debug] ";
        EditorStrings.RayCastDebug.StringDebug01 = EditorStrings.RayCastDebug.StringRCDebug + "Creado RayCast Debug";
        EditorStrings.RayCastDebug.StringDebug02 = EditorStrings.RayCastDebug.StringRCDebug + "Añadido RayCast Debug";
    }
}


