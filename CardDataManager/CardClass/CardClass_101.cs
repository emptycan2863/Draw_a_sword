using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
이성과 본성
리카 전용

액티브
대상에게 [시전자의 잃은 체력의 20%] 의 피해를 입힌다.
시전자는 [시전자의 보유 체력의 10%] 의 방어를 얻는다.

패시브
[힘 +2][지능 +1][민첩 +1]
생명력이 50% 이상일 경우 [행운 +1]
생명력이 25% 미만일 경우 [턴 드로우 +1]
 */
namespace CardClass {
    public class CreastCardClass_101 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_101();
        }
    }

    public class CardClass_101 : CardData {
        int hitCount;
        public override string GetActiveValueText() {
            int maxHp = owner.GetMaxHp();
            int hp = owner.GetHp();

            string text = base.GetActiveValueText();
            text = text.Replace("{0}", "" + ((int)(float)owner.GetStrength() + (int)((float)(maxHp - hp) * 0.1f)));
            text = text.Replace("{1}", "" + (int)((float)hp * 0.1f));
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
            int maxHp = owner.GetMaxHp();
            int hp = owner.GetHp();

            if (code == 1) {
                int guard = (int)((float)hp * 0.1f);
                owner.UseGuard(guard);
            } else {
                int damage = (int)(float)owner.GetStrength() + (int)((float)(maxHp - hp) * 0.1f);
                target.Damage(damage, owner);
            }
            --hitCount;
            if (hitCount == 0) ActionManager.ActionTimeRestart();
        }

        override public bool IsConditionCheck() {
            return true;
        }
        override public void ConditionCheck() {
            int maxHp = owner.GetMaxHp();
            int hp = owner.GetHp();

            if (maxHp / 2 <= hp) owner.conditionalLuck += 1;
            if (maxHp / 4 > hp) owner.conditionalDrawBonus += 1;
        }
    }
}