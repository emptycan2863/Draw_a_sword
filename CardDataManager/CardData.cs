using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

enum _cardLocation {
    none,
    deckBattle,
    tomb,
    hand,
    action,
    draw,
}

public class CardData
{
    protected ActorData owner = null;
    protected CardBaseData baseData = null;
    _cardLocation location = _cardLocation.none;
    public bool drawEffect = false;
    int cardID = 0;
    int tableIndex = 0;

    public static CardData CreateCardData(int index, int id) {
        CardData cardData = CardDataInitManager.CreateCardData(index);
        cardData.tableIndex = index;
        cardData.baseData = CardBaseData.GetCardData(index);
        cardData.cardID = id;
        return cardData;
    }

    public static CardData CreateInstantCardData(int index, ActorData setOwner) {
        CardData cardData = CardDataInitManager.CreateCardData(index);
        cardData.tableIndex = index;
        cardData.baseData = CardBaseData.GetCardData(index);
        cardData.owner = setOwner;
        return cardData;
    }

    public string GetCardName() {
        return baseData.GetCardName();
    }

    virtual public string GetCardNameValue() {
        return baseData.GetCardName();
    }

    public string GetActiveText() {
        return baseData.GetActiveText();
    }

    virtual public string GetActiveValueText() {
        return baseData.GetActiveValueText();
    }

    public string GetStateText() {
        return baseData.GetStateText();
    }

    public string GetPassiveText() {
        return baseData.GetPassiveText();
    }

    virtual public string GetPassiveValueText() {
        return baseData.GetPassiveValueText();
    }

    public int GetID() {
        return cardID;
    }

    public int GetTableIndex() {
        return tableIndex;
    }

    public void SetOwner(ActorData _owner = null) {
        owner = _owner;
    }

    public ActorData GetOwner() {
        return owner;
    }

    public string GetOwnerCodeName() {
        return baseData.GetCardOwner();
    }

    public int GetStrength() {
        return baseData.GetStrength();
    }

    public int GetDexterity() {
        return baseData.GetDexterity();
    }

    public int GetIntelligence() {
        return baseData.GetIntelligence();
    }

    public int GetLuck() {
        return baseData.GetLuck();
    }

    public int GetMemory() {
        return baseData.GetMemory();
    }

    public int GetDrawBonus() {
        return baseData.GetDrawBonus();
    }

    public int GetRank() {
        return baseData.GetRank();
    }

    public _targetType GetTargetType() {
        return baseData.GetTargetType();
    }

    public List<string> GetTagA() {
        return baseData.GetTagA();
    }

    public List<string> GetTagP() {
        return baseData.GetTagP();
    }

    public void Reset() {
        VirtualReset();
    }

    virtual protected void VirtualReset() {
    }

    public void InitCardGraphic(Transform cardPrefab) {
        Sprite icon = SpriteData.GetActorIconSprite(baseData.GetCardOwner());
        Transform iconTf = cardPrefab.Find("Icon");
        if (icon != null) {
            iconTf.gameObject.SetActive(true);
            iconTf.GetComponent<Image>().sprite = icon;
        } else {
            iconTf.gameObject.SetActive(false);
        }

        Sprite cardImage = SpriteData.GetCardSprite(baseData.GetIndex());

        if (cardImage != null) {
            cardPrefab.Find("Illustration").GetComponent<Image>().sprite = cardImage;
        } else {
            if (icon != null) {
                cardPrefab.Find("Illustration").GetComponent<Image>().sprite = icon;
            } else {
                cardPrefab.Find("Illustration").GetComponent<Image>().sprite = SpriteData.GetCardSprite(0);
            }
        }

        InitCardName(cardPrefab);
        InitCardInfoText(cardPrefab);
    }

    public void InitCardName(Transform cardPrefab) {
        Transform textObject = cardPrefab.Find("TextObject");
        textObject.Find("CardName").gameObject.GetComponent<Text>().text = GetCardName();
    }

    public void InitCardNameValue(Transform cardPrefab) {
        Transform textObject = cardPrefab.Find("TextObject");
        textObject.Find("CardName").gameObject.GetComponent<Text>().text = GetCardNameValue();
    }

    public void InitCardInfoText(Transform cardPrefab) {
        Transform textObject = cardPrefab.Find("TextObject");
        textObject.Find("Active").gameObject.GetComponent<Text>().text = GetActiveText();
        string passiveText = GetStateText() + GetPassiveText();
        textObject.Find("Passive").gameObject.GetComponent<Text>().text = passiveText;
    }

    public void InitCardInfoValueText(Transform cardPrefab) {
        Transform textObject = cardPrefab.Find("TextObject");
        textObject.Find("Active").gameObject.GetComponent<Text>().text = GetActiveValueText();
        string passiveText = GetStateText() + GetPassiveValueText();
        textObject.Find("Passive").gameObject.GetComponent<Text>().text = passiveText;
    }

    public GameObject CardSelectInstant(Transform tr) {
        GameObject pf = GameObject.Instantiate(DataManager.card_CardSelectPf, tr);
        InitCardGraphic(pf.transform.Find("UICardGraphic"));        
        pf.GetComponent<Card_CardSelect>().linkCardData = this;
        return pf;
    }

    public GameObject HandCardInstant(Transform tr) {
        GameObject pf = GameObject.Instantiate(DataManager.card_HandCardtPf, tr);
        InitCardGraphic(pf.transform.Find("UICardGraphic"));
        pf.GetComponent<Card_HandCard>().linkCardData = this;
        return pf;
    }

    public void UseCard(ActorData targetActor) {
        if (GetTargetType() == _targetType.self) targetActor = owner;
        if (!IsTarget(targetActor)) return;
        ActionManager.UseCard(this, targetActor);
        SendToAction();
        UIHandCard.ChangedActorHand(owner);
    }

    virtual public bool IsTarget(ActorData targetActor) {
        if (targetActor.isDead) return false; 
        TeamData ownerTeam = owner.GetTeam();
        TeamData targetTeam = targetActor.GetTeam();

        switch (GetTargetType()) {
            case _targetType.self:
                return true;
            case _targetType.enemy:
                return ownerTeam != targetTeam;
            case _targetType.friendly:
                return ownerTeam == targetTeam;
            case _targetType.freeTarget:
                return true;
            case _targetType.notSelf:
                return owner != targetActor;
            case _targetType.notSelfFriendly:
                return owner != targetActor && ownerTeam == targetTeam;
        }
        return false;
    }

    void LocationEscape() {
        if (owner == null) return;
        List<CardData> cardList = null;
        switch (location) {
            case _cardLocation.deckBattle:
                cardList = owner.GetDeckBattle();
                break;
            case _cardLocation.tomb:
                cardList = owner.GetCardTomb();
                break;
            case _cardLocation.hand:
                cardList = owner.GetHandCard();
                break;
        }
        if (cardList != null) {
            int index = cardList.IndexOf(this);
            if (index != -1) {
                cardList.RemoveAt(index);
            }
        }

        if (location == _cardLocation.hand) {
            UIGauge gauge = owner.GetUIGauge();
            if (gauge != null) gauge.SetCardCount(owner.GetHandCard().Count);
        }
    }

    public void SendToDeckBattle() {
        LocationEscape();
        location = _cardLocation.deckBattle;
        owner.GetDeckBattle().Add(this);
    }

    public void SendToHand(bool useEffect = true) {
        LocationEscape();
        location = _cardLocation.hand;
        owner.GetHandCard().Add(this);
        UIGauge gauge = owner.GetUIGauge();
        if (gauge != null) gauge.SetCardCount(owner.GetHandCard().Count);

        if (owner.GetActorType() == _actorType.hero && useEffect) {
            owner.DrawEffect(this);
        }
    }

    //다른 카드의 효과로 버려지는 경우. 이 경우 유발효과가 발동할 수 있다.
    public void SendToTombCost(bool effect = true) {
        if (effect) CostEffect();
        SendToTomb();
        OnSendTombCost();
        owner.OnSendTombCost(this);
    }

    public void SendToTomb() {
        LocationEscape();
        location = _cardLocation.tomb;
        owner.GetCardTomb().Add(this);
    }
    public void SendToAction() {
        LocationEscape();
        location = _cardLocation.action;
    }

    public void SendToDraw() {
        LocationEscape();
        location = _cardLocation.draw;
    }

    public void SendToVoid() {
        LocationEscape();
        location = _cardLocation.none;
    }

    public _cardType GetCardType() {
        return baseData.GetCardType();
    }

    protected bool goTomb = false;
    protected List<UseCardEffectScript> useCardEffectList = new List<UseCardEffectScript>();
    public void UseCardAction(ActorData target) {
        if (useCardEffectList.Count != 0) {
            foreach (UseCardEffectScript obj in useCardEffectList) if (!obj.IsDestroyed()) GameObject.Destroy(obj.gameObject);
            useCardEffectList.Clear();
        }
        AddCardEffect();
        goTomb = true;
        VirtualUseCardAction(target);
        if (goTomb) SendToTomb();
    }

    public UseCardEffectScript AddCardEffect(bool isAction = true, bool isExtra = false) {
        Transform tr = UIManager.GetUIObject().transform.Find("BattleUI").Find("EffectUI");
        GameObject effect = GameObject.Instantiate(DataManager.useCardEffect, tr);
        effect.transform.position = GameManager.mainCamera.WorldToScreenPoint(owner.GetUseCardPos());
        if(!isAction) effect.transform.SetAsFirstSibling();
        UseCardEffectScript effectSC = effect.GetComponent<UseCardEffectScript>();
        effectSC.SetEffect(this, isAction, isExtra);
        useCardEffectList.Add(effectSC);
        return effectSC;
    }

    static ActorData cardCostLastOwner = null;
    static float cardCostLastTime = 0;
    static float cardCostLastPos = 0;
    public void CostEffect(UseCardEffectScript effect = null) {
        if (effect == null) {
            Transform tr = UIManager.GetUIObject().transform.Find("BattleUI").Find("EffectUI");
            GameObject newEffect = GameObject.Instantiate(DataManager.useCardEffect, tr);
            Vector3 v3 = GameManager.mainCamera.WorldToScreenPoint(owner.GetUseCardPos());
            float offsetX = -40;

            if (cardCostLastOwner == owner && cardCostLastTime > GameManager.GetTime() - 0.25f) offsetX += cardCostLastPos;
            cardCostLastOwner = owner;
            cardCostLastTime = GameManager.GetTime();
            cardCostLastPos = offsetX;

            if (owner.GetIsFlip()) {
                offsetX = -offsetX;
            }
            v3.x += offsetX;

            newEffect.transform.position = v3;
            newEffect.GetComponent<UseCardEffectScript>().SetCost(this);
        } else {
            effect.SetCost();
            useCardEffectList.Remove(effect);
        }
    }

    virtual public void VirtualUseCardAction(ActorData target) {
    }

    virtual public void VirtualOnHitEvent(ActorData target, int code) {
    }

    public void ExtraCardAction(ActorData target) {
        if (useCardEffectList.Count != 0) {
            foreach (UseCardEffectScript obj in useCardEffectList) if (!obj.IsDestroyed()) GameObject.Destroy(obj.gameObject);
            useCardEffectList.Clear();
        }

        AddCardEffect(true, true);

        VirtualExtraCardAction(target);
    }

    virtual public void VirtualExtraCardAction(ActorData target) {
    }

    //스텟 변화에 따른 체크
    virtual public bool IsConditionCheck() {
        return false;
    }

    virtual public void ConditionCheck() { 
    }

    //공격에 대한 체크
    virtual public bool IsOnAttack() {
        return false;
    }

    virtual public void OnAttack(ActorData target, int atk, int damage, int hpDamage, bool isKill) {
    }

    //이 카드가 버려졌을 때 발동하는 효과
    virtual protected void OnSendTombCost() {
    }

    //카드가 버려졌을 때에 따른 체크(어떤 카드가 버려지든 발동하는 패시브)
    virtual public bool IsOnOwnerSendTombCost() {
        return false;
    }

    virtual public void OnOwnerSendTombCost(CardData card) {
    }

    virtual public bool DeckLimitCheck(HeroActorData hero) {
        return true;
    }

    virtual public void StartEffect() { 
    }
}
