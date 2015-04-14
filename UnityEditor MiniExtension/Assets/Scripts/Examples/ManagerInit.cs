/*  
    Autor: Antonio Mateo Tomas (lPinchol)
    Date: 14/04/2015
    GitHub: https://github.com/lPinchol
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ManagerInit : MonoBehaviour 
{

    public string dirIni;
    IniCore ini = new IniCore();
    public GameObject btnLoadDate;
    public GameObject btnWriteDate;
    public GameObject btnClose;
    public GameObject Input;

    public Text nameText;
    public InputField inputText;

    public bool isOpenIni = false;

    private void Update()
    {
        if (isOpenIni)
        {
            btnLoadDate.SetActive(true);
            btnClose.SetActive(true);
            Input.SetActive(true);
            if (inputText.text != "")
            {
                btnWriteDate.SetActive(true);
            }
            else
            {
                btnWriteDate.SetActive(false);
            }
        }
        else
        {
            btnLoadDate.SetActive(false);
            btnClose.SetActive(false);
            Input.SetActive(false);
            btnWriteDate.SetActive(false);
        }
    }

    public void OpenIni()
    {
        ini.Open(dirIni);
        isOpenIni = true;
    }

    public void LoadDate()
    {
        nameText.text = ini.ReadValue("Player", "name", "Player01").ToString();
    }

    public void WriteDate()
    {
        ini.WriteValue("Player", "name", inputText.text.ToString());
    }

    public void Close()
    {
        ini.Close();
        isOpenIni = false;
    }
}
