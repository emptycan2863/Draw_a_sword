using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGauge : MonoBehaviour {
    static List<UIGauge> GaugeList = new List<UIGauge>();

    static public UIGauge CreateUIGauge(ActorData actor) {
        GameObject actorObj = actor.GetActorObject();
        if (actorObj == null) return null;

        Transform parents = actorObj.transform.Find("Graphic").Find("UIGauge");
        foreach (Transform tr in parents) {
            Destroy(tr.gameObject);
        }
        Transform parentsUI = UIManager.GetUIObject().transform.Find("BattleUI").Find("GaugeUI");
        GameObject GaugeObject = null;
        bool isEnemy = false;

        switch (actor.GetActorType()) {
            case _actorType.hero:
                GaugeObject = Instantiate(DataManager.uiGaugeHeroPF, parentsUI);
                GaugeObject.transform.Find("HandCardCount").gameObject.GetComponent<GuageHandCard>().hero = (HeroActorData)actor;
                break;
            case _actorType.enemy:
                GaugeObject = Instantiate(DataManager.uiGaugeEnemyPF, parentsUI);
                isEnemy = true;
                break;

        }

        Vector3 pos = GameManager.mainCamera.WorldToScreenPoint(actor.GetGaugePos());
        pos.z = 0;
        GaugeObject.transform.position = pos;
        UIGauge uiGaugeSc = GaugeObject.GetComponent<UIGauge>();
        uiGaugeSc.isEnemy = isEnemy;
        uiGaugeSc.thisActor = actor;
        uiGaugeSc.thisObject = GaugeObject;

        uiGaugeSc.hp = uiGaugeSc.hpMax = actor.GetMaxHp();
        uiGaugeSc.mp = uiGaugeSc.mpMax = actor.GetMaxMp();
        uiGaugeSc.sp = actor.GetSpeed();
        uiGaugeSc.cardCount = 0;
        uiGaugeSc.UpdateText();

        uiGaugeSc.hpRatio = 1;
        uiGaugeSc.hpMaskRatio = 1;
        uiGaugeSc.mpRatio = 1;
        uiGaugeSc.mpMaskRatio = 1;

        GaugeList.Add(uiGaugeSc);

        return uiGaugeSc;
    }

    ActorData thisActor = null;
    GameObject thisObject = null;
    public GameObject hpGauge = null;
    public GameObject hpDamage = null;
    public GameObject hpText = null;
    public GameObject mpGauge = null;
    public GameObject mpDamage = null;
    public GameObject mpText = null;
    public GameObject spText = null;
    public GameObject cardText = null;
    public GameObject sdGaugeObject = null;
    public GameObject sdGaugeImage = null;
    public GameObject gdGaugeObject = null;
    public GameObject gdGaugeImage = null;
    public GameObject evGaugeObject = null;
    public GameObject evGaugeImage = null;

    bool isEnemy = false;
    bool doUpdate = false;

    int hpMax = 1;
    int hp = 1;
    int mpMax = 1;
    int mp = 1;
    int sp = 1;
    int accel = 100;
    int cardCount = 1;

    int guard = 0;
    int shield = 0;
    int evasion = 0;

    float hpRatio = 0.0f;
    float hpMaskRatio = 0.0f;
    float hpDelay = 0.0f;
    float mpRatio = 0.0f;
    float mpMaskRatio = 0.0f;
    float mpDelay = 0.0f;

    public void FrameUpdate() {
        if (!doUpdate) return;

        if (hpMaskRatio != hpRatio && hpDelay < GameManager.GetTime()) {
            if (hpRatio < hpMaskRatio) {
                hpMaskRatio -= GameManager.GetDeltaTime() * 0.3f;
                if (hpMaskRatio < hpRatio) hpMaskRatio = hpRatio;
            } else {
                hpMaskRatio = hpRatio;
            }
        }
        hpDamage.transform.localScale = new Vector3(hpMaskRatio, 1f, 1f);

        if (!isEnemy) {
            if (mpMaskRatio != mpRatio && mpDelay < GameManager.GetTime()) {
                if (mpRatio < mpMaskRatio) {
                    mpMaskRatio -= GameManager.GetDeltaTime() * 0.3f;
                    if (mpMaskRatio < mpRatio) mpMaskRatio = mpRatio;
                } else {
                    mpMaskRatio = mpRatio;
                }
            }
            mpDamage.transform.localScale = new Vector3(mpMaskRatio, 1f, 1f);
        }

        if (hpMaskRatio == hpRatio && mpMaskRatio == mpRatio) doUpdate = false;
    }

    public void SetHp(int v) {
        if (v < 0) v = 0;
        if (v > hpMax) v = hpMax;
        if (hp > v) hpDelay = GameManager.GetTime() + 0.4f;
        hp = v;

        hpRatio = (float)hp / (float)hpMax;
        hpGauge.transform.localScale = new Vector3(hpRatio, 1f, 1f);

        doUpdate = true;
        UpdateText();
    }

    public void SetMp(int v) {
        if (isEnemy) return;
        if (v < 0) v = 0;
        if (v > mpMax) v = mpMax;
        if (mp > v) mpDelay = GameManager.GetTime() + 0.4f;
        mp = v;

        mpRatio = (float)mp / (float)mpMax;
        mpGauge.transform.localScale = new Vector3(mpRatio, 1f, 1f);

        doUpdate = true;
        UpdateText();
    }

    public void SetSp(int v) {
        if (v < 0) v = 0;
        sp = v;
        UpdateText();
    }

    public void SetSpeedAndAccel(int a, int b) {
        if (a < 0) a = 0;
        sp = a;
        accel = b;
        UpdateText();
    }    

    public void SetCardCount(int v) {
        cardCount = v;
        UpdateText();
    }

    public void SetShield(int v) {
        if (v < 0) v = 0;
        shield = v;
        AddHpUpdate();
        UpdateText();
    }

    public void SetGuard(int v) {
        if (v < 0) v = 0;
        guard = v;
        AddHpUpdate();
        UpdateText();
    }

    public void SetEvasion(int v) {
        if (v < 0) v = 0;
        evasion = v;
        AddHpUpdate();
        UpdateText();
    }

    void AddHpUpdate() {
        int max = shield + guard + evasion;
        if (max < hpMax) max = hpMax;

        float shieldRatio = (float)shield / (float)max;
        float guardRatio = (float)guard / (float)max;
        float evasionRatio = (float)evasion / (float)max;
        RectTransform trTemp = null;

        if (shield > 0) {
            sdGaugeObject.SetActive(true);
            trTemp = sdGaugeImage.GetComponent<RectTransform>();
            trTemp.sizeDelta = new Vector2(shieldRatio * 130, trTemp.sizeDelta.y);
        } else {
            sdGaugeObject.SetActive(false);
        }

        if (guard > 0) {
            gdGaugeObject.SetActive(true);
            trTemp = gdGaugeImage.GetComponent<RectTransform>();
            trTemp.anchoredPosition = new Vector2(shieldRatio * 130, 0);
            trTemp.sizeDelta = new Vector2(guardRatio * 130, trTemp.sizeDelta.y);
        } else {
            gdGaugeObject.SetActive(false);
        }

        if (evasion > 0) {
            evGaugeObject.SetActive(true);
            trTemp = evGaugeImage.GetComponent<RectTransform>();
            trTemp.anchoredPosition = new Vector2(shieldRatio * 130 + guardRatio * 130, 0);
            trTemp.sizeDelta = new Vector2(evasionRatio * 130, trTemp.sizeDelta.y);
        } else {
            evGaugeObject.SetActive(false);
        }
    }

    void UpdateText() {
        string text = "" + hp + "/" + hpMax;
        int addHp = shield + guard + evasion;
        if (addHp > 0) text = text + "(" + addHp + ")";
        hpText.GetComponent<Text>().text = text;
        if (accel == 100) {
            spText.GetComponent<Text>().text = "" + sp;
        } else {
            spText.GetComponent<Text>().text = "" + sp + "(" + accel + "%)";
        }
        if (!isEnemy) {
            mpText.GetComponent<Text>().text = "" + mp + "/" + mpMax;
            cardText.GetComponent<Text>().text = "" + cardCount;
        }

    }

    public GameObject GetObject() {
        return thisObject;
    }

    static public void Clear() {
        foreach (UIGauge guage in GaugeList) {
            GameObject.Destroy(guage.thisObject);
        }
        GaugeList.Clear();
    }
}
