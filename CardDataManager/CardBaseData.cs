/**
 * CardBaseData 는 게임에 사용될 카드의 베이스가 되는 데이터를 관리하는 클래스 이다.
 */
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum _targetType { self, enemy, friendly, any, freeTarget, notSelf , notSelfFriendly };
public enum _cardType { characterCard, lockCard, publicCard, enemyCard };

public class CardBaseData {
    int index = 0;
    string coments = "";
    int cardNameScript = 0;
    int activeScript = 0;
    int passiveScript = 0;
    int strength = 0;
    int intelligence = 0;
    int dexterity = 0;
    int luck = 0;
    int memory = 0;
    int drawBonus = 0;
    _targetType targetType = 0;
    _cardType cardType = 0;
    List<string> tagListA = new List<string>();
    List<string> tagListP = new List<string>();

    string owner = "none";
    int rank = 1;

    static Dictionary<int, CardBaseData> cardData = new Dictionary<int, CardBaseData>();

    public static void SetData(TextAsset tableData) {
        cardData.Clear();

        string[] dataList = tableData.text.Split("\r\n");

        foreach (string data in dataList) {
            if (data.Substring(0, 2) == "//") continue;

            CardBaseData newData = new CardBaseData();
            string[] values = data.Split("\t");

            newData.index = int.Parse(values[0]);
            newData.coments = values[1];

            if (values[2] == "none") {
                newData.cardNameScript = 0;
            } else {
                newData.cardNameScript = int.Parse(values[2]);
            }

            if (values[3] == "none") {
                newData.activeScript = 0;
            } else {
                newData.activeScript = int.Parse(values[3]);
            }

            if (values[4] == "none") {
                newData.passiveScript = 0;
            } else {
                newData.passiveScript = int.Parse(values[4]);
            }

            newData.strength = int.Parse(values[5]);
            newData.intelligence = int.Parse(values[6]);
            newData.dexterity = int.Parse(values[7]);
            newData.luck = int.Parse(values[8]);
            newData.memory = int.Parse(values[9]);
            newData.drawBonus = int.Parse(values[10]);

            switch (values[11]) {
                case "self":
                    newData.targetType = _targetType.self;
                    break;
                case "enemy":
                    newData.targetType = _targetType.enemy;
                    break;
                case "friendly":
                    newData.targetType = _targetType.friendly;
                    break;
                case "any":
                    newData.targetType = _targetType.any;
                    break;
                case "freeTarget":
                    newData.targetType = _targetType.freeTarget;
                    break;
                case "notSelf":
                    newData.targetType = _targetType.notSelf;
                    break;
                case "notSelfFriendly":
                    newData.targetType = _targetType.notSelfFriendly;
                    break;
            }

            newData.owner = values[12];
            switch (newData.owner) {
                case "C_Rank":
                    newData.rank = 1;
                    break;
                case "B_Rank":
                    newData.rank = 2;
                    break;
                case "A_Rank":
                    newData.rank = 3;
                    break;
                case "S_Rank":
                    newData.rank = 4;
                    break;
            }

            switch (values[13]) {
                case "c":
                    newData.cardType = _cardType.characterCard;
                    break;
                case "l":
                    newData.cardType = _cardType.lockCard;
                    break;                    
                case "p":
                    newData.cardType = _cardType.publicCard;
                    break;
                case "e":
                    newData.cardType = _cardType.enemyCard;
                    break;
            }

            //key:value	active_tag	passive_tag
            string stData;
            string[] stListData;

            stData = values[15];
            if (stData != "none") {
                stData = stData.Replace(" ", "");
                stListData = stData.Split(",");
                foreach (string st in stListData) {
                    newData.tagListA.Add(st);
                }
            }

            stData = values[16];
            if (stData != "none") {
                stData = stData.Replace(" ", "");
                stListData = stData.Split(",");
                foreach (string st in stListData) {
                    newData.tagListP.Add(st);
                }
            }

            cardData.Add(newData.index, newData);
        }
    }

    public static CardBaseData GetCardData(int index) {
        return cardData[index];
    }

    public string GetCardName() {
        if (cardNameScript == 0) return "???";
        return ScriptData.GetScript(cardNameScript);
    }

    public string GetActiveText() {
        if (cardNameScript == 0) return "";
        return ScriptData.GetScript(activeScript);
    }

    public string GetActiveValueText() {
        if (cardNameScript == 0) return "";
        string text = ScriptData.GetScript(activeScript + 1000000);
        if (text == "") text = ScriptData.GetScript(activeScript);
        return text;
    }

    public string GetStateText() {
        if (strength == 0 && intelligence == 0 && dexterity == 0 && luck == 0 && memory == 0 && drawBonus == 0) return "";

        string text = "";
        if (strength != 0) text += ScriptData.GetScript(300027).Replace("{0}", "" + strength);
        if (intelligence != 0) text += ScriptData.GetScript(300028).Replace("{0}", "" + intelligence);
        if (dexterity != 0) text += ScriptData.GetScript(300029).Replace("{0}", "" + dexterity);
        if (luck != 0) text += ScriptData.GetScript(300030).Replace("{0}", "" + luck);
        if (memory != 0) text += ScriptData.GetScript(300031).Replace("{0}", "" + memory);
        if (drawBonus != 0) text += ScriptData.GetScript(300032).Replace("{0}", "" + drawBonus);
        text += "\n";

        return text;
    }

    public string GetPassiveText() {
        if (cardNameScript == 0) return "";
        return ScriptData.GetScript(passiveScript);
    }

    public string GetPassiveValueText() {
        if (cardNameScript == 0) return "";
        string text = ScriptData.GetScript(passiveScript + 1000000);
        if (text == "") text = ScriptData.GetScript(passiveScript);
        return text;
    }

    public int GetStrength() {
        return strength;
    }

    public int GetDexterity() {
        return dexterity;
    }

    public int GetIntelligence() {
        return intelligence;
    }

    public int GetLuck() {
        return luck;
    }

    public int GetMemory() {
        return memory;
    }

    public int GetDrawBonus() {
        return drawBonus;
    }

    public _targetType GetTargetType() {
        return targetType;
    }

    public int GetIndex() {
        return index;
    }

    public string GetCardOwner() {
        return owner;
    }

    public int GetRank() {
        return rank;
    }

    public List<string> GetTagA() {
        return tagListA;
    }

    public List<string> GetTagP() {
        return tagListP;
    }

    public _cardType GetCardType() {
        return cardType;
    }
}
