using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIReward : ClickObject {
    static UIReward _this = null;
    static bool isActive = false;
    static int state = 0;
    static int exp = 0;
    static float startTime = 0;
    static float padeEnd = 0;
    static float expActionStart = 0;
    static float expActionEnd = 0;
    static float clickTime = 0;
    static float rewardTime = 0;
    static float lvUpDelay = 0;
    static float getCardDelay = 0;
    static float lvUpPosX = 0;
    static float getCardPosX = 0;
    static float getCardPosY = 0;

    static bool clickOk = false;

    static List<HeroActorData> lvUpHero = null;
    static List<CardData> getCard = null;
    public GameObject lvUPprefab = null;
    public GameObject cardRewardPrefab = null;

    private void Start() {
        _this = this;
        this.GetComponent<RectTransform>().transform.localPosition = Vector3.zero;
        this.gameObject.SetActive(false);
        isActive = false;
    }

    static public void UIUpdate() {
        if (!isActive) return;
        float t = GameManager.GetTime();
        switch (state) {
            case 1:
                float pade = (t - startTime) / (padeEnd - startTime);
                if (pade > 1) pade = 1;
                _this.GetComponent<CanvasGroup>().alpha = pade;

                if (expActionStart < t) {
                    float extRad = (t - expActionStart) / (expActionEnd - expActionStart);
                    if (extRad > 1) extRad = 1;
                    _this.transform.Find("expText").gameObject.GetComponent<Text>().text = "" + (int)(exp * extRad);

                    if (t > clickTime) clickOk = true;

                    if (lvUpHero.Count > 0) {
                        if (t > rewardTime) {
                            HeroActorData hero = lvUpHero[0];
                            GameObject lvUpObj = Instantiate(_this.lvUPprefab, _this.transform.Find("lvUp"));
                            lvUpObj.transform.Find("icon").GetComponent<Image>().sprite = hero.GetIconSprite();
                            RectTransform rt = lvUpObj.GetComponent<RectTransform>();
                            rt.position = new Vector3(rt.position.x + lvUpPosX, rt.position.y, rt.position.z);
                            lvUpPosX += 190;
                            lvUpHero.RemoveAt(0);
                            rewardTime += lvUpDelay;
                        }
                    }
                    if (lvUpHero.Count == 0 && getCard.Count > 0) {
                        if (t > rewardTime) {
                            CardData card = getCard[0];
                            GameObject cardRewardObj = Instantiate(_this.cardRewardPrefab, _this.transform.Find("RewardCard"));
                            cardRewardObj.GetComponent<CardObject>().linkCardData = card;
                            card.InitCardGraphic(cardRewardObj.transform.Find("UICardGraphic"));
                            RectTransform rt = cardRewardObj.GetComponent<RectTransform>();
                            rt.position = new Vector3(rt.position.x + (getCardPosX * 193f), rt.position.y + (getCardPosY * -300f), rt.position.z);
                            getCardPosX++;
                            if (getCardPosX > 6) {
                                getCardPosX -= 7;
                                getCardPosY++;
                            }
                            if (getCardPosY == 2) {
                                getCard = null;
                            }
                            getCard.RemoveAt(0);
                            rewardTime += getCardDelay;
                        }
                    }
                }
                break;
        }
    }

    static public void SetRewardUI(int _exp, List<HeroActorData> _lvUpHero, List<CardData> _getCard) {
        _this.gameObject.SetActive(true);
        isActive = true;
        clickOk = false;
        _this.GetComponent<CanvasGroup>().alpha = 0;

        startTime = GameManager.GetTime();
        padeEnd = startTime + 0.5f;
        expActionStart = startTime + 1.0f;
        expActionEnd = startTime + 1.6f;
        lvUpDelay = 0.25f;
        getCardDelay = 0.25f;
        rewardTime = expActionEnd + lvUpDelay;
        lvUpPosX = 0;
        getCardPosX = 0;
        getCardPosY = 0;

        clickTime = startTime + 3.0f;

        state = 1;
        exp = _exp;
        _this.transform.Find("expText").gameObject.GetComponent<Text>().text = "0";
        lvUpHero = _lvUpHero;
        getCard = _getCard;

        foreach (Transform tr in _this.transform.Find("lvUp")) {
            GameObject.Destroy(tr.gameObject);
        }

        foreach (Transform tr in _this.transform.Find("RewardCard")) {
            GameObject.Destroy(tr.gameObject);
        }
    }

    static public void CloseUI() {
        _this.gameObject.SetActive(false);
        isActive = false;
        clickOk = false;
        state = 0;
    }

    protected override void OnClickEvent(_mouseType button) {
        if (!clickOk) return;
        if (button == _mouseType.l) {
            GameManager.BattleEnd();
            clickOk = false;
        }
    }
}
