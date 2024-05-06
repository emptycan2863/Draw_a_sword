using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewerTagP : ClickObject {
    public GameObject cardViewer = null;

    override protected void OnOverEvent() {
        if (cardViewer == null) return;
        CardViewer cardViewerSc = cardViewer.GetComponent<CardViewer>();
        cardViewerSc.ChangeInfo(true);
        UITag.SetTag(cardViewerSc.GetCardData().GetTagP());
    }
    override protected void OnOutEvent() {
        UITag.SetTag();
        if (cardViewer == null) return;
        CardViewer cardViewerSc = cardViewer.GetComponent<CardViewer>();
        cardViewerSc.ChangeInfo(false);
    }
}
