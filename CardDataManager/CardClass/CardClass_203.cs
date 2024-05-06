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