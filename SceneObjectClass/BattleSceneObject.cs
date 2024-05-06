using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneObject : SceneObject {
    // Start is called before the first frame update
    void Start()
    {
        GameManager.gameMode = _gamemode.battle;
        StartBattle();
    }

    void StartBattle() {
        SceneObjectStart();
        UIManager.SetBattleUIActive(true);
        TurnManager.StartGame();
    }
}
