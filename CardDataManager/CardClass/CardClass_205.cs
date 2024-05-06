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
    public class CreastCardClass_205 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_205();
        }
    }

    public class CardClass_205 : CardData {
        public override string GetActiveValueText() {
            string text = base.GetActiveValueText();
            text = text.Replace("{0}", "" + ((int)(owner.GetDexterity() * 0.2f)));
            return text;
        }

        override public void VirtualUseCardAction(ActorData target) {
            goTomb = false;
            useCardEffectList[0].ShotTarget(owner);
            ActionManager.ActionTimeStop();
        }

        override public void VirtualOnHitEvent(ActorData target, int code) {
            owner.SpeedAccel(1);
            owner.UseEvasion(((int)(owner.GetDexterity() * 0.2f)));
            ActionManager.ActionTimeRestart();
            SendToHand();
        }
    }
}