using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
�䳢��
���� ī��

��Ƽ��
�ҹ��Ѵ�.
 */
namespace CardClass {
    public class CreastCardClass_5 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_5();
        }
    }

    public class CardClass_5 : CardData {
        override public void VirtualUseCardAction(ActorData target) {
            useCardEffectList[0].SetFail();
        }
    }
}