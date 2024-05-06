using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/*
침착한 저격수
그로마 전용

액티브
적 대상에 [민첩의 160%]의 피해를 입히고 패를 최대 1장 까지 버릴 수 있다.
패를 버렸을 경우 [민첩의 220%]의 회피를 얻는다.

패시브
[힘 +2][지능 +2][민첩 +6]
전투 시작 시 [민첩의 500%]의 보호막을 얻는다.
*/
namespace CardClass {
    public class CreastCardClass_201 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_201();
        }
    }

    public class CardClass_201 : CardData {
        public override void StartEffect() {
            int guard = owner.GetDexterity() * 5;
            owner.UseGuard(guard);
        }

        public override string GetActiveValueText() {
            string text = base.GetActiveValueText();
            text = text.Replace("{0}", "" + (int)(owner.GetDexterity() * 1.6f));
            text = text.Replace("{1}", "" + (int)(owner.GetDexterity() * 2.2f));
            return text;
        }

        public override string GetPassiveValueText() {
            string text = base.GetPassiveValueText();
            text = text.Replace("{0}", "" + (int)(owner.GetDexterity() * 5.0f));
            return text;
        }

        class CardClass_201Cbc : UICardSelect.SelectCardCallBackClass {
            public CardClass_201 thisCard = null;
            public ActorData owner = null;
            public override void Func() {
                if (selectCard.Count != 0) {
                    foreach (CardData card in selectCard) card.SendToTombCost();
                    ActionManager.UseExtraCard(thisCard, owner);
                }
                ActionManager.ActionTimeRestart();
            }
        }

        override public void VirtualUseCardAction(ActorData target) {
            ActionManager.ActionTimeStop();

            if (target.isDead) {
                useCardEffectList[0].SetFail();
                NewAction();
            } else {
                useCardEffectList[0].ShotTarget(target);
                useCardEffectList[0].SetCode(0);
            }
        }

        public override void VirtualOnHitEvent(ActorData target, int code) {
            if (code == 0) {
                int damage = (int)(owner.GetDexterity() * 1.6f);
                target.Damage(damage, owner);
                NewAction();
            } else {
                int eva = (int)(owner.GetDexterity() * 2.2f);
                owner.UseEvasion(eva);
                ActionManager.ActionTimeRestart();
            }
        }

        void NewAction() {
            CardClass_201Cbc cbc = new CardClass_201Cbc();
            cbc.thisCard = this;
            cbc.owner = owner;

            UICardSelect.SelectCard(owner.GetHandCard(), cbc, 0, 1, ScriptData.GetScript(300005));
        }
        override public void VirtualExtraCardAction(ActorData target) {
            ActionManager.ActionTimeStop();
            useCardEffectList[0].SetCode(1);
            useCardEffectList[0].ShotTarget(target);
        }
    }
}