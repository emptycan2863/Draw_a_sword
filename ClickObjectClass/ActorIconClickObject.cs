using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorIconClickObject : ClickObject
{
    public ActorData actorData = null;

    override protected void OnOverEvent() {
        OverEvent();
    }

    override protected void OnOutEvent() {
        OutEvent();
    }

    override protected void OnDragOverEvent() {
        OverEvent();
    }

    override protected void OnDragOutEvent() {
        OutEvent();
    }

    public void OverEvent() {
        if (actorData != null) actorData.SelectEffect();
    }

    public void OutEvent() {
        if (actorData != null) actorData.SelectEffectDestroy();
    }
}
