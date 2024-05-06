using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//데이터를 저장하거나 불러올 떄 사용하기 위하여, javascript 스타일의 오브젝트 형식으로 데이터를 관리하고 json 변환할 수 있도록 만든 유틸.

public enum _jsDataType {
    _null, _string, _list, _dictionary
}

public class JsObject {
    _jsDataType dataType = _jsDataType._null;
    string data = "";
    List<JsObject> list = null;
    Dictionary<string, JsObject> dictionary = null;

    public JsObject() {
    }

    public JsObject(string _data) {
        Set(_data);
    }
    public JsObject(int _data) {
        Set(_data);
    }
    public JsObject(float _data) {
        Set(_data);
    }
    public JsObject(List<JsObject> _data) {
        Set(_data);
    }
    public JsObject(Dictionary<string, JsObject> _data) {
        Set(_data);
    }

    public JsObject(_jsDataType type) {
        dataType = type;

        switch (type) {
            case _jsDataType._list:
                list = new List<JsObject>();
                break;
            case _jsDataType._dictionary:
                dictionary = new Dictionary<string, JsObject>();
                break;
        }
    }

    public _jsDataType TypeOf() { return dataType; }

    public void SetNull() {
        dataType = _jsDataType._null;
        data = "";
        list = null;
        dictionary = null;
    }

    public void Set(string stringData) {
        dataType = _jsDataType._string;
        data = stringData;
        list = null;
        dictionary = null;
    }

    public void Set(int intData) {
        dataType = _jsDataType._string;
        data = "" + intData;
        list = null;
        dictionary = null;
    }

    public void Set(float floatData) {
        dataType = _jsDataType._string;
        data = "" + floatData;
        list = null;
        dictionary = null;
    }

    public void Set(List<JsObject> listData) {
        if (listData == null) {
            SetNull();
            return;
        }
        dataType = _jsDataType._list;
        data = "";
        list = listData;
        dictionary = null;
    }

    public void Set(Dictionary<string, JsObject> dictionaryData) {
        if (dictionaryData == null) {
            SetNull();
            return;
        }
        dataType = _jsDataType._dictionary;
        data = "";
        list = null;
        dictionary = dictionaryData;
    }

    public string GetString() { return data; }
    public int GetInt() {
        if (data == "") return 0;
        return int.Parse(data);
    }
    public float GetFloat() {
        if (data == "") return 0;
        return float.Parse(data);
    }
    public List<JsObject> GetList() { return list; }
    public Dictionary<string, JsObject> GetDictionary() { return dictionary; }

    public string GetJson() {
        string json = "";
        switch (dataType) {
            case _jsDataType._null:
                json = "null";
                break;
            case _jsDataType._string:
                json = "\"" + data + "\"";
                break;
            case _jsDataType._list:
                json = "[";
                for (int i = 0, len = list.Count; i < len; ++i) {
                    json += list[i].GetJson();
                    if (i + 1 < len) json += ", ";
                }
                json += "]";
                break;
            case _jsDataType._dictionary:
                json = "{";
                int count = 0;
                int keysLen = dictionary.Count;
                foreach (string key in dictionary.Keys) {
                    json += "\"" + key + "\" : " + dictionary[key].GetJson();
                    ++count;
                    if (count < keysLen) json += ", ";
                }
                json += "}";
                break;
        }
        return json;
    }

    public class StringPoint {
        string str = "";
        int point = 0;

        public StringPoint(string _str) {
            str = _str;
        }

        public char GetChar() {
            if (str.Length <= point) return '\0';
            char c = str[point];
            return c;
        }

        public int GetLength() {
            return str.Length;
        }

        public void SkipBlink() {
            bool w = true;
            while (w) {
                switch (GetChar()) {
                    case ' ':
                        Next();
                        break;
                    case '\r':
                        Next();
                        break;
                    case '\n':
                        Next();
                        break;
                    case '\t':
                        Next();
                        break;
                    default:
                        w = false;
                        break;
                }
            }
        }

        public void SkipNullData() {
            bool w = true;
            while (w) {
                switch (GetChar()) {
                    case '"':
                        w = false;
                        break;
                    case '[':
                        w = false;
                        break;
                    case ']':
                        w = false;
                        break;
                    case '{':
                        w = false;
                        break;
                    case '}':
                        w = false;
                        break;
                    case ',':
                        w = false;
                        break;
                    case '\0':
                        w = false;
                        break;
                    default:
                        Next();
                        w = true;
                        break;
                }
            }
        }

        public void Next() {
            point++;
        }
    }

    public static JsObject ParseJson(string json) {
        JsObject newObject = new JsObject();

        StringPoint jsonPoint = new StringPoint(json);
        newObject.SetJson(jsonPoint);

        return newObject;
    }

    public void SetJson(StringPoint jsonPoint) {
        jsonPoint.SkipNullData();
        switch (jsonPoint.GetChar()) {
            case '"':
                ParseString(jsonPoint);
                break;
            case '[':
                ParseList(jsonPoint);
                break;
            case '{':
                ParseDictionary(jsonPoint);
                break;
            default:
                SetNull();
                break;
        }
    }

    void ParseString(StringPoint jsonPoint) {
        jsonPoint.Next();
        string strData = "";
        while (jsonPoint.GetChar() != '"') {
            char c = jsonPoint.GetChar();
            if (c == '\0') {
                SetNull();
                return;
            }
            strData += c;
            jsonPoint.Next();
        }
        jsonPoint.Next();
        Set(strData);
    }

    void ParseList(StringPoint jsonPoint) {
        jsonPoint.Next();

        List<JsObject> newList = new List<JsObject>();

        while (jsonPoint.GetChar() != ']') {
            jsonPoint.SkipBlink();
            JsObject newObj = new JsObject();
            newObj.SetJson(jsonPoint);
            newList.Add(newObj);
            jsonPoint.SkipBlink();
            if (jsonPoint.GetChar() == ',') jsonPoint.Next();
            if (jsonPoint.GetChar() == '\0') {
                SetNull();
                return;
            }
        }

        jsonPoint.Next();
        Set(newList);
    }

    void ParseDictionary(StringPoint jsonPoint) {
        jsonPoint.Next();

        Dictionary<string, JsObject> newDictionary = new Dictionary<string, JsObject>();

        while (jsonPoint.GetChar() != '}') {
            jsonPoint.SkipBlink();
            char c = jsonPoint.GetChar();
            if (c != '"') {
                SetNull();
                return;
            }

            string key = "";
            jsonPoint.Next();
            while (jsonPoint.GetChar() != '"') {
                c = jsonPoint.GetChar();
                if (c == '\0') {
                    SetNull();
                    return;
                }
                key += c;
                jsonPoint.Next();
            }
            jsonPoint.Next();

            jsonPoint.SkipBlink();
            if (jsonPoint.GetChar() != ':') {
                SetNull();
                return;
            }
            jsonPoint.Next();
            jsonPoint.SkipBlink();

            JsObject newObj = new JsObject();
            newObj.SetJson(jsonPoint);
            newDictionary.Add(key, newObj);
            jsonPoint.SkipBlink();
            if (jsonPoint.GetChar() == ',') jsonPoint.Next();
            if (jsonPoint.GetChar() == '\0') {
                SetNull();
                return;
            }
        }

        jsonPoint.Next();
        Set(newDictionary);
    }
}