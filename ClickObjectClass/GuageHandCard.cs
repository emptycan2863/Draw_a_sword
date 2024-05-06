using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuageHandCard : ClickObject {
    public HeroActorData hero = null;
    public GameObject prefabCardObj = null;
    float posXtemp = 0f;

    protected override void OnOverEvent() {
        posXtemp = 0f;
        if (hero != null) {
            foreach (CardData card in hero.GetHandCard()) {
                GameObject obj = Instantiate(prefabCardObj, this.transform);
                card.InitCardGraphic(obj.transform.Find("UICardGraphic"));
                obj.GetComponent<GuageHandCardOver>().linkCardData = card;
                obj.transform.position = new Vector3(obj.transform.position.x + posXtemp, obj.transform.position.y, obj.transform.position.z);
                posXtemp += 45f;
            }
        }
    }

    protected override void OnOutEvent() {
        foreach (Transform tr in this.transform) {
            GameObject.Destroy(tr.gameObject);
        }
    }
}
