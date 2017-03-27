/*  
    Autor: Antonio Mateo Tomas (lPinchol)
    Date: 14/04/2015
    GitHub: https://github.com/lPinchol
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// <para>Class InitCore, Open saved file and read the Init.</para>
/// </summary>
public class IniCore
{
    #region Variables
    public int error                        = 0;
    private object m_Lock                   = new object();
    private string m_FileName               = null;
    private bool m_AutoFlush                = false;
    private string m_iniString              = null;
    private bool m_CacheModified            = false;
    private Dictionary<string, Dictionary<string, string>> m_Sections = new Dictionary<string, Dictionary<string, string>>();
    private Dictionary<string, Dictionary<string, string>> m_Modified = new Dictionary<string, Dictionary<string, string>>();

    public string FileName
    {
        get
        {
            return m_FileName;
        }
    }
    public string iniString
    {
        get
        {
            return m_iniString;
        }
    }
    #endregion

    #region "Methods"
    public void Open(string path)
    {
        m_FileName = path;

        if (File.Exists(m_FileName))
        {
            m_iniString = File.ReadAllText(m_FileName);
        }
        else
        {
            var temp = File.Create(m_FileName);
            temp.Close();
            m_iniString = "";
        }

        Initialize(m_iniString, false);
    }

    public void Open(TextAsset name)
    {
        if (name == null)
        {
            error = 1;
            m_iniString = "";
            m_FileName = null;
            Initialize(m_iniString, false);
        }
        else
        {
            m_FileName = Application.persistentDataPath + name.name;

            if (File.Exists(m_FileName))
            {
                m_iniString = File.ReadAllText(m_FileName);
            }
            else m_iniString = name.text;
            Initialize(m_iniString, false);
        }
    }

    public void OpenFromString(string str)
    {
        m_FileName = null;
        Initialize(str, false);
    }

    public override string ToString()
    {
        return m_iniString;
    }

    private void Initialize(string iniString, bool AutoFlush)
    {
        m_iniString = iniString;
        m_AutoFlush = AutoFlush;
        Refresh();
    }

    public void Close()
    {
        lock (m_Lock)
        {
            PerformFlush();

            m_FileName = null;
            m_iniString = null;
        }
    }

    private string ParseSectionName(string Line)
    {
        if (!Line.StartsWith("[")) return null;
        if (!Line.EndsWith("]")) return null;
        if (Line.Length < 3) return null;
        return Line.Substring(1, Line.Length - 2);
    }

    private bool ParseKeyValuePair(string Line, ref string Key, ref string Value)
    {
        int i;
        if ((i = Line.IndexOf('=')) <= 0) return false;

        int j = Line.Length - i - 1;
        Key = Line.Substring(0, i).Trim();
        if (Key.Length <= 0) return false;

        Value = (j > 0) ? (Line.Substring(i + 1, j).Trim()) : ("");
        return true;
    }

    private bool isComment(string Line)
    {
        string tmpKey = null, tmpValue = null;
        if (ParseSectionName(Line) != null) return false;
        if (ParseKeyValuePair(Line, ref tmpKey, ref tmpValue)) return false;
        return true;
    }

    private void Refresh()
    {
        lock (m_Lock)
        {
            StringReader sr = null;
            try
            {
                m_Sections.Clear();
                m_Modified.Clear();

                sr = new StringReader(m_iniString);

                Dictionary<string, string> CurrentSection = null;
                string s;
                string SectionName;
                string Key = null;
                string Value = null;
                while ((s = sr.ReadLine()) != null)
                {
                    s = s.Trim();

                    SectionName = ParseSectionName(s);
                    if (SectionName != null)
                    {
                        if (m_Sections.ContainsKey(SectionName))
                        {
                            CurrentSection = null;
                        }
                        else
                        {
                            CurrentSection = new Dictionary<string, string>();
                            m_Sections.Add(SectionName, CurrentSection);
                        }
                    }
                    else if (CurrentSection != null)
                    {
                        if (ParseKeyValuePair(s, ref Key, ref Value))
                        {
                            if (!CurrentSection.ContainsKey(Key))
                            {
                                CurrentSection.Add(Key, Value);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (sr != null) sr.Close();
                sr = null;
            }
        }
    }

    private void PerformFlush()
    {
        if (!m_CacheModified) return;
        m_CacheModified = false;

        StringWriter sw = new StringWriter();

        try
        {
            Dictionary<string, string> CurrentSection = null;
            Dictionary<string, string> CurrentSection2 = null;
            StringReader sr = null;
            try
            {
                sr = new StringReader(m_iniString);

                string s;
                string SectionName;
                string Key = null;
                string Value = null;
                bool Unmodified;
                bool Reading = true;

                bool Deleted = false;
                string Key2 = null;
                string Value2 = null;

                StringBuilder sb_temp;

                while (Reading)
                {
                    s = sr.ReadLine();
                    Reading = (s != null);

                    if (Reading)
                    {
                        Unmodified = true;
                        s = s.Trim();
                        SectionName = ParseSectionName(s);
                    }
                    else
                    {
                        Unmodified = false;
                        SectionName = null;
                    }

                    if ((SectionName != null) || (!Reading))
                    {
                        if (CurrentSection != null)
                        {
                            if (CurrentSection.Count > 0)
                            {
                                sb_temp = sw.GetStringBuilder();
                                while ((sb_temp[sb_temp.Length - 1] == '\n') || (sb_temp[sb_temp.Length - 1] == '\r'))
                                {
                                    sb_temp.Length = sb_temp.Length - 1;
                                }
                                sw.WriteLine();

                                foreach (string fkey in CurrentSection.Keys)
                                {
                                    if (CurrentSection.TryGetValue(fkey, out Value))
                                    {
                                        sw.Write(fkey);
                                        sw.Write('=');
                                        sw.WriteLine(Value);
                                    }
                                }
                                sw.WriteLine();
                                CurrentSection.Clear();
                            }
                        }

                        if (Reading)
                        {
                            if (!m_Modified.TryGetValue(SectionName, out CurrentSection))
                            {
                                CurrentSection = null;
                            }
                        }
                    }
                    else if (CurrentSection != null)
                    {
                        if (ParseKeyValuePair(s, ref Key, ref Value))
                        {
                            if (CurrentSection.TryGetValue(Key, out Value))
                            {
                                Unmodified = false;
                                CurrentSection.Remove(Key);

                                sw.Write(Key);
                                sw.Write('=');
                                sw.WriteLine(Value);
                            }
                        }
                    }

                    if (Unmodified)
                    {
                        if (SectionName != null)
                        {
                            if (!m_Sections.ContainsKey(SectionName))
                            {
                                Deleted = true;
                                CurrentSection2 = null;
                            }
                            else
                            {
                                Deleted = false;
                                m_Sections.TryGetValue(SectionName, out CurrentSection2);
                            }

                        }
                        else if (CurrentSection2 != null)
                        {
                            if (ParseKeyValuePair(s, ref Key2, ref Value2))
                            {
                                if (!CurrentSection2.ContainsKey(Key2)) Deleted = true;
                                else Deleted = false;
                            }
                        }
                    }


                    if (Unmodified)
                    {
                        if (isComment(s)) sw.WriteLine(s);
                        else if (!Deleted) sw.WriteLine(s);
                    }
                }

                sr.Close();
                sr = null;
            }
            finally
            {                 
                if (sr != null) sr.Close();
                sr = null;
            }

            foreach (KeyValuePair<string, Dictionary<string, string>> SectionPair in m_Modified)
            {
                CurrentSection = SectionPair.Value;
                if (CurrentSection.Count > 0)
                {
                    sw.WriteLine();

                    sw.Write('[');
                    sw.Write(SectionPair.Key);
                    sw.WriteLine(']');

                    foreach (KeyValuePair<string, string> ValuePair in CurrentSection)
                    {
                        sw.Write(ValuePair.Key);
                        sw.Write('=');
                        sw.WriteLine(ValuePair.Value);
                    }
                    CurrentSection.Clear();
                }
            }
            m_Modified.Clear();

            m_iniString = sw.ToString();
            sw.Close();
            sw = null;

            if (m_FileName != null)
            {
                File.WriteAllText(m_FileName, m_iniString);
            }
        }
        finally
        {               
            if (sw != null) sw.Close();
            sw = null;
        }
    }

    public bool IsSectionExists(string SectionName)
    {
        return m_Sections.ContainsKey(SectionName);
    }

    public bool IsKeyExists(string SectionName, string Key)
    {
        Dictionary<string, string> Section;

        if (m_Sections.ContainsKey(SectionName))
        {
            m_Sections.TryGetValue(SectionName, out Section);

            return Section.ContainsKey(Key);
        }
        else return false;
    }

    public void SectionDelete(string SectionName)
    {

        if (IsSectionExists(SectionName))
        {
            lock (m_Lock)
            {
                m_CacheModified = true;
                m_Sections.Remove(SectionName);
                m_Modified.Remove(SectionName);
                if (m_AutoFlush) PerformFlush();
            }
        }
    }

    public void KeyDelete(string SectionName, string Key)
    {
        Dictionary<string, string> Section;

        if (IsKeyExists(SectionName, Key))
        {
            lock (m_Lock)
            {
                m_CacheModified = true;
                m_Sections.TryGetValue(SectionName, out Section);
                Section.Remove(Key);
                if (m_Modified.TryGetValue(SectionName, out Section)) Section.Remove(SectionName);
                if (m_AutoFlush) PerformFlush();
            }
        }

    }

    public string ReadValue(string SectionName, string Key, string DefaultValue)
    {
        lock (m_Lock)
        {
            Dictionary<string, string> Section;
            if (!m_Sections.TryGetValue(SectionName, out Section)) return DefaultValue;

            string Value;
            if (!Section.TryGetValue(Key, out Value)) return DefaultValue;
            return Value;
        }
    }

    public void WriteValue(string SectionName, string Key, string Value)
    {
        lock (m_Lock)
        {
            m_CacheModified = true;
            Dictionary<string, string> Section;
            if (!m_Sections.TryGetValue(SectionName, out Section))
            {
                Section = new Dictionary<string, string>();
                m_Sections.Add(SectionName, Section);
            }

            if (Section.ContainsKey(Key)) Section.Remove(Key);
            Section.Add(Key, Value);

            if (!m_Modified.TryGetValue(SectionName, out Section))
            {
                Section = new Dictionary<string, string>();
                m_Modified.Add(SectionName, Section);
            }

            if (Section.ContainsKey(Key)) Section.Remove(Key);
            Section.Add(Key, Value);

            if (m_AutoFlush) PerformFlush();
        }
    }

    private string EncodeByteArray(byte[] Value)
    {
        if (Value == null) return null;

        StringBuilder sb = new StringBuilder();
        foreach (byte b in Value)
        {
            string hex = Convert.ToString(b, 16);
            int l = hex.Length;
            if (l > 2)
            {
                sb.Append(hex.Substring(l - 2, 2));
            }
            else
            {
                if (l < 2) sb.Append("0");
                sb.Append(hex);
            }
        }
        return sb.ToString();
    }

    private byte[] DecodeByteArray(string Value)
    {
        if (Value == null) return null;

        int l = Value.Length;
        if (l < 2) return new byte[] { };

        l /= 2;
        byte[] Result = new byte[l];
        for (int i = 0; i < l; i++) Result[i] = Convert.ToByte(Value.Substring(i * 2, 2), 16);
        return Result;
    }

    public bool ReadValue(string SectionName, string Key, bool DefaultValue)
    {
        string StringValue = ReadValue(SectionName, Key, DefaultValue.ToString(System.Globalization.CultureInfo.InvariantCulture));
        int Value;
        if (int.TryParse(StringValue, out Value)) return (Value != 0);
        return DefaultValue;
    }

    public int ReadValue(string SectionName, string Key, int DefaultValue)
    {
        string StringValue = ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
        int Value;
        if (int.TryParse(StringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value)) return Value;
        return DefaultValue;
    }

    public long ReadValue(string SectionName, string Key, long DefaultValue)
    {
        string StringValue = ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
        long Value;
        if (long.TryParse(StringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value)) return Value;
        return DefaultValue;
    }

    public double ReadValue(string SectionName, string Key, double DefaultValue)
    {
        string StringValue = ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
        double Value;
        if (double.TryParse(StringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value)) return Value;
        return DefaultValue;
    }

    public byte[] ReadValue(string SectionName, string Key, byte[] DefaultValue)
    {
        string StringValue = ReadValue(SectionName, Key, EncodeByteArray(DefaultValue));
        try
        {
            return DecodeByteArray(StringValue);
        }
        catch (FormatException)
        {
            return DefaultValue;
        }
    }

    public DateTime ReadValue(string SectionName, string Key, DateTime DefaultValue)
    {
        string StringValue = ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
        DateTime Value;
        if (DateTime.TryParse(StringValue, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AssumeLocal, out Value)) return Value;
        return DefaultValue;
    }

    public void WriteValue(string SectionName, string Key, bool Value)
    {
        WriteValue(SectionName, Key, (Value) ? ("1") : ("0"));
    }

    public void WriteValue(string SectionName, string Key, int Value)
    {
        WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
    }

    public void WriteValue(string SectionName, string Key, long Value)
    {
        WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
    }

    public void WriteValue(string SectionName, string Key, double Value)
    {
        WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
    }

    public void WriteValue(string SectionName, string Key, byte[] Value)
    {
        WriteValue(SectionName, Key, EncodeByteArray(Value));
    }

    public void WriteValue(string SectionName, string Key, DateTime Value)
    {
        WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
    }

    #endregion
}
