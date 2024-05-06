using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum _cardTagType { none, cardActive,cardPassive }

public class UITag : MonoBehaviour
{
    static UITag thisUi = null;
    public GameObject tagBox = null;
    static GameObject sTagBox = null;

    public GameObject tagText = null;
    static GameObject sTagText = null;

    private void Awake() {
        thisUi = this;
        sTagBox = tagBox;
        sTagText = tagText;

        thisUi.gameObject.SetActive(false);
        SetTag();
    }

    static public void UIUpdate() {
        RectTransform rt = sTagBox.GetComponent<RectTransform>();
        float sizeX = rt.sizeDelta.x;
        float sizeY = rt.sizeDelta.y;
        float moustX = Input.mousePosition.x;
        float moustY = Input.mousePosition.y;

        float posX = 0;
        if (moustX - 10 - sizeX < 0) {
            posX = moustX + 10 + sizeX / 2;
        } else {
            posX = moustX - 10 - sizeX / 2;
        }

        float posY = 0;
        if (moustY - 40 - sizeY < 0) {
            posY = sizeY / 2;
        } else {
            posY = moustY - 40 - sizeY / 2;
        }

        rt.position = new Vector2(posX, posY);
    }

    static public void ClearTag() {
        thisUi.gameObject.SetActive(false);
        return;
    }

    static public void SetTag(List<string> stringList = null) {
        if (stringList == null || stringList.Count == 0) {
            ClearTag();
            return;
        }

        string tagSt = "";
        foreach (string st in stringList) {
            if (tagSt != "") tagSt += "\n";
            tagSt += DataManager.GetCardTag(st);
        }
        Text textBox = sTagText.GetComponent<Text>();
        textBox.text = tagSt;

        thisUi.gameObject.SetActive(true);

        RectTransform rt = sTagBox.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, textBox.preferredHeight + 10);
    }

    static public void SetTagText(string text = "") {
        if (text == "") {
            ClearTag();
            return;
        }

        Text textBox = sTagText.GetComponent<Text>();
        textBox.text = text;

        thisUi.gameObject.SetActive(true);

        RectTransform rt = sTagBox.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, textBox.preferredHeight + 10);
    }
}
