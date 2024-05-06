using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
���� ���̵�
��ī ����

��Ƽ��
MP�� 5 �̸��̶�� �ҹ��Ѵ�.
MP�� 5 �Ҹ��ϰ� �������� �� ��� [���� 70%]�� ���ظ� ������.
MP�� 5 �̸� Ȥ��, ��� ���� ������ �� ���� ȿ���� �ݺ��Ѵ�.

�нú�
[�� +2][���� +2]
�ڽ��� �������� ���� óġ�� �� ���� MP�� 5 ȸ���Ѵ�.
*/

namespace CardClass {
    public class CreastCardClass_103 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_103();
        }
    }

    public class CardClass_103 : CardData {
        List<ActorData> attackTargetList = new List<ActorData>();
        UseCardEffectScript nextCardEffect = null;

        public override string GetActiveValueText() {
            return base.GetActiveValueText().Replace("{0}", "" + (int)((float)owner.GetStrength() * 0.8f));
        }

        override public void VirtualUseCardAction(ActorData target) {
            ActionManager.ActionTimeStop();

            attackTargetList.Clear();

            TeamData team = owner.GetTeam();
            foreach (ActorData actor in TurnManager.GetAllActors()) {
                if (actor.GetTeam() != team) {
                    if (actor.isDead) continue;
                    attackTargetList.Add(actor);
                }
            };

            nextCardEffect = useCardEffectList[0];
            NewAction();
        }

        public override void VirtualOnHitEvent(ActorData target, int code) {
            target.Damage((int)((float)owner.GetStrength() * 0.8f), owner);
            NewAction();
        }

        void NewAction() {
            if (owner.GetMp() > 5 && attackTargetList.Count > 0) {
                owner.MpCost(5);
                int index = Random.Range(0, attackTargetList.Count);
                ActorData target = attackTargetList[index];
                attackTargetList.RemoveAt(index);
                nextCardEffect.ShotTarget(target);

                nextCardEffect = AddCardEffect(false);
            } else {
                nextCardEffect.SetFail();
                ActionManager.ActionTimeRestart();
            }
        }

        override public bool IsOnAttack() {
            return true;
        }

        override public void OnAttack(ActorData target, int atk, int damage, int hpDamage, bool isKill) {
            if (isKill) owner.MpRecovery(5);
        }
    }
}
