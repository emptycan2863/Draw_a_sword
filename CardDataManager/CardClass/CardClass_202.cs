using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/*
속사
그로마 전용

액티브
발동 시 패를 전부 버린다. 버린 카드의 수 + 1회 만큼 적 전체에 [민첩의 40%]의 피해를 입힌다.

패시브
[기억력 +1]
*/
namespace CardClass {
    public class CreastCardClass_202 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_202();
        }
    }

    public class CardClass_202 : CardData {
        int hitCount = 0;
        public override string GetActiveValueText() {
            string text = base.GetActiveValueText();
            text = text.Replace("{0}", "" + (int)(owner.GetDexterity() * 0.4f));
            return text;
        }

        class CardClass_202CardCostEvent : TimeEvent {
            public List<CardData> cardList = null;
            int timeCount = 0;

            public override void Update(float startTime, float endTime, float progressTimeSec, float progressTimeRatio) {
                int thisTimeCount = (int)(progressTimeSec / 0.2f) + 1;
                int costCount = 0;
                if (timeCount < thisTimeCount) {
                    costCount = thisTimeCount - timeCount;
                    timeCount = thisTimeCount;
                }

                while (costCount > 0 && cardList.Count > 0) {
                    --costCount;
                    cardList[0].SendToTombCost();
                    cardList.RemoveAt(0);
                }
            }
        }

        class CardClass_202ShotEvent : TimeEvent {
            public List<ActorData> targetList = null;
            public CardClass_202 _this = null;
            int timeCount = 0;

            public override void Update(float startTime, float endTime, float progressTimeSec, float progressTimeRatio) {
                int thisTimeCount = (int)(progressTimeSec / 0.1f) + 1;
                int shotCount = 0;
                if (timeCount < thisTimeCount) {
                    shotCount = thisTimeCount - timeCount;
                    timeCount = thisTimeCount;
                }

                while (shotCount > 0 && targetList.Count > 0) {
                    --shotCount;

                    if (targetList.Count > 1) {
                        _this.AddCardEffect(false);
                        _this.useCardEffectList[_this.useCardEffectList.Count - 1].ShotTarget(targetList[0]);
                    } else {
                        _this.useCardEffectList[0].ShotTarget(targetList[0]);
                    }

                    targetList.RemoveAt(0);
                }
            }
        }

        override public void VirtualUseCardAction(ActorData target) {
            ActionManager.ActionTimeStop();

            CardClass_202CardCostEvent cardCostEvent = new CardClass_202CardCostEvent();
            List<CardData> cardList = new List<CardData>();
            foreach (CardData card in owner.GetHandCard()) {
                cardList.Add(card);
            }
            cardCostEvent.cardList = cardList;
            float duration = cardList.Count * 0.2f;
            cardCostEvent.SetTimeEvent(0, duration);


            List<ActorData> actorListRand = new List<ActorData>();
            List<ActorData> actorList = new List<ActorData>();

            TeamData team = owner.GetTeam();
            foreach (ActorData actor in TurnManager.GetAllActors()) {
                if (actor.GetTeam() != team) {
                    if (actor.isDead) continue;
                    for (int i = 0, len = cardList.Count + 1; i < len; ++i) {
                        actorListRand.Add(actor);
                    }
                }
            };

            while (actorListRand.Count > 0) {
                int v = Random.Range(0, actorListRand.Count - 1);
                actorList.Add(actorListRand[v]);
                actorListRand.RemoveAt(v);
            }


            CardClass_202ShotEvent shotEvent = new CardClass_202ShotEvent();
            shotEvent.targetList = actorList;
            float duration2 = actorList.Count * 0.1f;
            shotEvent.SetTimeEvent(duration, duration2);
            shotEvent._this = this;

            hitCount = actorList.Count;
        }

        public override void VirtualOnHitEvent(ActorData target, int code) {
            int damage = (int)(owner.GetDexterity() * 0.4f);
            target.Damage(damage, owner);

            hitCount--;
            if (hitCount == 0) ActionManager.ActionTimeRestart();
        }
    }
}