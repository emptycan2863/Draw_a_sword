using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class UICardSelect : MonoBehaviour {
    static GameObject thisUI = null;
    static GameObject selectOKButton = null;
    public GameObject pSelectOKButton = null;
    static GameObject cardSelectScroll = null;
    public GameObject pCardSelectScroll = null;
    static GameObject scrollContents = null;
    public GameObject pScrollContents = null;
    static UIScroll scrollSc = null;

    public class SelectCardCallBackClass : CallBackClass {
        public List<CardData> selectCard = new List<CardData>();
        public List<CardData> notSelectedCard = new List<CardData>();
    }
    class SelectCardLClick : CallBackClass {
        public CardData cardData = null;
        public GameObject cardObject = null;
        public override void Func() {
            if (cardData == null) return;
            int indexOf = selectCardList.IndexOf(this);
            Card_CardSelect cardUIscript = cardObject.GetComponent<Card_CardSelect>();

            if (indexOf == -1) {
                if (selectCardMax == 1 && selectCardList.Count == 1) {
                    selectCardList[0].cardObject.GetComponent<Card_CardSelect>().SelectCheck(false);
                    selectCardList.Clear();
                }
                if (selectCardList.Count >= selectCardMax) return;
                selectCardList.Add(this);
                cardUIscript.SelectCheck(true);
            } else {
                selectCardList.RemoveAt(indexOf);
                cardUIscript.SelectCheck(false);
            }

            CardSelectButtonUpdate();
        }
    }
    class CardSelectButtonCallBask : ButtonObjectCallBack {
        public override void Func() {
            if (mouseType == _mouseType.l) {
                thisUI.SetActive(false);
                if (selectCardCallBackClass != null) {
                    selectCardCallBackClass.selectCard.Clear();
                    foreach (SelectCardLClick CardClickClass in selectCardList) {
                        selectCardCallBackClass.notSelectedCard.Remove(CardClickClass.cardData);
                        selectCardCallBackClass.selectCard.Add(CardClickClass.cardData);
                    }
                }
                selectCardCallBackClass.Func();
            }
        }
    }

    static SelectCardCallBackClass selectCardCallBackClass = null;
    static int selectCardMin = 0;
    static int selectCardMax = 0;
    static List<SelectCardLClick> selectCardList = new List<SelectCardLClick>();


    void Awake() {
        thisUI = this.gameObject;
        selectOKButton = pSelectOKButton;
        cardSelectScroll = pCardSelectScroll;
        scrollContents = pScrollContents;
        scrollSc = cardSelectScroll.GetComponent<UIScroll>();
        thisUI.SetActive(false);

        InitCardSelectButton();
    }

    static void InitCardSelectButton() {
        selectOKButton = thisUI.transform.Find("OKButton").gameObject;
        ButtonObject selectButtonScript = selectOKButton.GetComponent<ButtonObject>();
        selectButtonScript.SetButtonActive(false);
        selectButtonScript.SetOnClickCallBackClass(new CardSelectButtonCallBask());
    }

    static public void SelectCard(List<CardData> cardList, SelectCardCallBackClass cbc = null, int minCount = 1, int maxCount = 1, string text = "") {
        if (cardList.Count <= minCount) {
            foreach (CardData card in cardList) cbc.selectCard.Add(card);
            cbc.Func();
            return;
        }
        foreach (CardData card in cardList) cbc.notSelectedCard.Add(card);

        thisUI.SetActive(true);

        selectCardCallBackClass = cbc;
        selectCardMin = minCount;
        selectCardMax = minCount > maxCount ? minCount : maxCount;
        selectCardList.Clear();

        RectTransform rt = thisUI.GetComponent<RectTransform>();
        float height = 350;
        float yPos = 0;
        int widthCount = cardList.Count;
        if (widthCount > 5) {
            widthCount = 5;
            height = 500;
            yPos = -150;
        }
        float width = 200 * widthCount + 50;
        float xPos = (widthCount - 1) * -100;
        rt.sizeDelta = new Vector2(width, height);

        scrollSc.SetSize(width, height);
        scrollSc.SetMaskSize(width - 50, height - 50);

        foreach (Transform child in scrollContents.transform) {
            Destroy(child.gameObject);
        }

        int coulmnCount = 0;
        int rowCount = 1;

        foreach (CardData card in cardList) {
            ++coulmnCount;
            if (coulmnCount > 5) {
                coulmnCount -= 5;
                xPos = (widthCount - 1) * -100;
                yPos -= 300;
                ++rowCount;
            }
            GameObject cardObject = card.CardSelectInstant(scrollContents.transform);
            cardObject.GetComponent<RectTransform>().localPosition = new Vector2(xPos, yPos);
            cardObject.GetComponent<ClickObject>().SetScroll(scrollSc);

            SelectCardLClick lClickCbc = new SelectCardLClick();
            lClickCbc.cardData = card;
            lClickCbc.cardObject = cardObject;
            Card_CardSelect clickScript = cardObject.GetComponent<Card_CardSelect>();

            clickScript.SetLClickFn(lClickCbc);
            clickScript.SetMaskingObject(cardSelectScroll.transform.Find("Mask").gameObject);

            xPos += 200;
        }

        scrollSc.SetContentsSize(rowCount * 300);
        scrollSc.SetScrollPos(0);

        RectTransform okButton = selectOKButton.GetComponent<RectTransform>();
        if (rowCount > 1) {
            okButton.localPosition = new Vector3(0, -285, 0);
        } else {
            okButton.localPosition = new Vector3(0, -210, 0);
        }

        Text buttonText = okButton.transform.Find("Text").gameObject.GetComponent<Text>();
        if (text == "") {
            text = ScriptData.GetScript(300003);
        }

        if (minCount >= maxCount) {
            text = text.Replace("{0}", ScriptData.GetScript(300001));
            text = text.Replace("{0}", "" + minCount);
        } else {
            text = text.Replace("{0}", ScriptData.GetScript(300002));
            text = text.Replace("{0}", "" + minCount);
            text = text.Replace("{1}", "" + maxCount);
        }

        buttonText.text = text;

        CardSelectButtonUpdate();
    }

    static void CardSelectButtonUpdate() {
        int selectCount = selectCardList.Count;
        ButtonObject selectButtonScript = selectOKButton.GetComponent<ButtonObject>();
        selectButtonScript.SetButtonActive(selectCount >= selectCardMin && selectCount <= selectCardMax);
    }
}
