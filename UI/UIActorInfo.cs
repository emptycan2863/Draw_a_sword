using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIActorInfo : MonoBehaviour {
    static UIActorInfo thisUI = null;
    private void Awake() {
        thisUI = this;
        this.gameObject.SetActive(false);
    }

    static GameObject iconObject = null;

    public static void SetActorData(ActorData actor = null) {
        if (actor == null) {
            thisUI.gameObject.SetActive(false);
            return;
        }

        if (iconObject != null) {
            GameObject.Destroy(iconObject);
            iconObject = null;
        }
        Transform iconFrame = thisUI.transform.Find("ActorIcon");
        iconObject = UIManager.IconInstantiate(actor, iconFrame);

        string srt = ScriptData.GetScript(300007);

        srt = srt.Replace("{0}", "" + actor.GetLevel());
        srt = srt.Replace("{1}", "" + actor.GetName());
        thisUI.transform.Find("LvNameText").gameObject.GetComponent<Text>().text = srt;

        switch (actor.GetActorType()) {
            case _actorType.hero:
                srt = ScriptData.GetScript(300008);
                srt = srt.Replace("{0}", "" + actor.GetStrength());
                srt = srt.Replace("{1}", "" + actor.GetIntelligence());
                srt = srt.Replace("{2}", "" + actor.GetDexterity());
                break;
            case _actorType.enemy:
                switch (actor.GetRank()) {
                    case 1:
                        srt = ScriptData.GetScript(300019);
                        break;
                    case 2:
                        srt = ScriptData.GetScript(300020);
                        break;
                    case 3:
                        srt = ScriptData.GetScript(300021);
                        break;
                    case 4:
                        srt = ScriptData.GetScript(300022);
                        break;
                }
                break;
        }

        thisUI.transform.Find("StatusText").gameObject.GetComponent<Text>().text = srt;

        thisUI.gameObject.SetActive(true);
    }
}
