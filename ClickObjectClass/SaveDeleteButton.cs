using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDeleteButton : ClickObject {
    public GameObject selectGameObject = null;

    protected override void OnOverEvent() {
        selectGameObject.GetComponent<SelectGameButton>().OnOver();
    }

    protected override void OnOutEvent() {
        selectGameObject.GetComponent<SelectGameButton>().OnOut();
    }

    protected override void OnClickEvent(_mouseType button) {
        selectGameObject.GetComponent<SelectGameButton>().Delete();
    }
}
