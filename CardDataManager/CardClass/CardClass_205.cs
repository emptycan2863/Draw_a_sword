using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/*
�ӻ�
�׷θ� ����

��Ƽ��
�ߵ� �� �и� ���� ������. ���� ī���� �� + 1ȸ ��ŭ �� ��ü�� [��ø�� 40%]�� ���ظ� ������.

�нú�
[���� +1]
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