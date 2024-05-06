using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScrolBarClick : ClickObject {
    public GameObject UIScrollObject = null;
    UIScroll uiScrollSC = null;

    private void Awake() {
        Awake_clickObject();
        uiScrollSC = UIScrollObject.GetComponent<UIScroll>();
    }

    override protected void OnDownEvent(_mouseType button) {
        if (button == _mouseType.l) uiScrollSC.BarOnDown();
    }

    override protected void OnDragEvent(_mouseType button) {
        if (button == _mouseType.l) uiScrollSC.BarDrag();
    }

    override protected void OnWheelEvent(float v) {
        uiScrollSC.OnWheel(v);
    }
}
