using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby_CardBoxDragCheck : ClickObject {
    protected override void OnDragOverEvent() {
        LobbySceneObject.DragInBoxCheck(true);
    }

    protected override void OnDragOutEvent() {
        LobbySceneObject.DragInBoxCheck(false);
    }

    protected override void OnDragUpEvent(_mouseType button) {
        LobbySceneObject.OnDragUpCardBox();
    }
}
