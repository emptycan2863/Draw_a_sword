using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 몬스터 전용 공격 카드
 */
namespace CardClass {
    public class CreastCardClass_2 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_2();
        }
    }

    public class CardClass_2 : CardData {
        public override string GetActiveValueText() {
            return base.GetActiveValueText().Replace("{0}", "" + owner.GetEnemyAttack());
        }

        override public void VirtualUseCardAction(ActorData target) {
            if (target.isDead) {
                useCardEffectList[0].SetFail();
            } else {
                useCardEffectList[0].ShotTarget(target);
                ActionManager.ActionTimeStop();
            }
        }

        override public void VirtualOnHitEvent(ActorData target, int code) {
            target.Damage(owner.GetEnemyAttack(), owner);
            ActionManager.ActionTimeRestart();
        }
    }
}