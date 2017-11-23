//                                  ┌∩┐(◣_◢)┌∩┐
//																				\\
// UtilEvent.cs (22/11/2017)													\\
// Autor: Antonio Mateo (.\Moon Antonio) 	antoniomt.moon@gmail.com			\\
// Descripcion:		Utilidades para los eventos.								\\
// Fecha Mod:		22/11/2017													\\
// Ultima Mod:		Version Inicial												\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
#endregion

namespace MoonAntonio.UEME.MDefine
{
	/// <summary>
	/// <para>Utilidades para los eventos</para>
	/// </summary>
	public static class UtilEvent
    {
		#region API
		/// <summary>
		/// <para>Si el mouse entra.</para>
		/// </summary>
		/// <param name="rect"></param>
		/// <returns></returns>
		public static bool IsMouseOn(Rect rect)// Si el mouse entra
		{
			return rect.Contains(Event.current.mousePosition);
		}

		/// <summary>
		/// <para>Si se clica</para>
		/// </summary>
		/// <param name="rect"></param>
		/// <returns></returns>
		public static bool IsClicked(Rect rect)// Si se clica
		{
			return Event.current.type == EventType.MouseDown && IsMouseOn(rect);
		}

		/// <summary>
		/// <para>Si se presiona enter.</para>
		/// </summary>
		/// <returns></returns>
		public static bool IsEnterPressed()// Si se presiona enter
		{
			if (Event.current.isKey && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)) return true;

			return false;
		}

		/// <summary>
		/// <para>Si se usa algun numerico</para>
		/// </summary>
		/// <returns></returns>
		public static bool IsInputUnsignedNumber()// Si se usa algun numerico
		{
			if (Event.current.isKey)
			{
				char inputChar = Event.current.character;
				if (inputChar < '0' || inputChar > '9')
				{
					return false;
				}
			}
			return true;
		}
		#endregion
	}
}