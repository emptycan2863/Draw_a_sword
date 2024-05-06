using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSettingButton_Save : ClickObject {
    protected override void OnClickEvent(_mouseType button) {
        LobbySceneObject.ClickDeckSettingSaveButton();
    }
}