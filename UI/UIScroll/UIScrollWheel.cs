using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScrollWheel : ClickObject {
    UIScroll uiScrollSC = null;

    private void Awake() {
        Awake_clickObject();
        uiScrollSC = this.gameObject.GetComponent<UIScroll>();
    }

    override protected void OnWheelEvent(float v) {
        uiScrollSC.OnWheel(v);
    }
}
