using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
��ũ�� ���
��ī ����

��Ƽ��
�� ��� [���� 140%]�� ���ظ� ������ �и� �ִ� 1����� ���� �� �ִ�.
�и� ������ ��� �� ī��� �з� ���ư���.

�нú�
[�� +3][���� +1]
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