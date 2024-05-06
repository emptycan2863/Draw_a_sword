using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UIManager;

public class UIManager : MonoBehaviour
{
    static GameObject thisUI = null;

    static GameObject fadeUI = null;
    static Image fadeImage = null;
    static GameObject battleUI = null;
    // Start is called before the first frame update
    void Awake()
    {
        thisUI = this.gameObject;

        fadeUI = thisUI.transform.Find("FadeUI").gameObject;
        fadeImage = fadeUI.transform.Find("FadeObject").GetComponent<Image>();

        battleUI = thisUI.transform.Find("BattleUI").gameObject;
        SetBattleUIActive(false);

        UIHandCard.Init();

        DontDestroyOnLoad(thisUI);
    }

    static CallBackClass fadeCbc = null;
    static int fadeState = 0; //0 상태 없음, 1 페이드 인 시작, 2 페이드 인 대기, 3 페이드 아웃
    static int fadeStart = 0;
    static int fadeEnd = 0;

    static public void UpdateFrame() {
        UITag.UIUpdate();

        if (fadeState > 0) {

            int flame = GameManager.GetFlame();
            float a = 0;
            switch (fadeState) {
                case 1:
                    a = (float)(flame - fadeStart) / (float)(fadeEnd - fadeStart);
                    if (flame >= fadeEnd) {
                        ++fadeState;
                        fadeStart = fadeEnd;
                        fadeEnd = fadeStart + 30;
                        if (fadeCbc != null) {
                            fadeCbc.Func();
                            fadeCbc = null;
                        }
                    }
                    break;
                case 2:
                    a = 1;
                    if (flame >= fadeEnd) {
                        ++fadeState;
                        fadeStart = fadeEnd;
                        fadeEnd = fadeStart + 60;
                    }
                    break;
                case 3:
                    a = 1.0f - ((float)(flame - fadeStart) / (float)(fadeEnd - fadeStart));
                    if (flame >= fadeEnd) {
                        fadeState = 0;
                        a = 0;
                        fadeUI.SetActive(false);
                    }
                    break;
            }

            if (a > 1) a = 1;
            if (a < 0) a = 0;
            fadeImage.color = new Color(0, 0, 0, a);
        }
    }

    static public GameObject GetUIObject() {
        return thisUI;
    }

    static public void FadeInOut(CallBackClass cbc) {
        fadeState = 1;
        fadeStart = GameManager.GetFlame();
        fadeEnd = GameManager.GetFlame() + 60;

        fadeCbc = cbc;
        fadeUI.SetActive(true);
    }

    static public bool UIClickCheck() {
        if (fadeState > 0) return false;
        return true;
    }

    static public GameObject IconInstantiate(ActorData actor, Transform parent) {
        GameObject go = GameManager.Instantiate(DataManager.iconPF, parent);
        go.GetComponent<ActorIconClickObject>().actorData = actor;
        go.GetComponent<Image>().sprite = actor.GetIconSprite();
        return go;
    }

    static public void SetBattleUIActive(bool a) {
        battleUI.SetActive(a);
    }

    static public void Clear() {
        UIGauge.Clear();

        Transform effectUI = thisUI.transform.Find("BattleUI").Find("EffectUI");
        foreach (Transform tf in effectUI) {
            GameObject.Destroy(tf.gameObject);
        }

        Transform damageUI = thisUI.transform.Find("BattleUI").Find("DamageUI");
        foreach (Transform tf in damageUI) {
            GameObject.Destroy(tf.gameObject);
        }
    }
}
