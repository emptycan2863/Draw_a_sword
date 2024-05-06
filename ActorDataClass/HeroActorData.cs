using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class HeroActorData : ActorData {
    int heroIndex = 0;
    HeroActorBaseData heroBaseData = null;
    int exp = 0;

    protected List<int> deckHero = new List<int>();

    public HeroActorData(int index) {
        actorType = _actorType.hero;
        heroIndex = index;

        SetLevel(1);
        heroBaseData = HeroActorBaseData.GetHeroData(index);

        prefabActorStand = PrefabActorStandManager.GatActorStand(heroBaseData.GetActorStandName());
        iconName = heroBaseData.GetIconName();

        deckHero.Clear();
        for (int i = 0, len = 40; i < len; ++i) {
            deckHero.Add(-1);
        }
    }

    public List<int> GetDeckData() {
        return deckHero;
    }

    public int GetIndex() {
        return heroIndex;
    }

    static public HeroActorData CreateHeroData(int index) {
        HeroActorData heroActor = new HeroActorData(index);
        heroActor.SetStartDeck();
        return heroActor;
    }

    static public HeroActorData LoadHeroData(int index, int lv, int exp, List<int> deckData) {
        HeroActorData heroActor = new HeroActorData(index);
        heroActor.SetLevel(lv);
        heroActor.AddExp(exp);
        heroActor.LoadDeck(deckData);
        return heroActor;
    }

    void SetStartDeck() {
        List<int> deckList = heroBaseData.GetStartDeck();
        for (int i = 0, len = deckList.Count; i < len; ++i) {
            CardData card = PlayerDataManager.CreateCardData(deckList[i]);
            if (card != null) {
                card.SetOwner(this);
                deckHero[i] = card.GetID();
            }
        }
    }

    void LoadDeck(List<int> deckData) {
        for (int i = 0, len = deckData.Count; i < len; ++i) {
            int id = deckData[i];
            CardData card = PlayerDataManager.GetCardById(id);
            if (card == null) continue;
            card.SetOwner(this);
            deckHero[i] = card.GetID();
        }
    }

    public void RemoveDeckCard(int slot) {
        int id = deckHero[slot];
        CardData card = PlayerDataManager.GetCardById(id);
        card.SetOwner();
        deckHero[slot] = -1;
    }

    public void SetDeckCard(int slot, CardData card) {
        int id = card.GetID();
        card.SetOwner(this);
        if (deckHero[slot] != -1) RemoveDeckCard(slot);
        deckHero[slot] = id;
    }

    override protected void VirtualSetBattleState() {
        maxHp = GetStrength() * 20;
        maxMp = GetIntelligence() * 20;
        if (maxHp <= 0) maxHp = 1;
        if (maxMp <= 0) maxMp = 1;
    }

    override public string GetName() {
        return ScriptData.GetScript(heroBaseData.GetActorNameIndex());
    }

    override protected void SetSpeed() {
        speed = GetDexterity() * 20 + conditionalSpeed;
        UIGauge gauge = GetUIGauge();
        if (gauge != null) {
            gauge.SetSp(GetSpeed());
        }
    }

    class HeroOverCardThrowCbc : UICardSelect.SelectCardCallBackClass {
        public HeroActorData thisActor = null;
        public override void Func() {
            foreach (CardData card in selectCard) card.SendToTomb();
            thisActor.UpdateHandCard(true);
            TurnManager.SetPhaseHeroActive();
        }
    }

    class HeroCardSelectCbc : UICardSelect.SelectCardCallBackClass {
        public HeroActorData thisActor = null;
        public override void Func() {
            foreach (CardData card in selectCard) {
                card.SendToHand();
                card.drawEffect = true;
            }
            foreach (CardData card in notSelectedCard) card.SendToTomb();
            thisActor.UpdateHandCard(true);

            List<CardData> handCardList = thisActor.GetHandCard();
            int handCount = handCardList.Count;
            int memory = thisActor.GetMemory();

            if (handCount > memory) {
                HeroOverCardThrowCbc cbc = new HeroOverCardThrowCbc();
                cbc.thisActor = thisActor;
                int throwCount = handCount - memory;
                UICardSelect.SelectCard(handCardList, cbc, throwCount, throwCount, ScriptData.GetScript(300006));
            } else {
                TurnManager.SetPhaseHeroActive();
            }
        }
    }

    override public void VirtualTurnStart() {
        UpdateHandCard(true);
        TurnManager.SetPhaseDraw();

        List<CardData> cardList = new List<CardData>();
        for (int i = 0, len = GetDrawBonus() + GetLuck(); i < len; ++i) {
            CardData card = PullUpCard();
            if (card != null) cardList.Add(card);
        }

        HeroCardSelectCbc cbc = new HeroCardSelectCbc();
        cbc.thisActor = this;
        int turnDrawCount = GetDrawBonus();
        List<CardData> handCardList = GetHandCard();
        int handCount = handCardList.Count;
        int memory = this.GetMemory();
        int minCount = memory - handCount;
        if (minCount > turnDrawCount) minCount = turnDrawCount;
        UICardSelect.SelectCard(cardList, cbc, minCount, turnDrawCount, ScriptData.GetScript(300004));
        UINextArrow.SetUIActive(true);
    }

    override public void VirtualTurnEnd() {
        UIHandCard.CloseUI();
        UINextArrow.SetUIActive(false);
    }

    public void UpdateHandCard(bool popup = false) {
        if (popup) UIHandCard.SetActorHand(this);
        UIHandCard.HandCardUpdate();
    }

    static float lastDrawTime = 0;
    override public void DrawEffect(CardData card) {
        float thisTime = GameManager.GetTime();
        float delay = lastDrawTime + 0.15f - thisTime;
        if (delay < 0) delay = 0;

        Transform tr = UIManager.GetUIObject().transform.Find("BattleUI").Find("EffectUI");
        GameObject newEffect = GameObject.Instantiate(DataManager.useCardEffect, tr);
        newEffect.transform.position = GetDrawPos();
        newEffect.GetComponent<UseCardEffectScript>().SetDraw(card, delay);

        lastDrawTime = thisTime + delay;
    }

    public void AddExp(int expReward) {
        exp += expReward;
    }

    public int GetExp() {
        return exp;
    }

    public int GetNeedExp() {
        return DataManager.GetLvUpExp(GetLevel(), heroBaseData.GetExpType());
    }

    public void LVUPCheck() {
        bool up = true;
        while (up) {
            int needExp = DataManager.GetLvUpExp(GetLevel(), heroBaseData.GetExpType());
            if (needExp <= exp && needExp != -1) {
                exp -= needExp;
                LVUP();
            } else {
                up = false;
            }
        }
    }

    protected override List<CardData> GetDeck() {
        List<CardData> deck = new List<CardData>();
        int len = GetLevel();
        if (len > deckHero.Count) len = deckHero.Count;
        for (int i = 0; i < len; ++i) {
            if (deckHero[i] == -1) continue;
            CardData card = PlayerDataManager.GetCardById(deckHero[i]);
            if (card == null) continue;
            deck.Add(card);
        }

        return deck;
    }

    public string GetCodeName() {
        return heroBaseData.GetCodeName();
    }
}
