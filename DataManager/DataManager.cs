using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class DataManager
{
    public static GameObject card_CardSelectPf = null;
    public static GameObject card_HandCardtPf = null;

    public static GameObject iconPF = null;
    public static GameObject uiGaugeHeroPF = null;
    public static GameObject uiGaugeEnemyPF = null;

    public static GameObject actorPositionObjPf = null;

    public static GameObject useCardEffect = null;
    public static GameObject hitEffect = null;
    public static GameObject guardEffect = null;
    public static GameObject shildEffect = null;
    public static GameObject evaEffect = null;

    public static GameObject selectEffect = null;
    public static GameObject myTurnEffect = null;

    static List<List<int>> expData = new List<List<int>>();

    static Dictionary<string, int> cardTagData = new Dictionary<string, int>();
    static Dictionary<string, List<int>> startDeck = new Dictionary<string, List<int>>();
    static public void SetCardTagData(TextAsset tableData) {
        cardTagData.Clear();

        string[] dataList = tableData.text.Split("\r\n");

        foreach (string data in dataList) {
            if (data.Substring(0, 2) == "//") continue;

            string[] values = data.Split("\t");

            cardTagData.Add(values[0], int.Parse(values[1]));
        }
    }
    static public string GetCardTag(string key) {
        if (!cardTagData.ContainsKey(key)) return "";
        return ScriptData.GetScript(cardTagData[key]);
    }

    static public void SetExpData(TextAsset tableData) {
        expData.Clear();

        string[] dataList = tableData.text.Split("\r\n");

        foreach (string data in dataList) {
            string[] values = data.Split("\t");
            if (data.Substring(0, 2) == "//") {
                for (int i = 0, len = values.Length - 1; i<len; ++i) {
                    expData.Add(new List<int>());
                }
            } else {
                for (int i = 0, len = values.Length - 1; i < len; ++i) {
                    expData[i].Add(int.Parse(values[i+1]));
                }
            }
        }
    }

    static public int GetLvUpExp(int lv, int type = 0) {
        if (expData.Count <= type) type = 0;
        if (expData[type].Count <= lv) return -1;
        return expData[type][lv-1];
    }

    static public void SetStartDeck(TextAsset tableData) {
        startDeck.Clear();

        string[] dataList = tableData.text.Split("\r\n");
        List<string> keys = new List<string>();

        foreach (string data in dataList) {
            string[] values = data.Split("\t");
            if (data.Substring(0, 2) == "//") {
                for (int i = 0, len = values.Length - 1; i < len; ++i) {
                    string key = values[i + 1];
                    startDeck.Add(key, new List<int>());
                    keys.Add(key);
                }
            } else {
                for (int i = 0, len = values.Length - 1; i < len; ++i) {
                    if (values[i + 1] == "none") {
                        startDeck[keys[i]].Add(-1);
                    } else {
                        startDeck[keys[i]].Add(int.Parse(values[i + 1]));
                    }
                }
            }
        }
    }

    static public List<int> GetStartDeck(string key) {
        if (!startDeck.ContainsKey(key)) return new List<int>();
        return startDeck[key];
    }
}
