using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_CardSelect : CardObject {
    CallBackClass lClickCbc = null;

    public void SetLClickFn(CallBackClass cbc) {
        lClickCbc = cbc;
    }

    override protected void OnClickEvent(_mouseType button) {
        if (button == _mouseType.l) {
            if (lClickCbc != null) lClickCbc.Func();
        }
        return;
    }

    public void SelectCheck(bool check) {
        this.transform.Find("check").gameObject.SetActive(check);
    }
}
