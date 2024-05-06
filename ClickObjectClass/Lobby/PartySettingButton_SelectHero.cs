using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartySettingButton_SelectHero : ClickObject {
    HeroActorData hero = null;
    int partyNum = 0;

    public void SetNumAndHeroData(int num, HeroActorData heroData = null) {
        hero = heroData;
        partyNum = num;
        if (hero != null) {
            int lv = heroData.GetLevel();
            float expRad = (float)heroData.GetExp() / (float)heroData.GetNeedExp();
            this.transform.Find("Icon").gameObject.GetComponent<Image>().sprite = heroData.GetIconSprite();

            GameObject lvObj = this.transform.Find("LV").gameObject;
            lvObj.SetActive(true);
            lvObj.GetComponent<Text>().text = "" + lv;

            this.transform.Find("EXPBar").gameObject.SetActive(true);

            GameObject expBarObj = this.transform.Find("EXPBarGuage").gameObject;
            expBarObj.SetActive(true);
            expBarObj.transform.localScale = new Vector3(expRad, 1, 1);
        }
    }

    protected override void OnClickEvent(_mouseType button) {
        PlayerDataManager.SetHeroInParty(partyNum, hero);
        LobbySceneObject.CloseHeroSelectUI();
    }
}
