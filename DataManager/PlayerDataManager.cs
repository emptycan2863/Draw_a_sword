using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

static public class PlayerDataManager {
    static float playTime = 0;
    static float gameStartTime = 0;

    static int masteryLv = 0;
    static Dictionary<int, HeroActorData> heroDic = new Dictionary<int, HeroActorData>();
    static List<HeroActorData> partyList = new List<HeroActorData>();

    static int lastCardID = 0;
    static Dictionary<int, CardData> cardInventory = new Dictionary<int, CardData>();

    static public List<int> GetHeroKeys() {
        List<int> newList = new List<int>();
        foreach (int key in heroDic.Keys) {
            newList.Add(key);
        }
        return newList;
    }

    static public HeroActorData GetHero(int key) {
        return heroDic[key];
    }

    static public void LoadData(int index) {
        JsObject loadData = SaveDataManager.GetSaveData(index);
        SaveDataManager.SetSaveIndex(index);

        heroDic.Clear();
        partyList.Clear();
        cardInventory.Clear();

        if (loadData == null) {
            playTime = 0;
            masteryLv = 1;
            lastCardID = 0;

            heroDic.Add(1, HeroActorData.CreateHeroData(1));
            heroDic.Add(2, HeroActorData.CreateHeroData(2));
            heroDic.Add(3, HeroActorData.CreateHeroData(3));

            partyList.Add(GetHero(1));
            partyList.Add(GetHero(2));
        } else {
            playTime = loadData.GetDictionary()["playTime"].GetFloat();
            masteryLv = loadData.GetDictionary()["masteryLv"].GetInt();

            JsObject cardInvenData = loadData.GetDictionary()["cardInvenData"];
            foreach (string ids in cardInvenData.GetDictionary().Keys) {
                int id = int.Parse(ids);
                CreateCardData(cardInvenData.GetDictionary()[ids].GetInt(), id);
            }
            lastCardID = loadData.GetDictionary()["lastCardID"].GetInt();

            JsObject heroDataDic = loadData.GetDictionary()["heroData"];
            foreach (string heroIndex in heroDataDic.GetDictionary().Keys) {
                int heroId = int.Parse(heroIndex);
                JsObject heroData = heroDataDic.GetDictionary()[heroIndex];
                int lv = heroData.GetDictionary()["LV"].GetInt();
                int exp = heroData.GetDictionary()["EXP"].GetInt();
                List<int> deck = new List<int>();
                JsObject deckData = heroData.GetDictionary()["heroDeck"];
                foreach (JsObject jsData in deckData.GetList()) {
                    deck.Add(jsData.GetInt());
                }

                heroDic.Add(heroId, HeroActorData.LoadHeroData(heroId, lv, exp, deck));
            }

            JsObject partyData = loadData.GetDictionary()["partyData"];

            foreach (JsObject data in partyData.GetList()) {
                partyList.Add(GetHero(data.GetInt()));
            }
        }
        gameStartTime = GameManager.GetTime();
    }

    //세이브 데이터의 형식은 여기서 만든다.
    static public void SaveData() {
        JsObject saveData = new JsObject(_jsDataType._dictionary);
        saveData.GetDictionary().Add("saveVersion", new JsObject(SaveDataManager.GetSaveVersion()));        
        float newPlayTime = playTime + GameManager.GetTime() - gameStartTime;
        saveData.GetDictionary().Add("playTime", new JsObject(newPlayTime));
        saveData.GetDictionary().Add("masteryLv", new JsObject(masteryLv));

        JsObject cardInvenData = new JsObject(_jsDataType._dictionary);
        saveData.GetDictionary().Add("cardInvenData", cardInvenData);
        foreach (int code in cardInventory.Keys) {
            CardData card = cardInventory[code];
            cardInvenData.GetDictionary().Add("" + code, new JsObject(card.GetTableIndex()));
        }
        saveData.GetDictionary().Add("lastCardID", new JsObject(lastCardID));

        JsObject heroData = new JsObject(_jsDataType._dictionary);
        saveData.GetDictionary().Add("heroData", heroData);

        foreach (int heroIndex in heroDic.Keys) {
            JsObject newHeroData = new JsObject(_jsDataType._dictionary);
            HeroActorData had = heroDic[heroIndex];

            newHeroData.GetDictionary().Add("LV", new JsObject(had.GetLevel()));
            newHeroData.GetDictionary().Add("EXP", new JsObject(had.GetExp()));
            JsObject deck = new JsObject(_jsDataType._list);
            foreach (int id in had.GetDeckData()) {
                deck.GetList().Add(new JsObject(id));
            }
            newHeroData.GetDictionary().Add("heroDeck", deck);

            heroData.GetDictionary().Add("" + heroIndex, newHeroData);
        }

        JsObject partyData = new JsObject(_jsDataType._list);
        saveData.GetDictionary().Add("partyData", partyData);

        foreach (HeroActorData hero in partyList) {
            partyData.GetList().Add(new JsObject(hero.GetIndex()));
        }

        SaveDataManager.Save(saveData);
    }

    static public HeroActorData GetHeroInParty(int index) {
        if (index >= partyList.Count) return null;
        return partyList[index];
    }

    static public void SetHeroInParty(int index, HeroActorData heroData = null) {
        if (heroData == null) {
            if (index >= partyList.Count) return;
            partyList.RemoveAt(index);
        } else {
            if (index >= partyList.Count) {
                partyList.Add(heroData);
                return;
            }
            partyList[index] = heroData;
        }
    }

    static public bool IsParty(HeroActorData hero) {
        if (partyList.Count == 0) return false;
        foreach (HeroActorData member in partyList) {
            if (member == hero) return true;
        }
        return false;
    }

    static public int GetPartyMemberCount() {
        return partyList.Count;
    }

    static public void StageReward(int stageIndex) {
        int exp = StageData.GetExpReward(stageIndex);
        int partyCount = partyList.Count;
        int remain = exp % partyCount;
        int divExp = exp / partyCount;
        List<int> lvUpcheck = new List<int>();

        foreach (HeroActorData hero in partyList) {
            lvUpcheck.Add(hero.GetLevel());
            hero.AddExp(divExp);
        }

        if (remain > 0) {
            List<int> randomList = new List<int>();
            for (int i = 0, len = partyCount; i < len; ++i) randomList.Add(i);
            while (remain > 0) {
                int v = Random.Range(0, randomList.Count);
                randomList.Remove(v);
                partyList[v].AddExp(1);
                --remain;
            }
        }

        List<HeroActorData> lvUpHero = new List<HeroActorData>();
        for (int i = 0; i < partyCount; ++i) {
            HeroActorData hero = partyList[i];
            hero.LVUPCheck();
            if (hero.GetLevel() - lvUpcheck[i] > 0) lvUpHero.Add(hero);
        }

        List<int> cardReward = StageData.GetCardReward(stageIndex);
        List<CardData> getCard = new List<CardData>();
        foreach (int cardIndex in cardReward) {
            getCard.Add(CreateCardData(cardIndex));
        }
        UIReward.SetRewardUI(exp, lvUpHero, getCard);
    }

    static public CardData CreateCardData(int index, int id = -1) {
        if (index == -1) return null;
        if (id == -1) {
            id = lastCardID;
            ++lastCardID;
        }
        CardData cardData = CardData.CreateCardData(index, id);
        cardInventory.Add(id, cardData);
        return cardData;
    }

    static public CardData GetCardById(int id) {
        if (!cardInventory.ContainsKey(id)) return null;
        return cardInventory[id];
    }

    static public List<int> GetCardIdList() {
        List<int> idList = new List<int>();
        foreach (int id in cardInventory.Keys) {
            idList.Add(id);
        }
        return idList;
    }
}
