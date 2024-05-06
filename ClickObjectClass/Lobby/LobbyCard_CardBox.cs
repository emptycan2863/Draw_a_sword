using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LobbyCard_CardBox : LobbyCardObject {
    protected override void OnDownEvent(_mouseType button) {
        if (button == _mouseType.l) {
            LobbySceneObject.OnDownBoxCard(this);
        }
    }

    protected override void OnUpEvent(_mouseType button) {
        if (button == _mouseType.l) {
            LobbySceneObject.OnUpBoxCard(this);
        }
    }

    protected override void OnClickEvent(_mouseType button) {
        if (button == _mouseType.r) {
            LobbySceneObject.OnRClickBoxCard(this);
        }
    }
}
