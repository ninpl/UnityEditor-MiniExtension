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
	}
}