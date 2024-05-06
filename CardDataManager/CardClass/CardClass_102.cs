using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
선봉의 책임
리카 전용

액티브
자신은 [힘의 150%]의 방어와 [민첩의 100%]의 회피를 얻는다.
자신 이외의 아군을 대상으로 하였을 경우, 이번 사이클 동안 대상이 받는 피해를 자신이 대신 받는다.

패시브
[힘 +2][민첩 +2]
[힘의 400%] 만큼 SPEED가 증가한다. */
namespace CardClass {
    public class CreastCardClass_102 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_102();
        }
    }

    public class CardClass_102 : CardData {
        int hitCount;

        public override string GetActiveValueText() {
            string text = base.GetActiveValueText();
            text = text.Replace("{0}", "" + (int)(owner.GetStrength() * 1.5f));
            text = text.Replace("{1}", "" + (int)(owner.GetDexterity() * 1.0f));
            return text;
        }

        public override string GetPassiveValueText() {
            return base.GetPassiveValueText().Replace("{0}", "" + (int)(owner.GetStrength() * 4.0f));
        }

        override public void VirtualUseCardAction(ActorData target) {
            useCardEffectList[0].ShotTarget(owner);
            hitCount = 1;
            if (target != owner && !target.isDead) {
                UseCardEffectScript effecSc = AddCardEffect(false);
                effecSc.SetCode(1);
                effecSc.ShotTarget(target);
                ++hitCount;
            }
            ActionManager.ActionTimeStop();
        }

        public override void VirtualOnHitEvent(ActorData target, int code) {
            if (code == 1) {
                if (target != owner) target.SetFriendShild(owner);
            } else {
                int guard = (int)(owner.GetStrength() * 1.5f);
                owner.UseGuard(guard);
                int eva = (int)(owner.GetDexterity() * 1.0f);
                owner.UseEvasion(eva);
            }

            --hitCount;
            if (hitCount == 0) ActionManager.ActionTimeRestart();
        }

        override public bool IsConditionCheck() {
            return true;
        }

        override public void ConditionCheck() {
            owner.conditionalSpeed += (int)(owner.GetStrength() * 4.0f);
        }
    }
}