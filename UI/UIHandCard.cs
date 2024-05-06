using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandCard
{
    static GameObject uiObject = null;
    static HeroActorData linkActor = null;
    static List<GameObject> HandCardObjectList = new List<GameObject>();

    static public void Init() {
        uiObject = UIManager.GetUIObject().transform.Find("BattleUI").Find("HandCardUI").gameObject;

        SetUIActive(false);
    }

    static void SetUIActive(bool active) {
        uiObject.SetActive(active);
    }

    static public void CloseUI() {
        linkActor = null;
        SetUIActive(false);
    }

    static public void SetActorHand(HeroActorData actor) {
        if (linkActor != actor) {
            SetUIActive(true);
            linkActor = actor;
            SetCardPos(false);
        }
    }

    static public void HandCardUpdate() {
        if (linkActor != null) {
            SetCardPos(true);
        }
    }

    static void SetCardPos(bool moving) {
        foreach (GameObject destroyObject in HandCardObjectList) GameObject.Destroy(destroyObject);
        HandCardObjectList.Clear();

        foreach (CardData card in linkActor.GetHandCard()) {
            GameObject cardObject = card.HandCardInstant(uiObject.transform);
            HandCardObjectList.Add(cardObject);
            if (card.drawEffect) {
                cardObject.GetComponent<Animator>().Play("DrawEffect");
                card.drawEffect = false;
            }
        }

        int len = HandCardObjectList.Count;
        if (len == 0) return;
        float startPos = (len - 1) * 100;
        if (startPos > 600) startPos = 600;
        for (int i = 0; i < len; ++i) {
            GameObject cardObject = HandCardObjectList[i];
            float pos = len  > 1 ? - startPos + (startPos * 2 / (len-1)) * i : 0;
            cardObject.GetComponent<RectTransform>().localPosition = new Vector3(pos, 0,0);
        }
    }

    static public void ChangedActorHand(ActorData actor) {
        if (linkActor == actor) {
            foreach (GameObject destroyObject in HandCardObjectList) GameObject.Destroy(destroyObject);
            HandCardObjectList.Clear();

            foreach (CardData card in linkActor.GetHandCard()) {
                GameObject cardObject = card.HandCardInstant(uiObject.transform);
                HandCardObjectList.Add(cardObject);
            }
            SetCardPos(false);
        }
    }
}
