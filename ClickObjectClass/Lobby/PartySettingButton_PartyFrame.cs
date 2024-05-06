using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartySettingButton_PartyFrame : ClickObject {
    public int index = 0;
    protected override void OnClickEvent(_mouseType button) {
        if (PlayerDataManager.GetPartyMemberCount() < index) return;
        PlayerDataManager.GetPartyMemberCount();
        LobbySceneObject.OpenHeroSelectUI(index);
    }
}
