using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActorData : ActorData {
    EnemyActorBaseData enemyBaseData = null;

    int enemyAttack = 2;
    int enemyHp = 10;
    int enemyMp = 10;
    int enemySp = 10;

    protected List<CardData> deckEnemy = new List<CardData>();

    public EnemyActorData(int index) {
        actorType = _actorType.enemy;
        
        SetLevel(1);
        enemyBaseData = EnemyActorBaseData.GetEnemyData(index);
        enemyAttack = enemyBaseData.GetAttack();
        enemyHp = enemyBaseData.GetHp();
        enemyMp = enemyBaseData.GetMp();
        enemySp = enemyBaseData.GetSp();

        prefabActorStand = PrefabActorStandManager.GatActorStand(enemyBaseData.GetActorStandName());
        iconName = enemyBaseData.GetIconName();

        strength = enemyBaseData.GetStrength();
        intelligence = enemyBaseData.GetIntelligence();
        dexterity = enemyBaseData.GetDexterity();

        foreach (int vnum in enemyBaseData.GetDeckData()) {            
            deckEnemy.Add(CardData.CreateInstantCardData(vnum, this));
        }
    }

    static public EnemyActorData CreateEnemyData(int index) {
        EnemyActorData enemyActor = new EnemyActorData(index);
        return enemyActor;
    }

    override protected void VirtualSetBattleState() {
        maxHp = enemyHp;
        maxMp = enemyMp;
    }

    override public string GetName() {
        return ScriptData.GetScript(enemyBaseData.GetActorNameIndex());
    }

    override protected void SetSpeed() {
        speed = enemySp + conditionalSpeed;
        UIGauge gauge = GetUIGauge();
        if (gauge != null) {
            gauge.SetSp(GetSpeed());
        }
    }

    override public int GetRank() {
        return enemyBaseData.GetRank();
    }

    override public int GetEnemyAttack() {
        return enemyAttack;
    }

    float endTime = 0;
    override public void VirtualTurnStart() {
        TurnManager.SetPhaseEnemyActive();
        int rank = GetRank();

        List<CardData> handCardList = GetHandCard();
        for (int i = 0, len = rank - handCardList.Count; i < len; ++i) {
            CardData card = PullUpCard();

            if (card == null) break;
            card.SendToHand();
        }

        int rankCount = 0;
        List<ActorData> targetList = null;

        do {
            CardData card = handCardList.Count > 0 ? handCardList[0] : null;
            if (card == null) break;
            int cardRank = card.GetRank();
            if (card.GetRank() <= rank - rankCount) {

                switch (card.GetTargetType()) {
                    case _targetType.enemy:
                        targetList = GetEnemyTargetList();
                        if (targetList.Count > 0) {
                            ActorData targetActor = targetList[Random.Range(0, targetList.Count)];
                            card.UseCard(targetActor);
                        }
                        break;
                    case _targetType.self:
                        card.UseCard(this);
                        break;
                }
            }
            rankCount += cardRank;
        } while (rankCount < rank);

        endTime = GameManager.GetTime() + 0.5f;
    }

    public override void VirtualActorUpate() {
        if (myTurn && endTime <= GameManager.GetTime()) {
            TurnManager.TurnEnd();
        }
    }

    protected override List<CardData> GetDeck() {
        return deckEnemy;
    }
}
