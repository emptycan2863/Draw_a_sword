using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
���� ��
���� ī��

��Ƽ��
���� MP�� 5%�� �Ҹ��Ѵ�.
�Ҹ��� MP�� 250%�� �������� ������.
 */
namespace CardClass {
    public class CreastCardClass_6 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_6();
        }
    }

    public class CardClass_6 : CardData {
        int mpCost = 0;

        public override string GetActiveValueText() {
            string text = base.GetActiveValueText();
            int cost = (int)(owner.GetMp() * 0.03f);
            text = text.Replace("{0}", "" + cost);
            text = text.Replace("{1}", "" + (int)(cost * 4.0f));
            return text;
        }

        override public void VirtualUseCardAction(ActorData target) {
            if (target.isDead) {
                useCardEffectList[0].SetFail();
            } else {
                useCardEffectList[0].ShotTarget(target);
                mpCost = (int)(owner.GetMp() * 0.03f);
                owner.MpCost(mpCost);
                ActionManager.ActionTimeStop();
            }
        }

        override public void VirtualOnHitEvent(ActorData target, int code) {
            int damage = (int)(mpCost * 4.0f);
            target.Damage(damage, owner);
            ActionManager.ActionTimeRestart();
        }
    }
}