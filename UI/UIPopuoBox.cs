using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIPopuoBox : MonoBehaviour
{
    static UIPopuoBox _this = null;
    static CallBackClass button1cbc = null;
    static CallBackClass button2cbc = null;

    void Start()
    {
        _this = this;
        Close();
    }

    static public void SetPopupOneButton(CallBackClass cbc = null, int textIndex = 0, int buttonTextIndex = 300024) {
        _this.gameObject.SetActive(true);
        Text textComp;

        textComp = _this.transform.Find("BoxText").gameObject.GetComponent<Text>();
        textComp.text = ScriptData.GetScript(textIndex);

        _this.transform.Find("Button1").gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, -160, 0);

        textComp = _this.transform.Find("Button1").Find("ButtonText").gameObject.GetComponent<Text>();
        textComp.text = ScriptData.GetScript(buttonTextIndex);

        _this.transform.Find("Button2").gameObject.SetActive(false);

        button1cbc = cbc;
        button2cbc = null;
    }

    static public void SetPopupTwoButton(CallBackClass cbc1 = null, CallBackClass cbc2 = null, int textIndex = 0, int button1TextIndex = 300024, int button2TextIndex = 300025) {
        _this.gameObject.SetActive(true);
        Text textComp;

        textComp = _this.transform.Find("BoxText").gameObject.GetComponent<Text>();
        textComp.text = ScriptData.GetScript(textIndex);

        _this.transform.Find("Button1").gameObject.GetComponent<RectTransform>().localPosition = new Vector3(180, -160, 0);

        textComp = _this.transform.Find("Button1").Find("ButtonText").gameObject.GetComponent<Text>();
        textComp.text = ScriptData.GetScript(button1TextIndex);

        _this.transform.Find("Button2").gameObject.SetActive(true);

        textComp = _this.transform.Find("Button2").Find("ButtonText").gameObject.GetComponent<Text>();
        textComp.text = ScriptData.GetScript(button2TextIndex);

        button1cbc = cbc1;
        button2cbc = cbc2;
    }

    static public void OnClickButton1() {
        if (button1cbc != null) button1cbc.Func();
        Close();
    }

    static public void OnClickButton2() {
        if (button2cbc != null) button2cbc.Func();
        Close();
    }

    static public void Close() {
        button1cbc = null;
        button2cbc = null;
        _this.gameObject.SetActive(false);
    }
}
