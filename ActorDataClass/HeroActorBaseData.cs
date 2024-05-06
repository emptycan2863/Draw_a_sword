using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroActorBaseData {
    int index = 0;
    string codeName = "";
    int actorNameIndex = 0;

    string actorStandName = "";
    string actorIconName = "";
    string actorIllustName = "";
    string startDeckName = "";
    int expType = 0;

    static Dictionary<int, HeroActorBaseData> heroActorData = new Dictionary<int, HeroActorBaseData>();
    static Dictionary<string, HeroActorBaseData> heroActorDataInCodeName = new Dictionary<string, HeroActorBaseData>();

    static public void SetData(TextAsset tableData) {
        heroActorData.Clear();

        string[] dataList = tableData.text.Split("\r\n");

        foreach (string data in dataList) {
            if (data.Substring(0, 2) == "//") continue;

            HeroActorBaseData newData = new HeroActorBaseData();
            string[] values = data.Split("\t");

            newData.index = int.Parse(values[0]);
            newData.codeName = values[1];
            newData.actorNameIndex = int.Parse(values[2]);
            newData.actorStandName = values[3];
            newData.actorIconName = values[4];
            newData.actorIllustName = values[5];
            newData.expType = int.Parse(values[6]);
            newData.startDeckName = values[7];

            heroActorData.Add(newData.index, newData);
            heroActorDataInCodeName.Add(newData.codeName, newData);
        }
    }

    static public string GetActorIconName(string codeName) {
        if (heroActorDataInCodeName.ContainsKey(codeName)) {
            return heroActorDataInCodeName[codeName].actorIconName;
        }
        return "none";
    }

    public static HeroActorBaseData GetHeroData(int index) {
        return heroActorData[index];
    }

    public int GetActorNameIndex() {
        return actorNameIndex;
    }

    public string GetActorStandName() {
        return actorStandName;
    }

    public string GetIconName() {
        return actorIconName;
    }

    public int GetExpType() {
        return expType;
    }

    public List<int> GetStartDeck() {
        return DataManager.GetStartDeck(startDeckName);
    }

    public string GetCodeName() {
        return codeName;
    }
}
