using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewer : ClickObject {
    CardData thisCardData = null;

    public void SetCardData(CardData data) {
        thisCardData = data;
    }

    protected override void OnOverEvent() {
        ChangeInfo(true);
    }

    protected override void OnOutEvent() {
        ChangeInfo(false);
    }

    public void ChangeInfo(bool isOver) {
        if (thisCardData == null) return;
        Transform tr = this.transform.Find("CardPrefab");
        if (isOver) {
            thisCardData.InitCardInfoValueText(tr);
        } else {
            thisCardData.InitCardInfoText(tr);
        }
    }

    public CardData GetCardData() {
        return thisCardData;
    }
}
