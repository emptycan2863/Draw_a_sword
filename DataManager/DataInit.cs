using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//데이터 로딩을 편하게 하기 위한 오브젝트

public class DataInit : MonoBehaviour {
    static bool isInit = false;
    public GameObject card_CardSelectPf = null;
    public GameObject card_HandCardtPf = null;

    public GameObject actorPositionObjPf = null;

    public GameObject iconPF = null;
    public GameObject uiGaugeHeroPF = null;
    public GameObject uiGaugeEnemyPF = null;

    //테이블 데이터
    public TextAsset drawaswordCardList = null;
    public TextAsset drawaswordEnemyActorList = null;
    public TextAsset drawaswordHeroActorList = null;
    public TextAsset drawaswordStageList = null;
    public TextAsset drawaswordExpTable = null;
    public TextAsset scriptTable = null;
    public TextAsset cardTagList = null;
    public TextAsset drawaswordStartDeck = null;

    //이팩트 관련. 나중에 나눌 가능성 있음
    public GameObject useCardEffect = null;
    public GameObject hitEffect = null;
    public GameObject guardEffect = null;
    public GameObject shildEffect = null;
    public GameObject evaEffect = null;

    public GameObject selectEffect = null;
    public GameObject myTurnEffect = null;

    void Awake() {
        if (isInit) return;

        DataManager.card_CardSelectPf = card_CardSelectPf;
        DataManager.card_HandCardtPf = card_HandCardtPf;

        DataManager.actorPositionObjPf = actorPositionObjPf;

        DataManager.iconPF = iconPF;
        DataManager.uiGaugeHeroPF = uiGaugeHeroPF;
        DataManager.uiGaugeEnemyPF = uiGaugeEnemyPF;

        DataManager.useCardEffect = useCardEffect;
        DataManager.hitEffect = hitEffect;
        DataManager.guardEffect = guardEffect;
        DataManager.shildEffect = shildEffect;
        DataManager.evaEffect = evaEffect;

        DataManager.selectEffect = selectEffect;
        DataManager.myTurnEffect = myTurnEffect;

        CardDataInitManager.Init();

        //테이블
        ScriptData.SetData(scriptTable);

        CardBaseData.SetData(drawaswordCardList);
        EnemyActorBaseData.SetData(drawaswordEnemyActorList);
        HeroActorBaseData.SetData(drawaswordHeroActorList);
        StageData.SetData(drawaswordStageList);
        DataManager.SetExpData(drawaswordExpTable);
        DataManager.SetCardTagData(cardTagList);
        DataManager.SetStartDeck(drawaswordStartDeck);

        SaveDataManager.SaveDataInit();
        isInit = true;
    }
}
