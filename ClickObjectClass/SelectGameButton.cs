using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SelectGameButton : ClickObject
{
    public GameObject titleSceneObject = null;
    public GameObject saveText = null;
    public GameObject newGameObject = null;
    public GameObject loadGameObjecrt = null;
    public GameObject playTimeText = null;
    int code = -1;

    bool clickCheck() {
        return titleSceneObject.GetComponent<TitleSceneObject>().GetSlotClickCheck();
    }

    protected override void OnOverEvent() {
        OnOver();
    }

    public void OnOver() {
        if (!clickCheck()) return;
        this.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.04f, 1.04f, 1);
    }

    protected override void OnOutEvent() {
        OnOut();
    }

    public void OnOut() {
        if (!clickCheck()) return;
        this.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }

    public void SetNumber(int n) {
        saveText.GetComponent<Text>().text = "SAVE " + (n+1);

        JsObject saveData = SaveDataManager.GetSaveData(n);

        if (saveData == null) {
            loadGameObjecrt.SetActive(false);
            newGameObject.SetActive(true);
        } else {
            loadGameObjecrt.SetActive(true);
            newGameObject.SetActive(false);

            JsObject playTimeData = saveData.GetDictionary()["playTime"];
            float playTime = playTimeData.GetFloat();
            int h = (int)(playTime / 3600);
            playTime = playTime % 3600;
            int m = (int)(playTime / 60);
            playTime = playTime % 60;
            int s = (int)playTime;

            string timeText = h < 10 ? "0" + h : "" + h;
            timeText += ":";
            timeText += m < 10 ? "0" + m : "" + m;
            timeText += ":";
            timeText += s < 10 ? "0" + s : "" + s;
            playTimeText.GetComponent<Text>().text = timeText;
        }

        code = n;
    }

    protected override void OnClickEvent(_mouseType button) {
        if (!clickCheck()) return;
        if (button == _mouseType.l) {
            titleSceneObject.GetComponent<TitleSceneObject>().OnClickSelectGame(code);
        }
    }

    class DeleteButtonCBC : CallBackClass {
        public SelectGameButton _this = null;
        public override void Func() {
            SaveDataManager.DeleteSaveData(_this.code);
            _this.SetNumber(_this.code);
        }
    }

    public void Delete() {
        DeleteButtonCBC cbc = new DeleteButtonCBC();
        cbc._this = this;
        UIPopuoBox.SetPopupTwoButton(cbc, null, 300026);
    }
}
