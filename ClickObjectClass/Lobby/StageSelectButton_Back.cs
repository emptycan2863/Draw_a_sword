using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectButton_Back : ClickObject {
    protected override void OnClickEvent(_mouseType button) {
        LobbySceneObject.GoDeckOrDungeon();
    }
}
