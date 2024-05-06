using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObject : ClickObject {
    public CardData linkCardData = null;

    override protected void OnOverEvent() {
        UICardViewer.SetCardViewer(linkCardData);
    }
}
