using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UICardViewer : MonoBehaviour
{
    static UICardViewer _this = null;
    public GameObject cardObj = null;
    public GameObject cardViewer = null;
    static CardData thisCardData = null;

    private void Awake() {
        _this = this;
        SetCardViewer();
    }

    static public void SetCardViewer(CardData card = null) {
        thisCardData = card;
        Transform tr = _this.cardObj.transform;
        if (card != null) {
            _this.gameObject.SetActive(true);
            card.InitCardGraphic(tr);
            card.InitCardNameValue(tr);
            _this.cardViewer.GetComponent<CardViewer>().SetCardData(card);
        } else {
            _this.gameObject.SetActive(false);
        }
    }
}
