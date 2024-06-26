using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/*
막기
몬스터 전용

액티브
힘 만큼 방어를 얻는다.
*/
namespace CardClass {
    public class CreastCardClass_1001 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_1001();
        }
    }

    public class CardClass_1001 : CardData {
        public override string GetActiveValueText() {
            return base.GetActiveValueText().Replace("{0}", "" + owner.GetStrength() * 2);
        }

        override public void VirtualUseCardAction(ActorData target) {
            useCardEffectList[0].ShotTarget(owner);
            ActionManager.ActionTimeStop();
        }

        override public void VirtualOnHitEvent(ActorData target, int code) {
            owner.UseGuard(owner.GetStrength() * 2);
            ActionManager.ActionTimeRestart();
        }
    }
}