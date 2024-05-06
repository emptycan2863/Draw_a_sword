using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public static class ActionManager {
    static int actionManagerId = 0;
    enum _actionState { none, run, delay, stop };
    static _actionState actionState = _actionState.none;
    static float delayCheckTime = 0;
    static float delayTime = 0;

    class Action {
        public ActorData user = null;
        public int actionId = 0;

        int lastUseCardId = 0;

        class UseCardData {
            public CardData card = null;
            ActorData target = null;
            public int cardId = 0;

            public UseCardData(CardData _card, ActorData _target, int id) {
                card = _card;
                target = _target;
                cardId = id;
            }

            public void Use() {
                card.UseCardAction(target);
            }
        }

        List<UseCardData> useCardList = new List<UseCardData>();

        public Action(ActorData _user, int id) {
            user = _user;
            actionId = id;
            lastUseCardId = actionId;
            useCardList.Clear();
            UIActionList.AddAction(user, actionId);
        }

        public void UseCard(CardData card, ActorData target) {
            useCardList.Add(new UseCardData(card, target, actionManagerId));
            if (target == user) target = null;
            UIActionList.AddActionCard(card, target, actionId, actionManagerId);
            ++actionManagerId;
        }

        public void RunAction() {
            if (lastUseCardId != actionId) UIActionList.ReleaseAction(lastUseCardId);
            if (useCardList.Count > 0) {
                useCardList[0].Use();
                lastUseCardId = useCardList[0].cardId;
                useCardList.RemoveAt(0);
            } else {
                if (useCardList.Count == 0) ActionObjectEnd();
            }
        }

        public void ResetUseCard() {
            foreach (UseCardData data in useCardList) {
                data.card.SendToHand(false);
            }
        }
    }

    static void ActionObjectEnd() {
        int i = actionList.Count - 1;
        UIActionList.ReleaseAction(actionList[i].actionId);
        actionList.RemoveAt(i);
    }

    static List<Action> actionList = new List<Action>();

    class ExtraAction {
        CardData card = null;
        ActorData target = null;

        public ExtraAction(CardData _card, ActorData _target) {
            card = _card;
            target = _target;
        }

        public void Use() {
            card.ExtraCardAction(target);
        }

        public ActorData GetOwner() {
            return card.GetOwner();
        }
    }

    static List<ExtraAction> extraExtionList = new List<ExtraAction>();

    static Action lastAction = null;

    public static void UseCard(CardData card, ActorData target) {
        Action action = null;
        ActorData user = card.GetOwner();
        int speed = user.GetSpeed();
        if (lastAction != null && lastAction.user == user) {
            action = lastAction;
        } else {
            action = new Action(user, actionManagerId);
            actionList.Add(action);
            ++actionManagerId;
        }

        action.UseCard(card, target);
        lastAction = action;
    }

    public static void ResetUseCard(ActorData actor) {
        if (actor.GetActorType() != _actorType.hero) return;
        if (actionList.Count == 0) return;
        Action ac = actionList[actionList.Count - 1];
        if (ac.user != actor) return;
        ac.ResetUseCard();
        UIActionList.ReleaseAction(ac.actionId);
        actionList.RemoveAt(actionList.Count - 1);
        lastAction = null;
    }

    public static void UseExtraCard(CardData card, ActorData target) {
        extraExtionList.Add(new ExtraAction(card, target));
    }

    public static void ActionManagerUpdate() {
        switch (actionState) {
            case _actionState.none:
                break;
            case _actionState.run:
                actionState = _actionState.delay;
                delayCheckTime = GameManager.GetTime();
                delayTime = 0.3f;

                if (extraExtionList.Count > 0) {
                    extraExtionList[0].Use();
                    extraExtionList.RemoveAt(0);
                } else if (actionList.Count > 0) {
                    actionList[actionList.Count - 1].RunAction();
                } else {
                    ActionEnd();
                }


                break;
            case _actionState.delay:
                if (GameManager.GetTime() > delayCheckTime + delayTime) actionState = _actionState.run;
                break;
            case _actionState.stop:
                break;
        }
    }

    public static void ActionTimeStop() {
        if (actionState == _actionState.delay) {
            actionState = _actionState.stop;
        }
    }

    public static void ActionTimeRestart() {
        if (actionState == _actionState.stop) {
            actionState = _actionState.delay;
            delayCheckTime = GameManager.GetTime();
        }
    }

    public static void ActionStart() {
        actionState = _actionState.run;
    }

    public static void ActionEnd() {
        actionState = _actionState.none;
        UIActionList.Clear();
        lastAction = null;
        TurnManager.NextCycle();
    }

    public static void ActorDie(ActorData actor) {
        for (int i = 0, len = actionList.Count; i < len; ++i) {
            if (actionList[i].user == actor) {
                UIActionList.ReleaseAction(actionList[i].actionId);
                actionList.RemoveAt(i);
                return;
            }
        }
        for (int i = extraExtionList.Count - 1; i >= 0; --i) {
            if(actor == extraExtionList[i].GetOwner()) extraExtionList.RemoveAt(i);
        }
    }
}