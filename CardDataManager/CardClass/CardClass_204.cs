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
    public class CreastCardClass_204 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_204();
        }
    }

    public class CardClass_204 : CardData {
        public override string GetActiveValueText() {
            string text = base.GetActiveValueText();
            text = text.Replace("{0}", "" + ((int)(owner.GetDexterity() * 2.25f)));
            return text;
        }

        override public void VirtualUseCardAction(ActorData target) {
            if (target.isDead) {
                useCardEffectList[0].SetFail();
            } else {
                useCardEffectList[0].ShotTarget(target);
                ActionManager.ActionTimeStop();
                owner.SpeedAccel(-20);
            }
        }

        public override void VirtualOnHitEvent(ActorData target, int code) {
            int damage = (int)(owner.GetDexterity() * 2.25f);
            target.Damage(damage, owner);
            ActionManager.ActionTimeRestart();
        }

        override public bool IsConditionCheck() {
            return true;
        }

        override public void ConditionCheck() {
            if (owner.GetAccel() >= 100) owner.conditionalLuck += 1;
        }
    }
}