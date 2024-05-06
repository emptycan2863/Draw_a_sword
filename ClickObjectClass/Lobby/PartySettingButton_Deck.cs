using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartySettingButton_Deck : ClickObject {
    public int index = 0;

    protected override void OnClickEvent(_mouseType button) {
        HeroActorData hero = PlayerDataManager.GetHeroInParty(index);
        if (hero == null) return;
        LobbySceneObject.GoDeckSetting(hero);
    }
}
