using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
오라 블레이드
리카 전용

액티브
MP가 5 미만이라면 불발한다.
MP를 5 소모하고 무작위의 적 대상에 [힘의 70%]의 피해를 입힌다.
MP가 5 미만 혹은, 모든 적을 공격할 때 까지 효과를 반복한다.

패시브
[힘 +2][지능 +2]
자신의 공격으로 적을 처치할 때 마다 MP를 5 회복한다.
*/

namespace CardClass {
    public class CreastCardClass_103 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_103();
        }
    }

    public class CardClass_103 : CardData {
        List<ActorData> attackTargetList = new List<ActorData>();
        UseCardEffectScript nextCardEffect = null;

        public override string GetActiveValueText() {
            return base.GetActiveValueText().Replace("{0}", "" + (int)((float)owner.GetStrength() * 0.8f));
        }

        override public void VirtualUseCardAction(ActorData target) {
            ActionManager.ActionTimeStop();

            attackTargetList.Clear();

            TeamData team = owner.GetTeam();
            foreach (ActorData actor in TurnManager.GetAllActors()) {
                if (actor.GetTeam() != team) {
                    if (actor.isDead) continue;
                    attackTargetList.Add(actor);
                }
            };

            nextCardEffect = useCardEffectList[0];
            NewAction();
        }

        public override void VirtualOnHitEvent(ActorData target, int code) {
            target.Damage((int)((float)owner.GetStrength() * 0.8f), owner);
            NewAction();
        }

        void NewAction() {
            if (owner.GetMp() > 5 && attackTargetList.Count > 0) {
                owner.MpCost(5);
                int index = Random.Range(0, attackTargetList.Count);
                ActorData target = attackTargetList[index];
                attackTargetList.RemoveAt(index);
                nextCardEffect.ShotTarget(target);

                nextCardEffect = AddCardEffect(false);
            } else {
                nextCardEffect.SetFail();
                ActionManager.ActionTimeRestart();
            }
        }

        override public bool IsOnAttack() {
            return true;
        }

        override public void OnAttack(ActorData target, int atk, int damage, int hpDamage, bool isKill) {
            if (isKill) owner.MpRecovery(5);
        }
    }
}
