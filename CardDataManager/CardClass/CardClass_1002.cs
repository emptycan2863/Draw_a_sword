using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/*
����
���� ����

��Ƽ��
�� ��ŭ �� ��´�.
*/
namespace CardClass {
    public class CreastCardClass_1002 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_1002();
        }
    }

    public class CardClass_1002 : CardData {
        public override string GetActiveValueText() {
            return base.GetActiveValueText().Replace("{0}", "" + owner.GetDexterity());
        }

        override public void VirtualUseCardAction(ActorData target) {
            useCardEffectList[0].ShotTarget(owner);
            ActionManager.ActionTimeStop();
        }

        override public void VirtualOnHitEvent(ActorData target, int code) {
            owner.UseEvasion(owner.GetDexterity());
            ActionManager.ActionTimeRestart();
        }
    }
}