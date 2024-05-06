using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class LobbySceneObject : SceneObject {
    static LobbySceneObject _this = null;
    public GameObject deckOrDungeon = null;
    public GameObject partySetting = null;
    public GameObject deckSetting = null;
    public GameObject stageSelect = null;

    public GameObject party1 = null;
    public GameObject party2 = null;
    public GameObject party3 = null;
    public GameObject party4 = null;
    List<GameObject> PartyList = new List<GameObject>();
    public GameObject HeroSelect = null;
    public GameObject selectHeroButtonFrepab = null;
    public GameObject stageSelectButtonFrepab = null;

    public GameObject myDeckCardFrameFrepab = null;
    List<GameObject> deckCardSlotList = new List<GameObject>();
    public GameObject cardBoxCardFrameFrepab = null;
    Dictionary<int, GameObject> cardBoxDic = new Dictionary<int, GameObject>();
    List<int> cardBoxOrder = new List<int>();


    GameObject cardDragObject = null;
    GameObject cardBoxDragCheck = null;
    GameObject cardBoxScroll = null;
    GameObject cardBoxDragCursor = null;

    private void Start() {
        SceneObjectStart();
        _this = this;

        PartyList.Clear();
        PartyList.Add(party1);
        PartyList.Add(party2);
        PartyList.Add(party3);
        PartyList.Add(party4);

        Transform cardSlotObject = _this.deckSetting.transform.Find("MyDeckObject").Find("CardSlot");
        deckCardSlotList.Clear();
        for (int x = 0, y = 0, i = 0, len = 40; i < len; ++x, ++i) {
            if (x == 10) {
                x = 0;
                ++y;
            }

            GameObject slot = GameObject.Instantiate(myDeckCardFrameFrepab, cardSlotObject);
            slot.GetComponent<RectTransform>().localPosition = new Vector2(42 + (x * 107), -60 + (-125 * y));
            slot.transform.Find("LvText").gameObject.GetComponent<Text>().text = "LV" + (i < 9 ? "0" : "") + (i + 1);
            deckCardSlotList.Add(slot);
        }

        cardDragObject = _this.deckSetting.transform.Find("CardDrag").gameObject;
        _this.cardDragObject.SetActive(false);
        cardBoxDragCheck = _this.deckSetting.transform.Find("CardBox").Find("CardBoxDragCheck").gameObject;
        cardBoxDragCheck.SetActive(false);
        cardBoxScroll = _this.deckSetting.transform.Find("CardBox").Find("UIScroll").gameObject;

        cardBoxDragCursor = _this.cardBoxScroll.transform.Find("Mask").Find("Contents").Find("ContentsParents").Find("DragCursor").gameObject;
        cardBoxDragCursor.SetActive(false);

        GameManager.gameMode = _gamemode.lobby;
        GoDeckOrDungeon();
    }

    protected override void VirtualSceneUpdate() {
        CardDragUpdate();
    }

    static public void AllClose() {
        if (_this.deckOrDungeon != null) _this.deckOrDungeon.SetActive(false);
        if (_this.partySetting != null) _this.partySetting.SetActive(false);
        if (_this.deckSetting != null) _this.deckSetting.SetActive(false);
        if (_this.stageSelect != null) _this.stageSelect.SetActive(false);
        UITag.SetTag();
    }

    static public void GoDeckOrDungeon() {
        AllClose();

        if (_this.deckOrDungeon != null) _this.deckOrDungeon.SetActive(true);
    }

    static public void GoPartySetting() {
        AllClose();

        if (_this.partySetting != null) _this.partySetting.SetActive(true);
        InitPartyButton();
        _this.HeroSelect.SetActive(false);
    }

    static public void InitPartyButton() {
        bool block = false;
        for (int i = 0, len = 4; i < len; ++i) {
            GameObject partyButton =_this.PartyList[i];
            GameObject blockObj = partyButton.transform.Find("PartyBlock").gameObject;
            GameObject heroIcon = partyButton.transform.Find("PartyIcon").gameObject;
            GameObject deckButton = partyButton.transform.Find("PartyDeck").gameObject;
            GameObject xButton = partyButton.transform.Find("PartyX").gameObject;
            if (block) {
                blockObj.SetActive(true);
                heroIcon.SetActive(false);
                deckButton.SetActive(false);
                xButton.SetActive(false);
            } else {
                blockObj.SetActive(false);
                HeroActorData hero = PlayerDataManager.GetHeroInParty(i);
                if (hero == null) {
                    heroIcon.SetActive(false);
                    deckButton.SetActive(false);
                    xButton.SetActive(false);
                    block = true;
                } else {
                    int lv = hero.GetLevel();
                    float expRad = (float)hero.GetExp() / (float)hero.GetNeedExp();

                    heroIcon.SetActive(true);
                    deckButton.SetActive(true);
                    xButton.SetActive(PlayerDataManager.GetPartyMemberCount() > 1);
                    heroIcon.GetComponent<Image>().sprite = hero.GetIconSprite();

                    GameObject lvObj = heroIcon.transform.Find("LV").gameObject;
                    lvObj.GetComponent<Text>().text = "" + lv;

                    GameObject expBarObj = heroIcon.transform.Find("EXPBarGuage").gameObject;
                    expBarObj.transform.localScale = new Vector3(expRad, 1, 1);
                }
            }
        }
    }

    static public void OpenHeroSelectUI(int index) {
        _this.HeroSelect.SetActive(true);
        Transform scrollCon = _this.HeroSelect.transform.Find("UIScroll").Find("Mask").Find("Contents").Find("ContentsParents");

        foreach (Transform trash in scrollCon) {
            GameObject.Destroy(trash.gameObject);
        }

        UIScroll uiScroll = _this.HeroSelect.transform.Find("UIScroll").gameObject.GetComponent<UIScroll>();
        GameObject mask = _this.HeroSelect.transform.Find("UIScroll").Find("Mask").gameObject;

        List<HeroActorData> list = new List<HeroActorData>();
        if (PlayerDataManager.GetPartyMemberCount() > 1 || index > 0) list.Add(null);
        HeroActorData thisHero = PlayerDataManager.GetHeroInParty(index);        
        if (thisHero != null) list.Add(thisHero);

        foreach (int heroCode in PlayerDataManager.GetHeroKeys()) {
            HeroActorData hero = PlayerDataManager.GetHero(heroCode);
            if (!PlayerDataManager.IsParty(hero)) list.Add(hero);
        }

        int xCount = -1;
        int yCount = 1;

        for (int i = 0, len = list.Count; i < len; ++i) {
            ++xCount;
            if (xCount == 5) {
                xCount = 0;
                ++yCount;
            }

            GameObject button = Instantiate(_this.selectHeroButtonFrepab, scrollCon);
            PartySettingButton_SelectHero buttonSc = button.GetComponent<PartySettingButton_SelectHero>();
            buttonSc.SetMaskingObject(mask);
            buttonSc.SetNumAndHeroData(index, list[i]);

            float x = (xCount * 280) - 560;
            float y = -((yCount-1) * 280) - 155;

            button.transform.localPosition = new Vector3(x, y);
        }

        float height = (yCount * 280) + 30;
        if (height < 650) height = 650;
        uiScroll.SetContentsSize(height);
        uiScroll.SetScrollPos(0);
    }

    static public void CloseHeroSelectUI() {
        _this.HeroSelect.SetActive(false);
        InitPartyButton();
        PlayerDataManager.SaveData();
    }

    HeroActorData deckSettingTarget = null;
    static public void GoDeckSetting(HeroActorData hero) {
        _this.deckSettingTarget = hero;
        AllClose();

        if (_this.deckSetting != null) _this.deckSetting.SetActive(true);

        int lv = hero.GetLevel();

        _this.deckSetting.transform.Find("HeroIcon").gameObject.GetComponent<Image>().sprite = hero.GetIconSprite();
        _this.deckSetting.transform.Find("HeroLv").gameObject.GetComponent<Text>().text = "LEVEL " + lv;
        _this.deckSetting.transform.Find("ExpText").gameObject.GetComponent<Text>().text = ScriptData.GetScript(300041).Replace("{0}", "" + hero.GetExp()).Replace("{1}", "" + hero.GetNeedExp());

        List<int> deckData = hero.GetDeckData();
        for (int i = 0, len = 40; i < len; ++i) {
            CardData card = PlayerDataManager.GetCardById(deckData[i]);
            GameObject slot = _this.deckCardSlotList[i];
            LobbyCard_MyDeck md = slot.GetComponent<LobbyCard_MyDeck>();

            md.linkCardData = card;
            md.slotNum = i;

            GameObject cardGraphic = slot.transform.Find("UICardGraphic").gameObject;
            if (card == null) {
                cardGraphic.SetActive(false);
            } else {
                if (card.GetCardType() == _cardType.lockCard) md.IsLockCard();
                cardGraphic.SetActive(true);
                card.InitCardGraphic(cardGraphic.transform);
            }

            GameObject cardLock = slot.transform.Find("Lock").gameObject;
            GameObject cardLevel = slot.transform.Find("LvText").gameObject;
            md.IsLvLockCard(i >= lv);
        }

        InitPassiveText();
        InitCardBox();

        SetCardViewer(_this.deckCardSlotList[0].GetComponent<LobbyCard_MyDeck>().linkCardData);

        List<string> tagTest = new List<string>();
    }

    static void InitPassiveText() {
        if (_this.deckSettingTarget == null) return;

        List<int> deckData = _this.deckSettingTarget.GetDeckData();
        int lv = _this.deckSettingTarget.GetLevel();
        int strength = 0;
        int intelligence = 0;
        int dexterity = 0;
        int luck = 0;
        int memory = 0;
        int drawBonus = 0;

        string specialPassiveText = "";

        for (int i = 0; i < lv; ++i) {
            CardData card = PlayerDataManager.GetCardById(deckData[i]);
            if (card != null) {
                strength += card.GetStrength();
                intelligence += card.GetIntelligence();
                dexterity += card.GetDexterity();
                luck += card.GetLuck();
                memory += card.GetMemory();
                drawBonus += card.GetDrawBonus();

                string cardPassive = card.GetPassiveText();
                if (cardPassive != "") specialPassiveText += "\n" + cardPassive + "\n";
            }
        }

        string passiveText = "";
        passiveText += ScriptData.GetScript(300027).Replace("{0}", "" + strength);
        passiveText += "\n" + ScriptData.GetScript(300033).Replace("{0}", "" + (strength*20)) + "\n";

        passiveText += "\n" + ScriptData.GetScript(300028).Replace("{0}", "" + intelligence);
        passiveText += "\n" + ScriptData.GetScript(300034).Replace("{0}", "" + (intelligence * 20)) + "\n";

        passiveText += "\n" + ScriptData.GetScript(300029).Replace("{0}", "" + dexterity);
        passiveText += "\n" + ScriptData.GetScript(300035).Replace("{0}", "" + (dexterity * 20)) + "\n";

        if (drawBonus != 0) passiveText += "\n" + ScriptData.GetScript(300032).Replace("{0}", "" + drawBonus);
        if (luck != 0) passiveText += "\n" + ScriptData.GetScript(300030).Replace("{0}", "" + luck);
        if (luck == 0) passiveText += "\n" + ScriptData.GetScript(300036).Replace("{0}", "" + (drawBonus + 2));
        else passiveText += "\n" + ScriptData.GetScript(300037).Replace("{0}", "" + (luck + drawBonus + 2)).Replace("{1}", "" + (drawBonus + 2));
        passiveText += "\n";

        if (memory != 0) passiveText += "\n" + ScriptData.GetScript(300031).Replace("{0}", "" + memory);
        passiveText += "\n" + ScriptData.GetScript(300038).Replace("{0}", "" + (memory + 3));

        passiveText += "\n\n";

        passiveText += specialPassiveText;

        GameObject PassiveScroll = _this.deckSetting.transform.Find("PassiveScroll").gameObject;
        GameObject textObject = PassiveScroll.transform.Find("Mask").Find("Contents").Find("ContentsParents").Find("HeroPassiveText").gameObject;
        textObject.gameObject.GetComponent<Text>().text = passiveText;
        UIScroll scrollSc = PassiveScroll.GetComponent<UIScroll>();
        RectTransform textRt = textObject.GetComponent<RectTransform>();

        float y = textObject.gameObject.GetComponent<Text>().preferredHeight;
        if (y < 950) y = 950;
        textRt.sizeDelta = new Vector2(textRt.sizeDelta.x, y);
        scrollSc.SetContentsSize(y);
        scrollSc.SetScrollPos(0);
    }

    static void InitCardBox() {
        CardBoxDestroy();
        List<int> cardIdList = PlayerDataManager.GetCardIdList();

        string heroCodeName = _this.deckSettingTarget.GetCodeName();

        List<int> characterCardList = new List<int>();
        List<int> publicCardList = new List<int>();
        foreach (int id in cardIdList) {
            CardData card = PlayerDataManager.GetCardById(id);
            if (card.GetOwner() != null) continue;
            string cardCodeName = card.GetOwnerCodeName();
            if (cardCodeName != "none" && cardCodeName != heroCodeName) continue;

            AddCardInBox(id);

            if (cardCodeName == "none") {
                publicCardList.Add(id);
            } else if (cardCodeName == heroCodeName) {
                characterCardList.Add(id);
            }
        }

        _this.cardBoxOrder.AddRange(characterCardList);
        _this.cardBoxOrder.AddRange(publicCardList);

        CardBoxRepositioning();

        _this.cardBoxScroll.GetComponent<UIScroll>().SetScrollPos(0);
    }

    static void CardBoxDestroy() {
        if (_this.cardBoxDic.Count > 0) {
            foreach (int id in _this.cardBoxDic.Keys) {
                GameObject.Destroy(_this.cardBoxDic[id]);
            }
        }
        _this.cardBoxDic.Clear();
        _this.cardBoxOrder.Clear();
    }

    static void CardBoxRepositioning() {
        int y = 0;
        for (int i = 0, len = _this.cardBoxOrder.Count, x = 0; i < len; ++i, ++x) {
            if (x == 10) {
                x = 0;
                ++y;
            }
            int id = _this.cardBoxOrder[i];
            GameObject obj = _this.cardBoxDic[id];

            obj.GetComponent<RectTransform>().localPosition = new Vector2(72f + (x * 132), -104f + (-217 * y));
        }
        UIScroll scrollSc = _this.cardBoxScroll.GetComponent<UIScroll>();
        float size = (217 * (y + 1)) + 11;
        if (size < 445) size = 445;
        scrollSc.SetContentsSize(size);

        _this.deckSetting.transform.Find("CardBox").Find("CardBoxText").gameObject.GetComponent<Text>().text = "CARD BOX [" + _this.cardBoxOrder.Count + "]";
    }

    static public void ClickDeckSettingSaveButton() {
        CardBoxDestroy();
        PlayerDataManager.SaveData();
        GoPartySetting();
    }

    static public void SetCardViewer(CardData cardData) {
        if (cardData == null) return;
        cardData.InitCardGraphic(_this.deckSetting.transform.Find("CardViewer").Find("UICardGraphic"));
    }

    CardData dragCardTarget = null;
    bool dragInBox = false;

    static void CardDragUpdate() {
        float moustX = Input.mousePosition.x;
        float moustY = Input.mousePosition.y;

        if (_this.dragCardTarget != null) {
            _this.cardDragObject.GetComponent<RectTransform>().position = new Vector2(moustX, moustY);

            if (_this.dragInBox) {
                RectTransform rt = _this.cardBoxScroll.transform.Find("Mask").Find("Contents").gameObject.GetComponent<RectTransform>();
                float boxPosX = moustX - rt.position.x + 664;
                if (boxPosX < 0) boxPosX = 0;
                float boxPosY = moustY - rt.position.y;
                if (boxPosY > 0) boxPosY = 0;

                int slotX = (int)((boxPosX + 66) / 132);
                int slotY = (int)((-boxPosY) / 217);

                DragCursorMove(slotX, slotY);
            }
        }
    }

    static int dragCursorX = -1;
    static int dragCursorY = -1;

    static void DragCursorMove(int x, int y) {
        if (dragCursorX == x && dragCursorY == y) return;
        int cardCount = _this.cardBoxOrder.Count;

        int xTemp = cardCount % 10;
        int yTemp = cardCount / 10;

        if (y > yTemp) y = yTemp;
        if (y == yTemp && x > xTemp) x = xTemp;

        dragCursorX = x;
        dragCursorY = y;

        RectTransform rt = _this.cardBoxDragCursor.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(-664 + (x * 132), -112 + (y * -217));

        int cardOrder = _this.cardBoxOrder.IndexOf(_this.dragCardTarget.GetID());

        if (cardOrder != -1) {
            int orderTemp = y * 10 + x;
            if (cardOrder == orderTemp || cardOrder + 1 == orderTemp) {
                _this.cardBoxDragCursor.SetActive(false);
            } else {
                _this.cardBoxDragCursor.SetActive(true);
            }
        }
    }

    static public void SetCardDrag(CardData card) {
        if (card == null) return;

        _this.dragCardTarget = card;
        _this.cardDragObject.SetActive(true);

        card.InitCardGraphic(_this.cardDragObject.transform.Find("UICardGraphic"));
        _this.cardBoxDragCheck.SetActive(true);
        ClickObject.CheckOver();
    }

    static public void ClearCardDrag() {
        _this.dragCardTarget = null;
        _this.cardDragObject.SetActive(false);
        _this.cardBoxDragCheck.SetActive(false);
        ClickObject.CheckOver();
    }

    static public void OnDownBoxCard(LobbyCard_CardBox target) {
        LobbySceneObject.SetCardDrag(target.linkCardData);
        target.transform.Find("UICardGraphic").gameObject.GetComponent<CanvasGroup>().alpha = 0.5f;
    }

    static public void OnUpBoxCard(LobbyCard_CardBox target) {
        LobbySceneObject.ClearCardDrag();
        target.transform.Find("UICardGraphic").gameObject.GetComponent<CanvasGroup>().alpha = 1f;
    }

    static public void OnDownMyDeckCard(LobbyCard_MyDeck target) {
        LobbySceneObject.SetCardDrag(target.linkCardData);
        target.transform.Find("UICardGraphic").gameObject.GetComponent<CanvasGroup>().alpha = 0.5f;
    }

    static public void OnUpMyDeckCard(LobbyCard_MyDeck target) {
        LobbySceneObject.ClearCardDrag();
        target.transform.Find("UICardGraphic").gameObject.GetComponent<CanvasGroup>().alpha = 1f;
    }

    static public void DragInBoxCheck(bool check) {
        _this.cardBoxDragCursor.SetActive(check);
        _this.dragInBox = check;
        dragCursorX = -1;
        dragCursorY = -1;
    }

    static public void OnDragUpCardBox() {
        int cardId = _this.dragCardTarget.GetID();
        int cardOrder = _this.cardBoxOrder.IndexOf(cardId);
        int orderMove = dragCursorY * 10 + dragCursorX;
        HeroActorData targetHero = _this.deckSettingTarget;

        if (cardOrder != -1) {
            if (cardOrder == orderMove || cardOrder + 1 == orderMove) {
                return;
            } else if (cardOrder + 1 < orderMove) {
                --orderMove;
            }
            _this.cardBoxOrder.RemoveAt(cardOrder);
        } else {
            int deckSlotOrder = targetHero.GetDeckData().IndexOf(cardId);
            GameObject slot = _this.deckCardSlotList[deckSlotOrder];
            GameObject cardGraphic = slot.transform.Find("UICardGraphic").gameObject;
            cardGraphic.SetActive(false);

            targetHero.RemoveDeckCard(deckSlotOrder);
            slot.GetComponent<LobbyCard_MyDeck>().linkCardData = null;

            AddCardInBox(cardId);
            InitPassiveText();
        }

        _this.cardBoxOrder.Insert(orderMove, cardId);
        CardBoxRepositioning();
    }

    static void AddCardInBox(int id) {
        if (_this.cardBoxDic.ContainsKey(id)) {
            _this.cardBoxDic[id].SetActive(true);
            return;
        }

        CardData card = PlayerDataManager.GetCardById(id);
        GameObject mask = _this.cardBoxScroll.transform.Find("Mask").gameObject;
        Transform cardBoxTr = mask.transform.Find("Contents").Find("ContentsParents").Find("CardBoxObj");
        UIScroll scrollSc = _this.cardBoxScroll.GetComponent<UIScroll>();

        GameObject obj = GameObject.Instantiate(_this.cardBoxCardFrameFrepab, cardBoxTr);
        LobbyCard_CardBox clickSc = obj.GetComponent<LobbyCard_CardBox>();
        clickSc.linkCardData = card;
        clickSc.SetMaskingObject(mask);
        clickSc.SetScroll(scrollSc);

        card.InitCardGraphic(obj.transform.Find("UICardGraphic"));
        _this.cardBoxDic.Add(id, obj);
    }

    static void SubCardInBox(int id) {
        if (!_this.cardBoxDic.ContainsKey(id)) return;
        _this.cardBoxDic[id].SetActive(false);
        _this.cardBoxOrder.Remove(id);
    }

    static public void OnDragUpMyDeckCard(LobbyCard_MyDeck target) {
        CardData card = _this.dragCardTarget;
        if (card == null) {
            return;
        }

        if (!card.DeckLimitCheck(_this.deckSettingTarget)) {
            return;
        }

        HeroActorData hero = _this.deckSettingTarget;
        int cardId = card.GetID();

        int cardOrder = _this.cardBoxOrder.IndexOf(cardId);
        int deckSlotOrder = hero.GetDeckData().IndexOf(cardId);
        if (cardOrder != -1) {
            if (target.linkCardData != null) {
                int beforeId = target.linkCardData.GetID();
                AddCardInBox(beforeId);
                hero.RemoveDeckCard(target.slotNum);
                _this.cardBoxOrder.Add(beforeId);
            }
        } else {
            GameObject beforeSlot = _this.deckCardSlotList[deckSlotOrder];
            GameObject beforeCardGraphic = beforeSlot.transform.Find("UICardGraphic").gameObject;

            hero.RemoveDeckCard(deckSlotOrder);
            if (target.linkCardData != null) {
                hero.RemoveDeckCard(target.slotNum);
                hero.SetDeckCard(deckSlotOrder, target.linkCardData);

                target.linkCardData.InitCardGraphic(beforeCardGraphic.transform);
            } else {
                beforeCardGraphic.SetActive(false);
            }
            beforeSlot.GetComponent<LobbyCard_MyDeck>().linkCardData = target.linkCardData;
        }

        SubCardInBox(cardId);
        hero.SetDeckCard(target.slotNum, card);

        target.linkCardData = card;
        GameObject cardGraphic = target.transform.Find("UICardGraphic").gameObject;
        cardGraphic.SetActive(true);
        card.InitCardGraphic(cardGraphic.transform);

        CardBoxRepositioning();
        InitPassiveText();
    }

    static public void OnRClickBoxCard(LobbyCard_CardBox cardObject) {
        LobbyCard_MyDeck targetSlot = null;
        foreach (GameObject slot in _this.deckCardSlotList) {
            LobbyCard_MyDeck md = slot.GetComponent<LobbyCard_MyDeck>();
            if (md.linkCardData == null) {
                targetSlot = md;
                break;
            }
        }
        if (targetSlot != null) {
            CardData card = cardObject.linkCardData;
            if (!card.DeckLimitCheck(_this.deckSettingTarget)) {
                return;
            }

            HeroActorData hero = _this.deckSettingTarget;
            int cardId = card.GetID();

            int cardOrder = _this.cardBoxOrder.IndexOf(cardId);

            SubCardInBox(cardId);
            hero.SetDeckCard(targetSlot.slotNum, card);

            targetSlot.linkCardData = card;
            GameObject cardGraphic = targetSlot.transform.Find("UICardGraphic").gameObject;
            cardGraphic.SetActive(true);
            card.InitCardGraphic(cardGraphic.transform);

            CardBoxRepositioning();
            InitPassiveText();
        }
    }

    static public void OnRClickDeckCard(LobbyCard_MyDeck cardObject) {
        if (cardObject.linkCardData == null) return;

        int cardId = cardObject.linkCardData.GetID();
        HeroActorData targetHero = _this.deckSettingTarget;

        GameObject slot = cardObject.gameObject;
        GameObject cardGraphic = slot.transform.Find("UICardGraphic").gameObject;
        cardGraphic.SetActive(false);

        targetHero.RemoveDeckCard(cardObject.slotNum);
        cardObject.linkCardData = null;

        AddCardInBox(cardId);
        InitPassiveText();

        _this.cardBoxOrder.Add(cardId);
        CardBoxRepositioning();
    }

    static public void GoStageSelect() {
        AllClose();

        if (_this.stageSelect != null) _this.stageSelect.SetActive(true);

        StageButtonInit();
    }

    static void StageButtonInit() {
        Transform stageTr = _this.stageSelect.transform.Find("SelectList");
        foreach (Transform tr in stageTr) {
            GameObject.Destroy(tr.gameObject);
        }
        int posCount = 0;
        float x = -750f;
        float y = 350f;

        foreach (int i in StageData.GetStageIndexList()) {
            GameObject button = GameObject.Instantiate(_this.stageSelectButtonFrepab, stageTr);

            button.transform.Find("NumText").gameObject.GetComponent<Text>().text = "" + (i);

            button.GetComponent<StageSelectButton_Stage>().stageNum = i;

            button.transform.localPosition = new Vector3(x, y, 0);
            x += 250f;
            ++posCount;
            if (posCount == 6) {
                posCount = 0;
                x = 0;
                y += -250f;
            }
        }
    }
}
