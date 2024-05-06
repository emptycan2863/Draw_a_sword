using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ScriptData;

public class StageData
{
    static Dictionary<int, StageData> stageList = new Dictionary<int, StageData>();
    int index = 0;

    int[] enemyList = new int[8];
    int expReward = 0;
    List<int> cardReward = new List<int>();
    string sceneName = "";

    static public void SetData(TextAsset tableData) {
        stageList.Clear();

        string[] dataList = tableData.text.Split("\r\n");
        foreach (string data in dataList) {
            if (data.Substring(0, 2) == "//") continue;

            StageData newData = new StageData();
            string[] values = data.Split("\t");

            newData.index = int.Parse(values[0]);
            for (int i = 0, len = 8; i < len; ++i) {
                string str = values[i + 2];
                int v = str == "none" ? 0 : int.Parse(str);
                newData.enemyList[i] = v;
            }
            newData.expReward = int.Parse(values[10]);
            if (values[11] != "none" && values[11] != "") {
                string[] crList = values[11].Replace(" ", "").Split(",");
                foreach (string cr in crList) {
                    newData.cardReward.Add(int.Parse(cr));
                }
            }

            string newSceneName = values[12];
            if (newSceneName != "none") {
                newData.sceneName = newSceneName;
            } else {
                newData.sceneName = "Scene11TestBattleMap";
            }

            stageList.Add(newData.index, newData);
        }
    }

    static public int[] GetEnemys(int index) {
        return stageList[index].enemyList;
    }

    static public int GetExpReward(int index) {
        return stageList[index].expReward;
    }

    static public List<int> GetCardReward(int index) {
        return stageList[index].cardReward;
    }

    static public List<int> GetStageIndexList() {
        List<int> list = new List<int>();
        foreach (int i in stageList.Keys) {
            list.Add(i);
        }
        return list;
    }

    static public string GetSceneName(int index) {
        return stageList[index].sceneName;
    }
}