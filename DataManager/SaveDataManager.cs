using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using UnityEngine;

public class SaveDataManager {
    static string saveVersion = "test240221";
    static JsObject[] saveDataList = new JsObject[9];
    static int saveSlotIndex = -1;

    static public string GetSaveVersion() { return saveVersion; }

    static public void SaveDataInit() {
        if (!Directory.Exists(Application.streamingAssetsPath + "/Save/")) {
            Directory.CreateDirectory(Application.streamingAssetsPath + "/Save/");
        }

        for (int i = 0, len = saveDataList.Length; i < len; ++i) {
            string fileName = "SAVE" + i + ".sav";
            string path = Application.streamingAssetsPath + "/Save/" + fileName;
            FileInfo fi = new FileInfo(path);

            if (fi.Exists) {
                StreamReader reader = new StreamReader(path);
                string json = ARS.Decrypt(reader.ReadToEnd());
                reader.Close();

                JsObject loadJsObject = JsObject.ParseJson(json);
                if (DataCheck(loadJsObject)) {
                    saveDataList[i] = loadJsObject;
                } else {
                    DeleteSaveData(i);
                }
            } else {
                saveDataList[i] = null;
            }
        }
    }

    static public void DeleteSaveData(int index) {
        saveDataList[index] = null;

        string fileName = "SAVE" + index + ".sav";
        string path = Application.streamingAssetsPath + "/Save/" + fileName;
        FileInfo fi = new FileInfo(path);

        if (fi.Exists) {
            File.Delete(path);
        }
    }

    static bool DataCheck(JsObject data) {
        if (!data.GetDictionary().ContainsKey("saveVersion")) return false;
        string dataVersion = data.GetDictionary()["saveVersion"].GetString();
        if (dataVersion != saveVersion && !VersionConvert(data)) return false;
        return true;
    }


    static bool VersionConvert(JsObject data) {
        return false;
    }
    static public JsObject GetSaveData(int index) {
        return saveDataList[index];
    }

    static public void SetSaveIndex(int index) {
        saveSlotIndex = index;
    }

    public static void Save(JsObject saveData) {
        if (saveSlotIndex == -1) return;
        int index = saveSlotIndex;
        saveDataList[index] = saveData;

        string fileName = "SAVE" + index + ".sav";
        string path = Application.streamingAssetsPath + "/Save/" + fileName;

        string json = saveData.GetJson();
        string arsData = ARS.Encrypt(json);
        File.WriteAllText(path, arsData);
    }

    //------------------

    class JsonTest2 {
        public string A = "AAA";
    }

    class JsonTest {
        public int a = 1;
        public int B = 100;
        public float c = 100.2f;
        public string d = "hello";
        public int count = 1;
        public JsonTest2 dc = new JsonTest2();
    }

    static void SaveTest() {
        string name = "testText.txt";
        string path = Application.streamingAssetsPath + "/Save/" + name;

        FileInfo nf = new FileInfo(path);
        JsonTest data = null;

        if (nf.Exists) {
            StreamReader reader = new StreamReader(path);
            string LoadData = reader.ReadToEnd();
            LoadData = ARS.Decrypt(LoadData);
            data = JsonUtility.FromJson<JsonTest>(LoadData);
            data.dc.A = data.dc.A + "B";
            reader.Close();
        } else {
            data = new JsonTest();
        }
        ++data.count;

        if (data.count > 3) {
            File.Delete(path);
        } else {
            string saveData = JsonUtility.ToJson(data);
            saveData = ARS.Encrypt(saveData);
            File.WriteAllText(path, saveData);
        }


        JsObject testSaveData = new JsObject(_jsDataType._list);

        JsObject strData = new JsObject("zzzz");
        testSaveData.GetList().Add(strData);
        JsObject listData = new JsObject(_jsDataType._list);
        testSaveData.GetList().Add(listData);
        JsObject dicData = new JsObject(_jsDataType._dictionary);
        testSaveData.GetList().Add(dicData);

        for (int i = 0, len = 500; i < len; ++i) {
            string key = "as" + i;

            listData.GetList().Add(new JsObject(i));
            dicData.GetDictionary().Add(key, new JsObject(i));
        }
        string json = testSaveData.GetJson();
        Debug.Log(JsObject.ParseJson(json).GetList()[1].GetList().Count);
    }
}
