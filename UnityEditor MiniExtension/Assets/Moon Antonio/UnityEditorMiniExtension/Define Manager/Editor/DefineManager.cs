//                                  ┌∩┐(◣_◢)┌∩┐
//																				\\
// DefineManager.cs (22/11/2017)												\\
// Autor: Antonio Mateo (.\Moon Antonio) 	antoniomt.moon@gmail.com			\\
// Descripcion:		Core de DefineManager										\\
// Fecha Mod:		22/11/2017													\\
// Ultima Mod:		Version Inicial												\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
#endregion

namespace MoonAntonio.UEME.MDefine
{
	/// <summary>
	/// <para>Core de DefineManager</para>
	/// </summary>
	public class DefineManager : EditorWindow
    {
		#region Variables Privadas
		private GlobalDefineSetting definesSaved = new GlobalDefineSetting();
		private GlobalDefineSetting definesEditing = new GlobalDefineSetting();
		private List<string> listDeleteReserved = new List<string>();
		private bool isInputProcessed = false;
		private string newDefineName = DEFAULT_NAME;
		private string selectedDefine = "";
		private string editingDefine = "";
		private string editingName = "";
		private bool isEditingName = false;
		private bool doRename = false;
		private Vector2 m_scrollDefines;
		#endregion

		#region Constantes
		private const int COMPILER_COUNT = (int)COMPILER.COUNT;
		private const string DEFAULT_NAME = "NUEVA_DEFINE";
		private const string STR_DEFINE_NAME_TEXT_FIELD = "EDITANDO_NOMBRE";
		private const float DEFINE_LABEL_WIDTH = 80f;
		private const float TOP_MENU_BUTTON_WIDTH = 250f;
		private const float BUTTON_WIDTH = 65f;
		private const float TOGGLE_WIDTH = 120f;
		private const float DEFINE_LABEL_LONG_WIDTH = DEFINE_LABEL_WIDTH + BUTTON_WIDTH + BUTTON_WIDTH + 8f;
		#endregion

		#region Enums
		public enum COMPILER
		{
			START = 0,
			CSHARP = START,
			CSHARP_EDITOR,
			UNITY_SCRIPT,
			END = UNITY_SCRIPT,
			COUNT = END - START + 1,
		}
		#endregion

		#region Class
		public class GlobalDefine
		{
			#region Variables Privadas
			private bool[] isEnableSettings = new bool[COMPILER_COUNT];
			#endregion

			#region Constantes
			private const string PATRON = @"^\w+$";
			#endregion

			#region Propiedades
			public string Nombre { get; private set; }
			#endregion

			#region Constructor
			public GlobalDefine(string n)
			{
				Nombre = n;
			}
			#endregion

			#region Metodos Publicos
			/// <summary>
			/// <para>Renombra un nombre.</para>
			/// </summary>
			/// <param name="newNombre"></param>
			public void Renombrar(string newNombre)// Renombra un nombre
			{
				Nombre = newNombre;
			}

			/// <summary>
			/// <para>Habilita el tipo de copilacion.</para>
			/// </summary>
			/// <param name="tipoCopilador"></param>
			/// <param name="isEnable"></param>
			public void SetEnable(COMPILER tipoCopilador, bool isEnable)// Habilita el tipo de copilacion
			{
				uint compilerIndex = (uint)tipoCopilador;

				if (compilerIndex < isEnableSettings.Length) isEnableSettings[compilerIndex] = isEnable;
			}

			/// <summary>
			/// <para>Habilita todos los tipos de copilacion</para>
			/// </summary>
			/// <param name="isEnable"></param>
			public void SetEnableAll(bool isEnable)// Habilita todos los tipos de copilacion
			{
				for (int i = 0; i < isEnableSettings.Length; ++i)
				{
					isEnableSettings[i] = isEnable;
				}
			}
			#endregion

			#region Funcionalidad
			/// <summary>
			/// <para>Esta activo el tipo de copilacion</para>
			/// </summary>
			/// <param name="tipoCopilador"></param>
			/// <returns></returns>
			public bool IsEnabled(COMPILER tipoCopilador)// Esta activo el tipo de copilacion
			{
				uint compilerIndex = (uint)tipoCopilador;

				if (compilerIndex < isEnableSettings.Length) return isEnableSettings[compilerIndex];

				return false;
			}

			/// <summary>
			/// <para>Determina si todos los tipos estan activos</para>
			/// </summary>
			/// <returns></returns>
			public bool IsEnabledAll()// Determina si todos los tipos estan activos
			{
				for (int i = 0; i < isEnableSettings.Length; ++i)
				{
					if (isEnableSettings[i] == false) return false;
				}
				return true;
			}

			/// <summary>
			/// <para>Iguala un global con otro</para>
			/// </summary>
			/// <param name="a"></param>
			/// <param name="b"></param>
			/// <returns></returns>
			public static bool IsEquals(GlobalDefine a, GlobalDefine b)// Iguala un global con otro
			{
				if (a == null && b == null)
					return true;

				if (a == null || b == null)
					return false;

				if (a.Nombre != b.Nombre)
				{
					return false;
				}

				for (int i = 0; i < a.isEnableSettings.Length; ++i)
				{
					if (a.isEnableSettings[i] != b.isEnableSettings[i])
						return false;
				}

				return true;
			}

			/// <summary>
			/// <para>Determina si es valido el nombre segun el patron</para>
			/// </summary>
			/// <param name="n"></param>
			/// <returns></returns>
			public static bool IsValidName(string n)// Determina si es valido el nombre segun el patron
			{
				return Regex.IsMatch(n, PATRON);
			}
			#endregion
		}

		public class GlobalDefineSetting
		{
			#region Variables Privadas
			private static readonly string[] RSP_FILE_PATHs = new string[COMPILER_COUNT] { StringsUEME.RSP_SMCS_MDEFINE, StringsUEME.RSP_GMCS_MDEFINE, StringsUEME.RSP_US_MDEFINE };
			private Dictionary<string, GlobalDefine> dicDefines = new Dictionary<string, GlobalDefine>();
			#endregion

			#region Constantes
			private const string NOMBRE_GRUPO = @"DEFINE";
			private const string PATRON = @"[-/]d(?:efine)*:(?:;*(?<" + NOMBRE_GRUPO + @">\w+))+;*";
			private const string SETTING_FILE_PATH = StringsUEME.SETTING_MDEFINE;
			private const string RSP_DEFINE_OPTION = "-define:";
			private const char DEFINE_DELIMITER = ';';
			#endregion

			#region Propiedades
			public ICollection<GlobalDefine> DefineCollection
			{
				get { return dicDefines.Values; }
			}
			#endregion

			#region Metodos
			/// <summary>
			/// <para>Renombra una definicion</para>
			/// </summary>
			/// <param name="oldName"></param>
			/// <param name="newName"></param>
			public void Renombrar(string oldName, string newName)// Renombra una definicion
			{
				GlobalDefine defineData = null;
				dicDefines.TryGetValue(oldName, out defineData);
				if (defineData == null)
					return;

				dicDefines.Remove(oldName);
				defineData.Renombrar(newName);
				dicDefines[defineData.Nombre] = defineData;
			}
			#endregion

			#region Funcionalidad
			/// <summary>
			/// <para>Obtener los archivos</para>
			/// </summary>
			/// <param name="compilerType"></param>
			/// <returns></returns>
			private static string RspFilePath(COMPILER compilerType)// Obtener los archivos
			{
				uint compilerIndex = (uint)compilerType;
				if (compilerIndex < RSP_FILE_PATHs.Length)
					return RSP_FILE_PATHs[compilerIndex];
				return "";
			}

			/// <summary>
			/// <para>Obtener Data</para>
			/// </summary>
			/// <param name="defineName"></param>
			/// <returns></returns>
			public GlobalDefine GetData(string defineName)// Obtener Data
			{
				if (string.IsNullOrEmpty(defineName))
					return null;

				GlobalDefine defineData = null;
				dicDefines.TryGetValue(defineName, out defineData);
				if (defineData == null)
				{
					defineData = new GlobalDefine(defineName);
					dicDefines[defineName] = defineData;
				}

				return defineData;
			}

			/// <summary>
			/// <para>Eliminar data</para>
			/// </summary>
			/// <param name="defineName"></param>
			/// <returns></returns>
			public bool DelData(string defineName)// Eliminar data
			{
				return dicDefines.Remove(defineName);
			}

			/// <summary>
			/// <para>Determina si esta definido</para>
			/// </summary>
			/// <param name="defineName"></param>
			/// <returns></returns>
			public bool IsDefined(string defineName)// Determina si esta definido
			{
				return dicDefines.ContainsKey(defineName);
			}

			/// <summary>
			/// <para>Es igual a otra globaldefine</para>
			/// </summary>
			/// <param name="a"></param>
			/// <param name="b"></param>
			/// <returns></returns>
			public static bool IsEquals(GlobalDefineSetting a, GlobalDefineSetting b)// Es igual a otra globaldefine
			{
				if (a.dicDefines.Count != b.dicDefines.Count)
					return false;

				foreach (GlobalDefine aDefine in a.dicDefines.Values)
				{
					if (aDefine == null)
						continue;

					GlobalDefine bDefine = null;
					b.dicDefines.TryGetValue(aDefine.Nombre, out bDefine);
					if (bDefine == null || GlobalDefine.IsEquals(aDefine, bDefine) == false)
						return false;
				}

				return true;
			}
			#endregion

			#region Metodos Cargar Ajustes
			/// <summary>
			/// <para>Carga los ajustes</para>
			/// </summary>
			public void CargarSettings()// Carga los ajustes
			{
				dicDefines.Clear();
				CargarSettingFile();
				CargarRspFile(COMPILER.CSHARP);
				CargarRspFile(COMPILER.CSHARP_EDITOR);
				CargarRspFile(COMPILER.UNITY_SCRIPT);
			}

			/// <summary>
			/// <para>Carga los archivos</para>
			/// </summary>
			private void CargarSettingFile()// Carga los archivos
			{
				if (!File.Exists(SETTING_FILE_PATH))
					return;

				string[] lines = File.ReadAllLines(SETTING_FILE_PATH);
				if (lines == null || lines.Length <= 0)
					return;

				for (int i = 1; i < lines.Length; ++i)
				{
					if (string.IsNullOrEmpty(lines[i]))
						continue;
					string defineText = lines[i].Replace("//", "");

					string[] defines = defineText.Split(DEFINE_DELIMITER);
					if (defines == null || defines.Length <= 0)
						continue;

					foreach (string define in defines)
					{
						if (string.IsNullOrEmpty(define))
							continue;

						GetData(define);
					}
				}
			}

			/// <summary>
			/// <para>Carga los RSP</para>
			/// </summary>
			/// <param name="compilerType"></param>
			private void CargarRspFile(COMPILER compilerType)// Carga los RSP
			{
				string path = RspFilePath(compilerType);

				if (!File.Exists(path))
					return;

				string rspOption = File.ReadAllText(path);

				MatchCollection defineMatchs = Regex.Matches(rspOption, PATRON);
				foreach (Match match in defineMatchs)
				{
					Group group = match.Groups[NOMBRE_GRUPO];
					foreach (Capture cap in group.Captures)
					{
						GlobalDefine define = GetData(cap.Value);
						if (define != null)
							define.SetEnable(compilerType, true);
					}
				}
			}
			#endregion

			#region Metodos Guardar Ajustes
			/// <summary>
			/// <para>Guarda los ajustes</para>
			/// </summary>
			public void GuardarSettings()// Guarda los ajustes
			{
				GuardarSettingFile();
				GuardarRspFile(COMPILER.CSHARP);
				GuardarRspFile(COMPILER.CSHARP_EDITOR);
				GuardarRspFile(COMPILER.UNITY_SCRIPT);

				AssetDatabase.Refresh();
				AssetDatabase.ImportAsset(SETTING_FILE_PATH, ImportAssetOptions.ForceUpdate);
			}

			/// <summary>
			/// <para>Guarda los ajustes en un fichero</para>
			/// </summary>
			private void GuardarSettingFile()// Guarda los ajustes en un fichero
			{
				string settingText = StringsUEME.NOTICIA_MDEFINE + System.Environment.NewLine + "//";
				foreach (GlobalDefine define in dicDefines.Values)
				{
					settingText += define.Nombre + DEFINE_DELIMITER;
				}

				using (StreamWriter writer = new StreamWriter(SETTING_FILE_PATH))
				{
					writer.Write(settingText);
				}
			}

			/// <summary>
			/// <para>Guarda los archivos RSP</para>
			/// </summary>
			/// <param name="compilerType"></param>
			private void GuardarRspFile(COMPILER compilerType)// Guarda los archivos RSP
			{
				string path = RspFilePath(compilerType);

				string rspOption = "";
				if (File.Exists(path))
				{
					rspOption = File.ReadAllText(path);

					MatchCollection defineMatchs = Regex.Matches(rspOption, PATRON);
					foreach (Match match in defineMatchs)
					{
						rspOption = rspOption.Replace(match.Value, "");
					}
				}

				string appendDefine = "";
				foreach (GlobalDefine define in dicDefines.Values)
				{
					if (define == null || define.IsEnabled(compilerType) == false)
						continue;
					appendDefine += define.Nombre + DEFINE_DELIMITER;
				}

				if (string.IsNullOrEmpty(appendDefine) == false)
					rspOption += RSP_DEFINE_OPTION + appendDefine;

				using (StreamWriter writer = new StreamWriter(path))
				{
					writer.Write(rspOption);
				}
			}
			#endregion
		}
		#endregion

		#region GUI
		/// <summary>
		/// <para>Interfaz</para>
		/// </summary>
		private void OnGUI()// Interfaz
		{
			isInputProcessed = false;

			OnGUITopMenu();
			OnGUIAddMenu();

			m_scrollDefines = EditorGUILayout.BeginScrollView(m_scrollDefines);
			listDeleteReserved.Clear();
			int no = 0;
			foreach (GlobalDefine defineItem in definesEditing.DefineCollection)
			{
				OnGUIDefineItem(++no, defineItem);
			}
			foreach (string delName in listDeleteReserved)
			{
				definesEditing.DelData(delName);
			}
			if (doRename)
			{
				doRename = false;
				definesEditing.Renombrar(editingDefine, editingName);
				EndRename();
			}
			EditorGUILayout.EndScrollView();

			ProcessMouseOut();

			if (isInputProcessed)
				Repaint();
		}
		#endregion

		#region GUI Elementos
		/// <summary>
		/// <para>Menu de la interfaz</para>
		/// </summary>
		private void OnGUITopMenu()// Menu de la interfaz
		{
			EditorGUILayout.BeginHorizontal();

			bool isChanged = !GlobalDefineSetting.IsEquals(definesSaved, definesEditing);

			EditorGUI.BeginDisabledGroup(!isChanged);
			if (isChanged)
			{
				GUI.backgroundColor = Color.red;
			}
			if (GUILayout.Button(StringsUEME.APLICAR_MDEFINE, GUILayout.Width(TOP_MENU_BUTTON_WIDTH)))
			{
				definesEditing.GuardarSettings();
				definesSaved.CargarSettings();
			}
			if (isChanged)
			{
				GUI.backgroundColor = Color.green;
			}
			if (GUILayout.Button(StringsUEME.REVERTIR_MDEFINE, GUILayout.Width(TOP_MENU_BUTTON_WIDTH)))
			{
				definesEditing.CargarSettings();
			}
			GUI.backgroundColor = Color.white;
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.EndHorizontal();

			if (isChanged)
			{
				EditorGUILayout.HelpBox(StringsUEME.AVISO_APLICAR_MDEFINE, MessageType.Warning);
			}
		}

		/// <summary>
		/// <para>Agregar de la interfaz</para>
		/// </summary>
		private void OnGUIAddMenu()// Agregar de la interfaz
		{
			EditorGUILayout.BeginHorizontal();

			newDefineName = EditorGUILayout.TextField(newDefineName, GUILayout.MinWidth(TOP_MENU_BUTTON_WIDTH));

			bool isInvalid = false;
			if (GlobalDefine.IsValidName(newDefineName) == false)
			{
				EditorGUILayout.HelpBox(StringsUEME.NOMBRE_INVALIDO_MDEFINE, MessageType.Error);
				isInvalid = true;
			}
			else if (definesEditing.IsDefined(newDefineName))
			{
				EditorGUILayout.HelpBox(StringsUEME.DEFINE_EXISTE_MDEFINE, MessageType.Error);
				isInvalid = true;
			}

			EditorGUI.BeginDisabledGroup(isInvalid);
			if (isInvalid == false)
			{
				GUI.backgroundColor = Color.cyan;
			}
			if (GUILayout.Button(StringsUEME.ADD_DEFINE_MDEFINE, GUILayout.Width(TOGGLE_WIDTH)))
			{
				GlobalDefine newDefine = definesEditing.GetData(newDefineName);
				if (newDefine != null)
					newDefine.SetEnableAll(true);
				GUI.FocusControl("");
				newDefineName = CrearNuevoNombreDefine();
			}
			GUI.backgroundColor = Color.white;
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.EndHorizontal();
		}

		/// <summary>
		/// <para>Item Define</para>
		/// </summary>
		/// <param name="no"></param>
		/// <param name="defineItem"></param>
		private void OnGUIDefineItem(int no, GlobalDefine defineItem)// Item Define
		{
			if (defineItem == null)
				return;

			bool isSelected = IsSelectedDefine(defineItem);
			bool isEditing = IsEditingDefine(defineItem);
			bool isEnabledAll = defineItem.IsEnabledAll();

			GUI.backgroundColor = isSelected ? Color.yellow : Color.white;
			Rect rtItem = EditorGUILayout.BeginHorizontal("TextArea", GUILayout.MinHeight(25f));
			GUI.backgroundColor = Color.white;

			GUI.color = isEnabledAll ? Color.cyan : Color.black;
			EditorGUILayout.LabelField(no.ToString() + ".", GUILayout.Width(20f));
			GUI.color = Color.white;

			if (isEditing)
			{
				GUI.SetNextControlName(STR_DEFINE_NAME_TEXT_FIELD);
				editingName = EditorGUILayout.TextField(editingName, GUILayout.MinWidth(DEFINE_LABEL_LONG_WIDTH));
				if (isEditingName == false)
				{
					isEditingName = true;
					GUI.FocusControl(STR_DEFINE_NAME_TEXT_FIELD);
				}
			}
			else
			{
				GUIStyle style = isEnabledAll ? EditorStyles.whiteLabel : EditorStyles.miniLabel;

				if (isSelected)
				{
					EditorGUILayout.LabelField(defineItem.Nombre, style, GUILayout.MinWidth(DEFINE_LABEL_WIDTH));
					if (GUILayout.Button(StringsUEME.RENOMBRAR_MDEFINE, GUILayout.Width(BUTTON_WIDTH)))
					{
						BeginRename(defineItem);
						isEditingName = false;
					}
					if (GUILayout.Button(StringsUEME.BORRAR_MDEFINE, GUILayout.Width(BUTTON_WIDTH)))
					{
						listDeleteReserved.Add(defineItem.Nombre);
					}
				}
				else
				{
					EditorGUILayout.LabelField(defineItem.Nombre, style, GUILayout.MinWidth(DEFINE_LABEL_LONG_WIDTH));
				}
			}

			GUI.backgroundColor = isEnabledAll ? Color.cyan : Color.white;
			bool newValue = GUILayout.Toggle(isEnabledAll, "All", EditorStyles.miniButtonLeft, GUILayout.Width(BUTTON_WIDTH));
			if (newValue != isEnabledAll)
			{
				defineItem.SetEnableAll(newValue);
				SetSelectedDefine(defineItem);
			}

			OnGUIDefineItemToggle(defineItem, COMPILER.CSHARP);
			OnGUIDefineItemToggle(defineItem, COMPILER.CSHARP_EDITOR);
			OnGUIDefineItemToggle(defineItem, COMPILER.UNITY_SCRIPT);

			EditorGUILayout.EndHorizontal();

			ProcessInput(rtItem, defineItem);
		}

		/// <summary>
		/// <para>Cuando hay un item en el toggle</para>
		/// </summary>
		/// <param name="defineItem"></param>
		/// <param name="eType"></param>
		private void OnGUIDefineItemToggle(GlobalDefine defineItem, COMPILER eType)// Cuando hay un item en el toggle
		{
			GUIStyle togStyle = EditorStyles.miniButtonMid;
			if (eType == COMPILER.END)
				togStyle = EditorStyles.miniButtonRight;

			string name = "Unity";
			if (eType == COMPILER.CSHARP)
				name = "C#";
			else if (eType == COMPILER.CSHARP_EDITOR)
				name = "C# editor";

			bool isEnabled = defineItem.IsEnabled(eType);
			GUI.backgroundColor = isEnabled ? Color.cyan : Color.white;
			bool newValue = GUILayout.Toggle(isEnabled, name, togStyle, GUILayout.Width(BUTTON_WIDTH));
			if (newValue == isEnabled)
				return;
			defineItem.SetEnable(eType, newValue);
			SetSelectedDefine(defineItem);
		}
		#endregion

		#region Metodos
		/// <summary>
		/// <para>Cuando esta activo</para>
		/// </summary>
		private void OnEnable()// Cuando esta activo
		{
			definesSaved.CargarSettings();
			definesEditing.CargarSettings();
			newDefineName = CrearNuevoNombreDefine();
		}

		/// <summary>
		/// <para>Crea un nuevo nombre</para>
		/// </summary>
		/// <returns></returns>
		private string CrearNuevoNombreDefine()// Crea un nuevo nombre
		{
			string newName = DEFAULT_NAME;
			int index = 1;
			while (definesEditing.IsDefined(newName))
			{
				++index;
				newName = DEFAULT_NAME + "_" + index.ToString();
			}
			return newName;
		}
		#endregion

		#region Inputs Eventos
		/// <summary>
		/// <para>Seleccionar define</para>
		/// </summary>
		/// <param name="define"></param>
		private void SetSelectedDefine(GlobalDefine define)// Seleccionar define
		{
            if (define == null)
                selectedDefine = "";
            else
                selectedDefine = define.Nombre;
            isInputProcessed = true;
        }

		/// <summary>
		/// <para>Determina si esta seleccionado</para>
		/// </summary>
		/// <param name="define"></param>
		/// <returns></returns>
		private bool IsSelectedDefine(GlobalDefine define)// Determina si esta seleccionado
		{
            if (define == null)
                return false;

            return selectedDefine == define.Nombre;
        }

		/// <summary>
		/// <para>Determina si se esta editando</para>
		/// </summary>
		/// <param name="define"></param>
		/// <returns></returns>
		private bool IsEditingDefine(GlobalDefine define)// Determina si se esta editando
		{
            if (define == null)
                return false;

            return editingDefine == define.Nombre;
        }

		/// <summary>
		/// <para>Proceso input</para>
		/// </summary>
		/// <param name="rtItem"></param>
		/// <param name="define"></param>
		private void ProcessInput(Rect rtItem, GlobalDefine define)// Proceso input
		{
            if (isInputProcessed == true || define == null)
                return;

            if (ProcessRename(rtItem, define))
            {
                isInputProcessed = true;
            }
            else if (ProcessMouse(rtItem, define))
            {
                isInputProcessed = true;
            }
        }

		/// <summary>
		/// <para>Renombrar</para>
		/// </summary>
		/// <param name="rtItem"></param>
		/// <param name="define"></param>
		/// <returns></returns>
		private bool ProcessRename(Rect rtItem, GlobalDefine define)// Renombrar
		{
            if (define == null || define.Nombre != editingDefine)
                return false;

            bool isInvalid = false;
            if (GlobalDefine.IsValidName(editingName) == false)
            {
                EditorGUILayout.HelpBox(StringsUEME.NOMBRE_INVALIDO_MDEFINE, MessageType.Error);
                isInvalid = true;
            }
            else if (editingName != define.Nombre && definesEditing.IsDefined(editingName))
            {
                EditorGUILayout.HelpBox(StringsUEME.DEFINE_EXISTE_MDEFINE, MessageType.Error);
                isInvalid = true;
            }

            if ((Event.current.type == EventType.MouseDown && !UtilEvent.IsClicked(rtItem)) ||
                (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return))
            {
                if (isInvalid)
                    EndRename();
                else
                    doRename = true;

                Event.current.Use();
                return true;
            }

            return false;
        }

		/// <summary>
		/// <para>Iniciando renombrado</para>
		/// </summary>
		/// <param name="define"></param>
		private void BeginRename(GlobalDefine define)// Iniciando renombrado
		{
            if (define == null)
                return;

            editingDefine = define.Nombre;
            editingName = define.Nombre;
        }

		/// <summary>
		/// <para>Finalizando renombrado</para>
		/// </summary>
		private void EndRename()// Finalizando renombrado
		{
            editingDefine = "";
            editingName = "";
            GUI.FocusControl("");
        }

		/// <summary>
		/// <para>Procesos mouse</para>
		/// </summary>
		/// <param name="rtItem"></param>
		/// <param name="define"></param>
		/// <returns></returns>
        private bool ProcessMouse(Rect rtItem, GlobalDefine define)// Procesos mouse
		{
            if (define == null || UtilEvent.IsClicked(rtItem) == false)
                return false;

            if (string.IsNullOrEmpty(editingDefine) == false)
            {
                EndRename();
            }
            if (IsSelectedDefine(define))
            {
                SetSelectedDefine(null);
            }
            else
            {
                SetSelectedDefine(define);
            }

            Event.current.Use();
            return true;
        }

		/// <summary>
		/// <para>Mouse soltado</para>
		/// </summary>
        public void ProcessMouseOut()// Mouse soltado
		{
            if (isInputProcessed == true)
                return;

            if (Event.current.type == EventType.MouseDown)
            {
                newDefineName = CrearNuevoNombreDefine();
                EndRename();
                Event.current.Use();
            }
        }
        #endregion
    }
}