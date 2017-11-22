//                                  ┌∩┐(◣_◢)┌∩┐
//																				\\
// UEMEMenus.cs (22/11/2017)													\\
// Autor: Antonio Mateo (.\Moon Antonio) 	antoniomt.moon@gmail.com			\\
// Descripcion:		Centro de menus de las herramientas de UEME					\\
// Fecha Mod:		22/11/2017													\\
// Ultima Mod:		Version Inicial												\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
using UnityEditor;
using MoonAntonio.UEME.MDefine;
using MoonAntonio.UEME.MConsola;
using System.Diagnostics;
#endregion

namespace MoonAntonio.UEME
{
	/// <summary>
	/// <para>Centro de menus de las herramientas de UEME</para>
	/// </summary>
	public class UEMEMenus : MonoBehaviour 
	{
		#region Menu Define Manager
		[MenuItem(StringsUEME.NOMBRE_DIRECCION_EDIT_MDEFINE)]
		[MenuItem(StringsUEME.NOMBRE_DIRECCION_BARRA_MDEFINE)]
		public static void MDefineMnaager()
		{
			DefineManager window = EditorWindow.GetWindow<DefineManager>(false, StringsUEME.NOMBRE_MENU_MDEFINE, true);
			if (window != null)
			{
				window.position = new Rect(200, 200, 515, 300);
				window.minSize = new Vector2(515f, 200f);
			}
		}
		#endregion

		#region Menu Reiniciar Nivel
		[MenuItem(StringsUEME.NOMBRE_MENU_REINICIAR_NIVEL, priority = 150)]
		private static void ReiniciarRunTime()
		{
			EditorApplication.isPlaying = false;
			EditorApplication.update += Reiniciando;
		}

		private static void Reiniciando()
		{
			if (EditorApplication.isPlaying) return;
			EditorApplication.isPlaying = true;
			EditorApplication.update -= Reiniciando;
		}
		#endregion

		#region Menu Reiniciar Editor
		[MenuItem(StringsUEME.NOMBRE_MENU_REINICIO_EDITOR)]
		private static void Reinicio()
		{
			var filename = EditorApplication.applicationPath;
			var arguments = "-projectPath " + Application.dataPath.Replace("/Assets", string.Empty);
			var startInfo = new ProcessStartInfo
			{
				FileName = filename,
				Arguments = arguments,
			};
			Process.Start(startInfo);

			EditorApplication.Exit(0);
		}
		#endregion

		#region Menu Consola
		[MenuItem(StringsUEME.NOMBRE_MENU_CONSOLA)]
		public static void Consola()
		{
			GameObject go = new GameObject();
			go.name = StringsUEME.NOMBRE_CONSOLA;
			go.gameObject.AddComponent<Consola>();
		}
		#endregion
	}
}