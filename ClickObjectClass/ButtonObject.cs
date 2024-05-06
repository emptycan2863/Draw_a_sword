using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonObjectCallBack : CallBackClass { 
    public _mouseType mouseType = _mouseType.l;
}

public class ButtonObject : ClickObject
{
    ButtonObjectCallBack onClickCallBack = null;
    public Text text = null;

    override protected void OnClickEvent(_mouseType button) {
        if (onClickCallBack == null) return;
        onClickCallBack.mouseType = button;
        onClickCallBack.Func();
    }

    public void SetOnClickCallBackClass(ButtonObjectCallBack cbc) {
        onClickCallBack = cbc;
    }

    public void SetButtonActive(bool active) {
        clickActive = active;
        GetComponent<Image>().color = active ? new Color(1, 1, 1, 1) : new Color(0.5f, 0.5f, 0.5f, 1);
        text.color = active ? new Color(1, 1, 1, 1) : new Color(0.5f, 0.5f, 0.5f, 1);
    }
}
