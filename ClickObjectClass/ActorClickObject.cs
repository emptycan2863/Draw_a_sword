using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorClickObject : ClickObject {
    public GameObject actorObject = null;
    public ActorData thisActorData = null;

    public void ActorClickObjectInit(GameObject actorStand, ActorData actorData) {
        actorObject = actorStand;
        thisActorData = actorData;
    }

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
        if (thisActorData != null) thisActorData.SelectEffect();
    }

    public void OutEvent() {
        if (thisActorData != null) thisActorData.SelectEffectDestroy();
    }
}
