using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCardObject : ClickObject {
    public CardData linkCardData = null;

    override protected void OnOverEvent() {
        LobbySceneObject.SetCardViewer(linkCardData);
    }
}
