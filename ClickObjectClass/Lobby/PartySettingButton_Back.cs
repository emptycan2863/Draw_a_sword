using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartySettingButton_Back : ClickObject {
    protected override void OnClickEvent(_mouseType button) {
        PlayerDataManager.SaveData();
        LobbySceneObject.GoDeckOrDungeon();
    }
}
