using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleStartButton : ClickObject
{
    public GameObject titleOvject = null;
    protected override void OnUpEvent(_mouseType button) {
        if (button == _mouseType.l) {
            titleOvject.GetComponent<TitleSceneObject>().OnStartButton();
        }
    }
}
