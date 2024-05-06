using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupBoxButton : ClickObject {
    public int buttonNum = 1;

    protected override void OnClickEvent(_mouseType button) {
        if (button == _mouseType.l) {
            switch (buttonNum) {
                case 1:
                    UIPopuoBox.OnClickButton1();
                    break;
                case 2:
                    UIPopuoBox.OnClickButton2();
                    break;
            }
        }
    }
}
