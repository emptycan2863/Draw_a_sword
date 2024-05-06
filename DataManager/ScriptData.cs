using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum _language {
    kr, en
}

static public class ScriptData{
    static _language language = _language.kr;

    public class Script {
        int index = 0;
        string kr = "";
        string en = "";

        public Script(string data) {
            string[] values = data.Split("\t");

            index = int.Parse(values[0]);
            kr = values[1];
            en = values[2];
        }

        public int GetIndex() {
            return index;
        }

        public string GetSctict(_language lan) {
            string str = "";
            switch (lan) {
                case _language.kr:
                    str = kr;
                    break;
                case _language.en:
                    str = en;
                    break;
            }
            return str;
        }
    }
    static Dictionary<int, Script> scriptList = new Dictionary<int, Script>();

    static public void SetData(TextAsset tableData) {
        scriptList.Clear();

        string[] dataList = tableData.text.Split("\r\n");

        foreach (string data in dataList) {
            if (data.Substring(0, 2) == "//") continue;

            Script newData = new Script(data);
            scriptList.Add(newData.GetIndex(), newData);
        }
    }

    static public string GetScript(int index) {
        if (!scriptList.ContainsKey(index)) return "";
        string str = scriptList[index].GetSctict(language);
        str = str.Replace("\\n", "\r\n");
        return str;
    }
}
