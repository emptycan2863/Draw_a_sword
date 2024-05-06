using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby_BackTitle : ClickObject {
    protected override void OnClickEvent(_mouseType button) {
        PlayerDataManager.SaveData();
        GameManager.SceneMove("Scene1Title");
    }
}