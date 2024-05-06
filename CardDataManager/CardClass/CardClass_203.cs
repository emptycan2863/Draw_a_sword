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
    public class CreastCardClass_203 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_203();
        }
    }

    public class CardClass_203 : CardData {
        bool isInit = false;
        int ownerAccel = 0;

        public override string GetActiveValueText() {
            string text = base.GetActiveValueText();
            text = text.Replace("{0}", "" + (int)(owner.GetDexterity() * 1.5f));
            return text;
        }

        override public void VirtualUseCardAction(ActorData target) {
            if (owner.GetMp() < 30) {
                useCardEffectList[0].SetFail();
            } else {
                useCardEffectList[0].ShotTarget(target);
                owner.MpCost(30);
                ActionManager.ActionTimeStop();
            }
        }

        public override void VirtualOnHitEvent(ActorData target, int code) {
            int eva = (int)(owner.GetDexterity() * 1.5f);
            target.UseEvasion(eva);
            target.UseShield(30);
            target.SpeedAccel(10);

            ActionManager.ActionTimeRestart();
        }

        override public bool IsConditionCheck() {
            return true;
        }
        override public void ConditionCheck() {
            int accel = owner.GetAccel();
            if (ownerAccel == accel) return;
            if (!isInit) {
                isInit = true;
                ownerAccel = accel;
                return;
            }

            int v = ownerAccel - accel;
            ownerAccel = accel;

            if (v > 0) {
                owner.MpRecovery(v);
            }
        }

        protected override void VirtualReset() {
            isInit = false;
        }
    }
}