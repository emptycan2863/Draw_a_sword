using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum _actorType {
    none,
    hero,
    enemy,
}

public class ActorData {
    int level = 0;

    protected _actorType actorType = _actorType.none;

    protected int maxHp = 0;
    int hp = 0;

    protected int maxMp = 0;
    int mp = 0;

    protected int speed = 0;

    public bool isDead = false;
    bool isFlip = false;

    int guard = 0;
    int shield = 0;
    int evasion = 0;

    protected int strength = 0;
    protected int intelligence = 0;
    protected int dexterity = 0;
    protected int luck = 0;
    protected int memory = 3;
    protected int drawBonus = 2;

    public int conditionalStrength = 0;
    public int conditionalIntelligence = 0;
    public int conditionalDexterity = 0;
    public int conditionalLuck = 0;
    public int conditionalMemory = 0;
    public int conditionalDrawBonus = 0;

    public int conditionalSpeed = 0;

    int deckStrength = 0;
    int deckIntelligence = 0;
    int deckDexterity = 0;
    int deckLuck = 0;
    int deckMemory = 0;
    int deckDrawBonus = 0;
    int accel = 100;

    public bool turnCheck = false;
    GameObject actorObject = null;

    protected PrefabActorStand prefabActorStand = null;
    protected UIGauge uiGauge = null;
    protected string iconName = "";

    protected List<CardData> deckBattle = new List<CardData>();
    protected List<CardData> deckTomb = new List<CardData>();
    protected List<CardData> handCard = new List<CardData>();

    List<CardData> conditionCheckList = new List<CardData>();
    List<CardData> onAttackList = new List<CardData>();
    List<CardData> onSendTombCostList = new List<CardData>();

    TeamData thisTeam = null;

    protected bool myTurn = false;
    bool startEffectCheck = false;

    static ActorData selectActor = null;

    //특수한 효과에 의한 변수들
    ActorData friendShild = null;

    float dieTime = 0;

    public ActorData() {
        //엑터 스탠드 데이터가 없을 때를 위한 더미
        prefabActorStand = PrefabActorStandManager.GatActorStand("TestEnemy");
    }

    public void SetLevel(int i) {
        level = i;
    }

    public void LVUP() {
        ++level;
    }

    public int GetLevel() {
        return level;
    }

    virtual public string GetName() {
        return "";
    }

    public void SetTeam(TeamData team) {
        thisTeam = team;
    }

    public TeamData GetTeam() {
        return thisTeam;
    }

    public _actorType GetActorType() {
        return actorType;
    }

    public int GetMaxHp() {
        return maxHp;
    }

    public int GetMaxMp() {
        return maxMp;
    }

    public int GetHp() {
        return hp;
    }
    public int GetMp() {
        return mp;
    }

    public int GetStrength() {
        return strength + deckStrength + conditionalStrength;
    }

    public int GetIntelligence() {
        return intelligence + deckIntelligence + conditionalIntelligence;
    }

    public int GetDexterity() {
        return dexterity + deckDexterity + conditionalDexterity;
    }

    public int GetLuck() {
        return luck + deckLuck + conditionalLuck;
    }

    public int GetMemory() {
        return memory + deckMemory + conditionalMemory;
    }

    public int GetDrawBonus() {
        return drawBonus + deckDrawBonus + conditionalDrawBonus;
    }

    virtual public int GetRank() {
        return 0;
    }

    virtual public int GetEnemyAttack() {
        return 0;
    }

    public List<CardData> GetDeckBattle() {
        return deckBattle;
    }

    public List<CardData> GetCardTomb() {
        return deckTomb;
    }

    public List<CardData> GetHandCard() {
        return handCard;
    }

    public string GetIconName() {
        return iconName;
    }

    public Sprite GetIconSprite() {
        return SpriteData.GetSprite("ActorIcon/" + iconName);
    }


    protected List<ActorData> GetEnemyTargetList() {
        List<ActorData> list = new List<ActorData>();
        List<ActorData> actorList = TurnManager.GetAllActors();
        foreach (ActorData actor in actorList) {
            if (actor.GetTeam() != thisTeam) list.Add(actor);
        }
        return list;
    }

    public void SetActorObject(Transform tf, _teamPos type) {
        GameObject actorStand = prefabActorStand.Instantiate(tf);
        actorObject = actorStand;

        Transform graphic = actorStand.transform.Find("Graphic");

        if (type == _teamPos.Right) {
            graphic.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            isFlip = true;
        }


        ActorClickObject aco = graphic.gameObject.GetComponent<ActorClickObject>();
        if (aco != null) {
            aco.ActorClickObjectInit(actorStand, this);
        }

        uiGauge = UIGauge.CreateUIGauge(this);
    }

    public GameObject GetActorObject() {
        return actorObject;
    }

    public Vector3 GetHeadPos() {
        if (actorObject != null && !actorObject.IsDestroyed()) {
            Transform tf = actorObject.transform.Find("Graphic").Find("Head");
            if (tf != null) {
                return tf.position;
            }
        }
        return actorObject.transform.position;
    }
    public Vector3 GetDamagePos() {
        if (actorObject != null && !actorObject.IsDestroyed()) {
            Transform tf = actorObject.transform.Find("Graphic").Find("Damage");
            if (tf != null) {
                return tf.position;
            }
        }
        return actorObject.transform.position;
    }

    public Vector3 GetUseCardPos() {
        if (actorObject != null && !actorObject.IsDestroyed()) {
            Transform tf = actorObject.transform.Find("Graphic").Find("UseCard");
            if (tf != null) {
                return tf.position;
            }
        }
        return actorObject.transform.position;
    }
    
    public Vector3 GetGaugePos() {
        if (actorObject != null && !actorObject.IsDestroyed()) {
            Transform tf = actorObject.transform.Find("Graphic").Find("UIGauge");
            if (tf != null) {
                return tf.position;
            }
        }
        return actorObject.transform.position;
    }

    public Vector3 GetDrawPos() {
        if (GetActorType() == _actorType.hero) {
            if (actorObject != null && !actorObject.IsDestroyed()) {
                Transform tf = GetUIGauge().GetObject().transform.Find("DrawEffectPos");
                if (tf != null) {
                    return tf.position;
                }
            }
        }
        return Vector3.zero;
    }

    public bool GetIsFlip() {
        return isFlip;
    }

    public UIGauge GetUIGauge() {
        return uiGauge;
    }

    public void Damage(int pow, ActorData attacker) {
        if (pow < 0) return;
        if (isDead) return;

        if (friendShild != null && !friendShild.isDead) {
            friendShild.Damage(pow, attacker);
            return;
        }

        if (actorObject != null && !actorObject.IsDestroyed()) {
            Transform tf = actorObject.transform.Find("Graphic").Find("Damage");
            GameObject.Instantiate(DataManager.hitEffect, tf);

            DamageText(_damageTextType.damage, pow);
        }

        int atk = pow;
        int damage = pow;
        int hpDamage = hp;

        if (evasion > 0) {
            if (evasion > pow) {
                SetEvasion(evasion - pow);
                pow = 0;
            } else {
                pow = pow - evasion;
                SetEvasion(0);
            }
        }

        if (pow > 0 && guard > 0) {
            if (guard > pow) {
                SetGuard(guard - pow);
                pow = 0;
            } else {
                pow = pow - guard;
                SetGuard(0);
            }
        }

        if (pow > 0 && shield > 0) {
            if (shield > pow) {
                SetShield(shield - pow);
                pow = 0;
            } else {
                pow = pow - shield;
                SetShield(0);
            }
        }

        SetHp(hp - pow);

        if (hp <= 0) {
            Die();
            hp = 0;
        }

        if (attacker != null) {
            if (hp < pow) damage += hp - pow;
            hpDamage = hpDamage - hp;

            attacker.OnAttackEvent(this, atk, damage, hpDamage, isDead);
        }
    }

    public void OnAttackEvent(ActorData target, int atk, int damage, int hpDamage, bool isKill) {
        foreach (CardData card in onAttackList) {
            card.OnAttack(target, atk, damage, hpDamage, isKill);
        }
    }

    void Die() {
        GetUIGauge().GetObject().SetActive(false);
        isDead = true;
        dieTime = GameManager.GetTime();
        Transform tr = actorObject.transform.Find("Graphic");
        if (tr != null) {
            Transform shadowObject = tr.Find("ShadowObject");
            if (shadowObject != null) shadowObject.gameObject.SetActive(false);
        }
        ActionManager.ActorDie(this);
        SoundManager.PlaySound("die", 0.5f, 1.2f);
    }

    public void HpRecovery(int pow) {
        if (pow < 0) return;
        if (isDead) return;

        if (actorObject != null && !actorObject.IsDestroyed()) {
            DamageText(_damageTextType.heal, pow);
        }

        SetHp(hp + pow);
    }

    public void MpCost(int pow) {
        if (pow < 0) return;
        if (isDead) return;

        if (actorObject != null && !actorObject.IsDestroyed()) {
            DamageText(_damageTextType.mpCost, pow);
        }

        SetMp(mp - pow);
    }

    public void MpRecovery(int pow) {
        if (pow < 0) return;
        if (isDead) return;

        if (actorObject != null && !actorObject.IsDestroyed()) {
            DamageText(_damageTextType.mpRecov, pow);
        }

        SetMp(mp + pow);
    }

    void SetHp(int pow) {
        if (pow < 0) pow = 0;
        hp = pow;
        if (hp < 0) hp = 0;
        if (hp > maxHp) hp = maxHp;
        UIGauge gauge = GetUIGauge();
        if (gauge != null) {
            gauge.SetHp(hp);
        }

        ConditionCheck();
    }

    void SetMp(int pow) {
        if (pow < 0) pow = 0;
        mp = pow;
        if (mp < 0) mp = 0;
        if (mp > maxMp) mp = maxMp;
        UIGauge gauge = GetUIGauge();
        if (gauge != null) {
            gauge.SetMp(mp);
        }

        ConditionCheck();
    }

    virtual protected void SetSpeed() {
    }

    public int GetSpeed() {
        return (int)((float)speed * (float)accel / 100.0f);
    }

    public int GetAccel() {
        return accel;
    }

    public void SpeedAccel(int v) {
        accel += v;
        if (accel > 125) accel = 125;
        if (accel < 25) accel = 25;

        UIGauge gauge = GetUIGauge();
        if (gauge != null) {
            gauge.SetSpeedAndAccel(GetSpeed(), accel);
        }

        if (actorObject != null && !actorObject.IsDestroyed()) {
            DamageText(_damageTextType.accel, v);
        }

        ConditionCheck();
    }

    void SetGuard(int pow) {
        if (pow < 0) pow = 0;
        guard = pow;
        UIGauge gauge = GetUIGauge();
        if (gauge != null) {
            gauge.SetGuard(guard);
        }

        ConditionCheck();
    }

    void SetShield(int pow) {
        if (pow < 0) pow = 0;
        shield = pow;
        UIGauge gauge = GetUIGauge();
        if (gauge != null) {
            gauge.SetShield(shield);
        }

        ConditionCheck();
    }

    void SetEvasion(int pow) {
        if (pow < 0) pow = 0;
        evasion = pow;
        UIGauge gauge = GetUIGauge();
        if (gauge != null) {
            gauge.SetEvasion(evasion);
        }

        ConditionCheck();
    }

    //방어(가드)는 보유 수치와 획득 수치중 높은 수치를 따라간다.
    public void UseGuard(int pow) {
        guard = guard>pow?guard:pow;

        if (actorObject != null && !actorObject.IsDestroyed()) {
            Transform tf = actorObject.transform.Find("Graphic").Find("Damage");
            GameObject.Instantiate(DataManager.guardEffect, tf);
        }

        UIGauge gauge = GetUIGauge();
        if (gauge != null) {
            gauge.SetGuard(guard);
        }

        if (actorObject != null && !actorObject.IsDestroyed()) {
            DamageText(_damageTextType.guard, pow);
        }
    }

    //보호막(쉴드)는 중첩과 유지에 제한이 없다.
    public void UseShield(int pow) {
        shield += pow;

        if (actorObject != null && !actorObject.IsDestroyed()) {
            Transform tf = actorObject.transform.Find("Graphic").Find("Damage");
            GameObject.Instantiate(DataManager.shildEffect, tf);
        }

        UIGauge gauge = GetUIGauge();
        if (gauge != null) {
            gauge.SetShield(shield);
        }

        if (actorObject != null && !actorObject.IsDestroyed()) {
            DamageText(_damageTextType.shild, pow);
        }
    }
    //방어(가드)는 보유 수치와 획득 수치중 높은 수치를 따라간다.
    public void UseEvasion(int pow) {
        evasion += pow;

        if (actorObject != null && !actorObject.IsDestroyed()) {
            Transform tf = actorObject.transform.Find("Graphic").Find("Damage");
            GameObject.Instantiate(DataManager.evaEffect, tf);
        }

        UIGauge gauge = GetUIGauge();
        if (gauge != null) {
            gauge.SetEvasion(evasion);
        }

        if (actorObject != null && !actorObject.IsDestroyed()) {
            DamageText(_damageTextType.eva, pow);
        }
    }

    public void ResetEvasion() {
        evasion = 0;
        UIGauge gauge = GetUIGauge();
        if (gauge != null) {
            gauge.SetEvasion(0);
        }
    }

    public void SetFriendShild(ActorData actor) {
        if (actor == this) return;
        if (actor.GetTeam() != this.GetTeam()) return;
        actor.ResetFriendShild();
        friendShild = actor;
    }

    public void ResetFriendShild() {
        friendShild = null;
    }

    protected virtual List<CardData> GetDeck() {
        return new List<CardData>();
    }

    public void SetBattleState() {
        deckStrength = 0;
        deckIntelligence = 0;
        deckDexterity = 0;
        deckLuck = 0;
        deckMemory = 0;
        deckDrawBonus = 0;

        conditionalStrength = 0;
        conditionalIntelligence = 0;
        conditionalDexterity = 0;
        conditionalLuck = 0;
        conditionalMemory = 0;
        conditionalDrawBonus = 0;

        List<CardData> deck = GetDeck();

        foreach (CardData card in deck) {
            deckStrength += card.GetStrength();
            deckIntelligence += card.GetIntelligence();
            deckDexterity += card.GetDexterity();
            deckLuck += card.GetLuck();
            deckMemory += card.GetMemory();
            deckDrawBonus += card.GetDrawBonus();
        }

        conditionCheckList.Clear();
        onAttackList.Clear();
        onSendTombCostList.Clear();
        deckBattle.Clear();

        foreach (CardData card in deck) {
            card.Reset();
            card.SendToDeckBattle();
            if (card.IsConditionCheck()) conditionCheckList.Add(card);
            if (card.IsOnAttack()) onAttackList.Add(card);
            if (card.IsOnOwnerSendTombCost()) onSendTombCostList.Add(card);
        }

        deckTomb.Clear();
        handCard.Clear();

        VirtualSetBattleState();
        ConditionCheck();
        accel = 100;
        SetSpeed();

        SetHp(maxHp);
        SetMp(maxMp);
        guard = 0;
        shield = 0;
        evasion = 0;

        isDead = false;
        startEffectCheck = false;
    }

    virtual protected void VirtualSetBattleState() {
    }

    protected CardData PullUpCard() {
        int count = deckBattle.Count;
        if (count == 0) {
            if (deckTomb.Count == 0) return null;
            while (deckTomb.Count > 0) deckTomb[0].SendToDeckBattle();
            count = deckBattle.Count;
        }
        int targetNum = UnityEngine.Random.Range(0, count);
        CardData card = deckBattle[targetNum];
        card.SendToDraw();
        return card;
    }

    public void TurnStart() {
        if (startEffectCheck == false) {
            List<CardData> cardList = GetDeckBattle();
            foreach (CardData card in cardList) {
                card.StartEffect();
            }
            startEffectCheck = true;
        }

        SoundManager.PlaySound("TurnEnd", 0.3f, 1.0f);
        myTurn = true;
        MyTurnEffect();
        VirtualTurnStart();
    }

    virtual public void VirtualTurnStart() {
        TurnManager.TurnEnd();
    }

    public void TurnEnd() {
        myTurn = false;
        MyTurnEffectDestroy();
        SetSpeed();
        VirtualTurnEnd();
    }

    virtual public void VirtualTurnEnd() {
    }

    public void CycleReset() {
        ResetEvasion();
        ResetFriendShild();
        VirtualCycleReset();
    }

    virtual public void VirtualCycleReset() {
    }    

    public void ObjectDarkly(bool check) {
        if (isDead) return;
        float c = check ? 0.3f : 1;
        if (actorObject != null && !actorObject.IsDestroyed() && actorObject.activeSelf) {
            Transform tf = actorObject.transform.Find("Graphic");
            if (tf != null) {
                SpriteRenderer spr = tf.Find("CharacterImage").gameObject.GetComponent<SpriteRenderer>();
                if (spr != null) spr.color = new Color(c, c, c, 1);
            }
        }
    }

    public void ActorUpate() {
        if (isDead && dieTime > 0) {
            float a = (dieTime + 1.2f - GameManager.GetTime()) / 1.2f;
            if (a > 1) a = 1;
            if (a < 0) a = 0;
            Transform tf = actorObject.transform.Find("Graphic");
            if (tf != null) {
                SpriteRenderer spr = tf.Find("CharacterImage").gameObject.GetComponent<SpriteRenderer>();
                if (spr != null) spr.color = new Color(0, 0, 0, a);
            }
            if (a == 0) {
                actorObject.SetActive(false);
                dieTime = 0;
            }
        }

        if (uiGauge != null) uiGauge.FrameUpdate();
        VirtualActorUpate();
    }

    virtual public void VirtualActorUpate() {
    }
    
    public void ConditionCheck() {
        conditionalStrength = 0;
        conditionalIntelligence = 0;
        conditionalDexterity = 0;
        conditionalLuck = 0;
        conditionalMemory = 0;
        conditionalDrawBonus = 0;
        conditionalSpeed = 0;

        foreach (CardData card in conditionCheckList) {
            card.ConditionCheck();
        }
    }

    public void OnSendTombCost(CardData card) {
        foreach (CardData card2 in onSendTombCostList) {
            card2.OnOwnerSendTombCost(card);
        }
    }

    GameObject SelectObject = null;

    public void SelectEffect() {
        if (selectActor == this) return;

        if (selectActor != null) {
            selectActor.SelectEffectDestroy();
        }

        if (actorObject != null && !actorObject.IsDestroyed()) {
            Transform tf = actorObject.transform.Find("Graphic").Find("ShadowObject").Find("SelectObject");
            SelectObject = GameObject.Instantiate(DataManager.selectEffect, tf);
            selectActor = this;
        }

        UIActorInfo.SetActorData(this);
    }

    public void SelectEffectDestroy() {
        if (actorObject != null && !actorObject.IsDestroyed() && SelectObject != null) {
            GameObject.Destroy(SelectObject);
            selectActor = null;
        }
    }

    GameObject MyTurnObject = null;

    public void MyTurnEffect() {
        if (actorObject != null && !actorObject.IsDestroyed()) {
            Transform tf = actorObject.transform.Find("Graphic").Find("ShadowObject").Find("SelectObject");
            MyTurnObject = GameObject.Instantiate(DataManager.myTurnEffect, tf);
        }
    }

    public void MyTurnEffectDestroy() {
        if (actorObject != null && !actorObject.IsDestroyed() && MyTurnObject != null) {
            GameObject.Destroy(MyTurnObject);
        }
    }

    public List<GameObject> damageList = new List<GameObject>();
    void DamageText(_damageTextType type, int pow) {        
        int count = -1;
        bool reset = true;
        for (int i = 0, len = damageList.Count; i < len; ++i) {
            if (damageList[i].IsDestroyed()) {
                if (count == -1) {
                    count = i;
                }
            } else {
                reset = false;
            }
        }
        if (reset) {
            damageList.Clear();
            count = -1;
        }

        if (count == -1) {
            damageList.Add(null);
            count = damageList.Count -1;
        }

        Vector3 pos = GameManager.mainCamera.WorldToScreenPoint(GetGaugePos());
        pos.x += -60;
        pos.y += 30 + 20 * count;
        GameObject go = UIDamageText.AddDamageText(type, pow, pos);
        damageList[count] = go;
    }

    virtual public void DrawEffect(CardData card) {
    }
}
