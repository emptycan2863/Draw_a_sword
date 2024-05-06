using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
���
���� ī��

��Ƽ��
2% �����Ѵ�.
�� ��� [��ø�� 70%]�� ���ظ� ������.
�ڽ��� [��ø�� 100%]�� ȸ�Ǹ� ��´�.
 */
namespace CardClass {
    public class CreastCardClass_4 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_4();
        }
    }

    public class CardClass_4 : CardData {
        int hitCount;

        public override string GetActiveValueText() {
            string text = base.GetActiveValueText();
            text = text.Replace("{0}", "" + ((int)(owner.GetDexterity() * 0.7f)));
            text = text.Replace("{1}", "" + ((int)(owner.GetDexterity() * 1.0f)));
            return text;
        }

        override public void VirtualUseCardAction(ActorData target) {
            UseCardEffectScript buffEffect;
            if (target.isDead) {
                hitCount = 1;
                buffEffect = useCardEffectList[0];
            } else {
                hitCount = 2;
                useCardEffectList[0].ShotTarget(target);
                buffEffect = AddCardEffect(false);
            }
            buffEffect.SetCode(1);
            buffEffect.ShotTarget(owner);

            ActionManager.ActionTimeStop();
        }

        public override void VirtualOnHitEvent(ActorData target, int code) {
            if (code == 1) {
                int eva = (int)(owner.GetDexterity() * 1.0f);
                owner.UseEvasion(eva);
                owner.SpeedAccel(-5);
            } else {
                int damage = (int)(owner.GetDexterity() * 0.7f);
                target.Damage(damage, owner);
            }
            --hitCount;
            if (hitCount == 0) ActionManager.ActionTimeRestart();
        }
    }
}