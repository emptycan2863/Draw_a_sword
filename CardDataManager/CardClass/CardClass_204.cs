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