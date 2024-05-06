using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyButton_Dungeon : ClickObject
{
    public GameObject ButtonOverObject = null;

    void Awake() {
        Awake_clickObject();
        Awake_LobbyButton_BuildDeck();
    }

    void Awake_LobbyButton_BuildDeck() {
        if (ButtonOverObject != null) {
            ButtonOverObject.SetActive(false);
        }
    }

    protected override void OnOverEvent() {
        if (ButtonOverObject != null) {
            ButtonOverObject.SetActive(true);
        }
    }

    protected override void OnOutEvent() {
        if (ButtonOverObject != null) {
            ButtonOverObject.SetActive(false);
        }
    }

    protected override void OnClickEvent(_mouseType button) {
        LobbySceneObject.GoStageSelect();
    }
}
