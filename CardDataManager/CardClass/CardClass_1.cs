using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
���� ����
���� ����

��Ƽ��
��󿡰� [���� 125%]�� ���ظ� ������.

�нú�
[�� +2]
 */
namespace CardClass {
    public class CreastCardClass_1 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_1();
        }
    }

    public class CardClass_1 : CardData {
        public override string GetActiveValueText() {
            return base.GetActiveValueText().Replace("{0}", "" + (int)((float)owner.GetStrength() * 1.25f));
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
            int damage = (int)((float)owner.GetStrength() * 1.25f);
            target.Damage(damage, owner);
            ActionManager.ActionTimeRestart();
        }
    }
}