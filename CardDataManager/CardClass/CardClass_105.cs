using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/*
능숙한 검술
리카 전용

액티브
발동한 이 카드를 버린다.
이 카드가 버려졌을 경우 자신 마지막으로 피해를 입한 적 혹은 무작위의 대상에게 [힘의 100%]의 피해를 입힌다.

패시브
[힘 +3]
카드가 버려질 때 마다 HP를 2 회복한다. 이 효과로 전투 중 최대 20의 HP를 회복할 수 있다.
*/
namespace CardClass {
    public class CreastCardClass_105 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_105();
        }
    }

    public class CardClass_105 : CardData {
        ActorData lastTarget = null;
        int passiveCount = 0;

        public override string GetActiveValueText() {
            return base.GetActiveValueText().Replace("{0}", "" + (int)((float)owner.GetStrength() * 1.0f));
        }

        public override string GetPassiveValueText() {
            return base.GetPassiveValueText().Replace("{0}", "" + passiveCount);
        }

        protected override void VirtualReset() {
            lastTarget = null;
            passiveCount = 0;
        }

        override public void VirtualUseCardAction(ActorData target) {
            goTomb = false;
            this.SendToTombCost(false);
            CostEffect(useCardEffectList[0]);
        }

        override public void VirtualExtraCardAction(ActorData target) {
            target = null;
            if (lastTarget != null && !lastTarget.isDead) {
                target = lastTarget;
            } else {
                TeamData team = owner.GetTeam();
                List<ActorData> targetList = new List<ActorData>();
                foreach (ActorData actor in TurnManager.GetAllActors()) {
                    if (actor.isDead) continue;
                    if (actor.GetTeam() == team) continue;
                    targetList.Add(actor);
                }
                if (targetList.Count > 0) target = targetList[Random.Range(0, targetList.Count)];
            }

            if (target != null) {
                ActionManager.ActionTimeStop();
                useCardEffectList[0].ShotTarget(target);
            } else {
                useCardEffectList[0].SetFail();
            }
        }

        public override void VirtualOnHitEvent(ActorData target, int code) {
            target.Damage((int)((float)owner.GetStrength() * 1.0f), owner);
            ActionManager.ActionTimeRestart();
        }


        override protected void OnSendTombCost() {
            TeamData team = owner.GetTeam();
            foreach (ActorData actor in TurnManager.GetAllActors()) {
                if (actor.isDead) continue;
                if (actor.GetTeam() == team) continue;

                ActionManager.UseExtraCard(this, owner);
                return;
            }
        }

        public override bool IsOnAttack() {
            return true;
        }

        public override void OnAttack(ActorData target, int atk, int damage, int hpDamage, bool isKill) {
            if (!isKill) lastTarget = target;
        }

        public override bool IsOnOwnerSendTombCost() {
            return true;
        }

        public override void OnOwnerSendTombCost(CardData card) {
            if (passiveCount >= 20) return; 

            owner.HpRecovery(2);
            passiveCount += 2;
        }
    }
}