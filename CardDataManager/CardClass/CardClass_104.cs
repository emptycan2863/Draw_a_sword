using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
오크의 대검
리카 전용

액티브
적 대상에 [힘의 140%]의 피해를 입히고 패를 최대 1장까지 버릴 수 있다.
패를 버렸을 경우 이 카드는 패로 돌아간다.

패시브
[힘 +3][기억력 +1]
*/
namespace CardClass {
    public class CreastCardClass_104 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_104();
        }
    }

    public class CardClass_104 : CardData {

        public override string GetActiveValueText() {
            return base.GetActiveValueText().Replace("{0}", "" + (int)((float)owner.GetStrength() * 1.4f));
        }

        class CardClass_104Cbc : UICardSelect.SelectCardCallBackClass {
            public CardClass_104 thisCard = null;
            public ActorData owner = null;
            public override void Func() {
                if (selectCard.Count == 0) {
                    thisCard.SendToTomb();
                } else {
                    foreach (CardData card in selectCard) card.SendToTombCost();
                    thisCard.SendToHand();
                }

                ActionManager.ActionTimeRestart();
            }
        }

        override public void VirtualUseCardAction(ActorData target) {
            goTomb = false;
            ActionManager.ActionTimeStop();

            if (target.isDead) {
                useCardEffectList[0].SetFail();
                newAction();
            } else {
                useCardEffectList[0].ShotTarget(target);
            }
        }

        public override void VirtualOnHitEvent(ActorData target, int code) {
            target.Damage((int)((float)owner.GetStrength() * 1.4f), owner);
            newAction();
        }

        void newAction() {
            CardClass_104Cbc cbc = new CardClass_104Cbc();
            cbc.thisCard = this;
            cbc.owner = owner;

            UICardSelect.SelectCard(owner.GetHandCard(), cbc, 0, 1, ScriptData.GetScript(300005));
        }
    }
}