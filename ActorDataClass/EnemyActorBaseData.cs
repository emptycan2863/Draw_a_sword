using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActorBaseData
{
    int index = 0;
    string coments = "";

    int actorNameIndex = 0;

    int attack = 0;
    int hp = 0;
    int mp = 0;
    int sp = 0;

    int strength = 0;
    int intelligence = 0;
    int dexterity = 0;

    int rank = 0;
    List<int> deckData = new List<int>();

    string actorStandName = "";
    string actorIconName = "";

    static Dictionary<int, EnemyActorBaseData> enemyActorData = new Dictionary<int, EnemyActorBaseData>();

    static public void SetData(TextAsset tableData) {
        enemyActorData.Clear();

        string[] dataList = tableData.text.Split("\r\n");

        foreach (string data in dataList) {
            if (data.Substring(0, 2) == "//") continue;

            EnemyActorBaseData newData = new EnemyActorBaseData();
            string[] values = data.Split("\t");

            newData.index = int.Parse(values[0]);
            newData.coments = values[1];
            newData.actorNameIndex = int.Parse(values[2]);
            newData.attack = int.Parse(values[3]);
            newData.hp = int.Parse(values[4]);
            newData.mp = int.Parse(values[5]);
            newData.sp = int.Parse(values[6]);
            newData.strength = int.Parse(values[7]);
            newData.intelligence = int.Parse(values[8]);
            newData.dexterity = int.Parse(values[9]);
            newData.rank = int.Parse(values[10]);
            newData.actorStandName = values[11];
            newData.actorIconName = values[12];
            string deckSrt = values[13];
            if (deckSrt != "none") {
                deckSrt = deckSrt.Replace(" ", "");
                string[] deckList = deckSrt.Split(",");
                foreach (string str in deckList) {
                    newData.deckData.Add(int.Parse(str));
                }
            }

            enemyActorData.Add(newData.index, newData);
        }
    }

    public static EnemyActorBaseData GetEnemyData(int index) {
        return enemyActorData[index];
    }

    public int GetActorNameIndex() {
        return actorNameIndex;
    }

    public string GetCardName() {
        return coments;
    }

    public int GetAttack() {
        return attack;
    }

    public int GetHp() {
        return hp;
    }

    public int GetMp() {
        return mp;
    }

    public int GetSp() {
        return sp;
    }

    public int GetStrength() {
        return strength;
    }

    public int GetIntelligence() {
        return intelligence;
    }

    public int GetDexterity() {
        return dexterity;
    }

    public int GetRank() {
        return rank;
    }

    public string GetActorStandName() {
        return actorStandName;
    }

    public string GetIconName() {
        return actorIconName;
    }

    public List<int> GetDeckData() {
        return deckData;
    }
}
