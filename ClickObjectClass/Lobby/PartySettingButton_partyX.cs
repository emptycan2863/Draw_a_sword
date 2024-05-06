using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartySettingButton_partyX : ClickObject {
    public int index = 0;
    protected override void OnClickEvent(_mouseType button) {
        if (PlayerDataManager.GetPartyMemberCount() < 2) return;
        PlayerDataManager.SetHeroInParty(index, null);
        LobbySceneObject.InitPartyButton();
    }
}
