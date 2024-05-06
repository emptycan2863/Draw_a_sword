using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/*
�ɼ��� �˼�
��ī ����

��Ƽ��
�ߵ��� �� ī�带 ������.
�� ī�尡 �������� ��� �ڽ� ���������� ���ظ� ���� �� Ȥ�� �������� ��󿡰� [���� 100%]�� ���ظ� ������.

�нú�
[�� +3]
ī�尡 ������ �� ���� HP�� 2 ȸ���Ѵ�. �� ȿ���� ���� �� �ִ� 20�� HP�� ȸ���� �� �ִ�.
*/
namespace CardClass {
    public class CreastCardClass_105 : CreateCardDataCallBack {
        override public CardData Create() {
            return new CardClass_105();
        }
    }

    public class CardClass_105 : CardData {
        ActorData lastTarget = null;
        int passiveCount = 0;

        public override string GetActiveValueText() {
            return base.GetActiveValueText().Replace("{0}", "" + (int)((float)owner.GetStrength() * 1.0f));
        }

        public override string GetPassiveValueText() {
            return base.GetPassiveValueText().Replace("{0}", "" + passiveCount);
        }

        protected override void VirtualReset() {
            lastTarget = null;
            passiveCount = 0;
        }

        override public void VirtualUseCardAction(ActorData target) {
            goTomb = false;
            this.SendToTombCost(false);
            CostEffect(useCardEffectList[0]);
        }

        override public void VirtualExtraCardAction(ActorData target) {
            target = null;
            if (lastTarget != null && !lastTarget.isDead) {
                target = lastTarget;
            } else {
                TeamData team = owner.GetTeam();
                List<ActorData> targetList = new List<ActorData>();
                foreach (ActorData actor in TurnManager.GetAllActors()) {
                    if (actor.isDead) continue;
                    if (actor.GetTeam() == team) continue;
                    targetList.Add(actor);
                }
                if (targetList.Count > 0) target = targetList[Random.Range(0, targetList.Count)];
            }

            if (target != null) {
                ActionManager.ActionTimeStop();
                useCardEffectList[0].ShotTarget(target);
            } else {
                useCardEffectList[0].SetFail();
            }
        }

        public override void VirtualOnHitEvent(ActorData target, int code) {
            target.Damage((int)((float)owner.GetStrength() * 1.0f), owner);
            ActionManager.ActionTimeRestart();
        }


        override protected void OnSendTombCost() {
            TeamData team = owner.GetTeam();
            foreach (ActorData actor in TurnManager.GetAllActors()) {
                if (actor.isDead) continue;
                if (actor.GetTeam() == team) continue;

                ActionManager.UseExtraCard(this, owner);
                return;
            }
        }

        public override bool IsOnAttack() {
            return true;
        }

        public override void OnAttack(ActorData target, int atk, int damage, int hpDamage, bool isKill) {
            if (!isKill) lastTarget = target;
        }

        public override bool IsOnOwnerSendTombCost() {
            return true;
        }

        public override void OnOwnerSendTombCost(CardData card) {
            if (passiveCount >= 20) return; 

            owner.HpRecovery(2);
            passiveCount += 2;
        }
    }
}