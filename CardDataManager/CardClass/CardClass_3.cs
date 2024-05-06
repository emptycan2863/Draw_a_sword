using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 몬스터 전용 공격 카드
 */
namespace CardClass {
    public class CreastCardClass_3 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_3();
        }
    }

    public class CardClass_3 : CardData {
        int cost = 0;
        public override string GetActiveValueText() {
            string text = base.GetActiveValueText();
            text = text.Replace("{0}", "" + (int)(owner.GetStrength() * 2.8f));
            text = text.Replace("{1}", "" + (int)(owner.GetStrength() * 1.2f));
            return text;
        }

        class CardClass_3Cbc : UICardSelect.SelectCardCallBackClass {
            public CardClass_3 thisCard = null;
            public ActorData owner = null;
            public override void Func() {
                thisCard.cost = selectCard.Count;
                foreach (CardData card in selectCard) card.SendToTombCost();
                thisCard.useCardEffectList[0].ShotTarget(owner);
            }
        }

        override public void VirtualUseCardAction(ActorData target) {
            ActionManager.ActionTimeStop();
            CardClass_3Cbc cbc = new CardClass_3Cbc();
            cbc.thisCard = this;
            cbc.owner = owner;

            UICardSelect.SelectCard(owner.GetHandCard(), cbc, 0, 1, ScriptData.GetScript(300005));
        }

        public override void VirtualOnHitEvent(ActorData target, int code) {
            if (cost == 0) {
                owner.UseGuard((int)((float)owner.GetStrength() * 1.2f));
            } else {
                owner.UseGuard((int)((float)owner.GetStrength() * 2.8f));
            }
            ActionManager.ActionTimeRestart();
        }
    }
}