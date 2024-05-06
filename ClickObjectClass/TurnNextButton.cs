using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnNextButton : ClickObject {
    protected override void OnClickEvent(_mouseType button) {
        switch (button) {
            case _mouseType.l:
                UINextArrow.OnClick();
                break;
        }
    }
}
