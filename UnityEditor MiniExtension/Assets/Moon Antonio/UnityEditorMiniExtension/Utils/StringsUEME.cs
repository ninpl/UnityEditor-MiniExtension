//                                  ┌∩┐(◣_◢)┌∩┐
//																				\\
// StringsUEME.cs (22/11/2017)													\\
// Autor: Antonio Mateo (.\Moon Antonio) 	antoniomt.moon@gmail.com			\\
// Descripcion:		Constantes de las herramientas en UEME						\\
// Fecha Mod:		22/11/2017													\\
// Ultima Mod:		Version Inicial												\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
#endregion

namespace MoonAntonio.UEME
{
	/// <summary>
	/// <para>Constantes de las herramientas en UEME</para>
	/// </summary>
	public class StringsUEME : MonoBehaviour
	{
		#region Menus
		public const string NOMBRE_DIRECCION_EDIT_MDEFINE	= "Edit/Project Settings/Define";
		public const string NOMBRE_DIRECCION_BARRA_MDEFINE	= "MoonAntonio/Define Manager";
		public const string NOMBRE_MENU_REINICIAR_NIVEL		= "Edit/Reiniciar #&P";
		public const string NOMBRE_MENU_REINICIO_EDITOR		= "File/Reiniciar";
		public const string NOMBRE_MENU_CONSOLA				= "MoonAntonio/Consola";
		#endregion

		#region MDefine
		public const string NOMBRE_MENU_MDEFINE				= "Define Manager";
		public const string RSP_SMCS_MDEFINE				= "Assets/Moon Antonio/UnityEditorMiniExtension/Define Manager/smcs.rsp";
		public const string RSP_GMCS_MDEFINE				= "Assets/Moon Antonio/UnityEditorMiniExtension/Define Manager/gmcs.rsp";
		public const string RSP_US_MDEFINE					= "Assets/Moon Antonio/UnityEditorMiniExtension/Define Manager/us.rsp";
		public const string SETTING_MDEFINE					= "Assets/Moon Antonio/UnityEditorMiniExtension/Define Manager/DefineSettings.cs";
		public const string NOTICIA_MDEFINE					= "// NO EDITAR O ELIMINAR ESTE ARCHIVO. UTILIZADO PARA 'DefineManager'.";
		public const string APLICAR_MDEFINE					= "Aplicar";
		public const string REVERTIR_MDEFINE				= "Revertir";
		public const string AVISO_APLICAR_MDEFINE			= "Debes presionar el boton Aplicar para aplicar cambios";
		public const string NOMBRE_INVALIDO_MDEFINE			= "Nombre invalido";
		public const string DEFINE_EXISTE_MDEFINE			= "Ya existe";
		public const string ADD_DEFINE_MDEFINE				= "Agregar Define";
		public const string RENOMBRAR_MDEFINE				= "Renombrar";
		public const string BORRAR_MDEFINE					= "Borrar";
		#endregion

		#region MConsola
		public const string NOMBRE_CONSOLA					= "Consola";
		#endregion
	}
}